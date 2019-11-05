// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportJobTestBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract load-file base class.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Security.AccessControl;
	using System.Security.Principal;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;

	public abstract class ImportJobTestBase : IDisposable
	{
		internal const int MinTestFileLength = 1024;

		internal const int MaxTestFileLength = 10 * MinTestFileLength;

		private static readonly object SyncRoot = new object();

		private readonly List<string> jobMessages = new List<string>();

		private readonly List<Exception> jobFatalExceptions = new List<Exception>();

		private readonly List<IDictionary> errorRows = new List<IDictionary>();

		private readonly List<long> progressCompletedRows = new List<long>();

		private ImportBulkArtifactJob importJob;

		private JobReport completedJobReport;

		protected ImportJobTestBase()
		{
			Assume.That(AssemblySetup.TestParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
		}

		protected TempDirectory2 TempDirectory
		{
			get;
			private set;
		}

		protected DateTime Timestamp
		{
			get;
			private set;
		}

		protected DataTable SourceData
		{
			get;
			set;
		}

		[SetUp]
		public void Setup()
		{
			this.Timestamp = DateTime.Now;
			this.TempDirectory = new TempDirectory2();
			this.TempDirectory.Create();
			this.SourceData = new DataTable { Locale = CultureInfo.InvariantCulture };
			this.SourceData.Columns.Add(WellKnownFields.ControlNumber, typeof(string));
			this.SourceData.Columns.Add(WellKnownFields.FilePath, typeof(string));
			this.jobMessages.Clear();
			this.jobFatalExceptions.Clear();
			this.errorRows.Clear();
			this.progressCompletedRows.Clear();
			this.importJob = null;
			this.completedJobReport = null;
		}

		[TearDown]
		public void Teardown()
		{
			if (this.TempDirectory != null)
			{
				try
				{
					string[] files = Directory.GetFiles(this.TempDirectory.Directory, "*");
					foreach (var file in files)
					{
						RestoreFileFullPermissions(file);
					}
				}
				finally
				{
					this.TempDirectory.ClearReadOnlyAttributes = true;
					this.TempDirectory.Dispose();
					this.TempDirectory = null;
				}
			}

			this.SourceData?.Dispose();
			if (this.importJob != null)
			{
				this.importJob.OnError -= this.ImportJob_OnError;
				this.importJob.OnFatalException -= this.ImportJob_OnFatalException;
				this.importJob.OnMessage -= this.ImportJob_OnMessage;
				this.importJob.OnComplete -= this.ImportJob_OnComplete;
				this.importJob.OnProgress -= this.ImportJob_OnProgress;
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		protected static void ChangeFileFullPermissions(string path, bool grant)
		{
			var accessControl = File.GetAccessControl(path);
			var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
			accessControl.AddAccessRule(
				new FileSystemAccessRule(
					sid,
					FileSystemRights.FullControl,
					grant ? AccessControlType.Allow : AccessControlType.Deny));
			File.SetAccessControl(path, accessControl);
		}

		protected static void RestoreFileFullPermissions(string path)
		{
			var accessControl = File.GetAccessControl(path);
			var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
			foreach (FileSystemAccessRule rule in accessControl.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount)))
			{
				if (rule.AccessControlType == AccessControlType.Deny)
				{
					accessControl.RemoveAccessRule(rule);
				}
			}

			accessControl.AddAccessRule(
				new FileSystemAccessRule(
					sid,
					FileSystemRights.FullControl,
					AccessControlType.Allow));
			File.SetAccessControl(path, accessControl);
		}

		protected static void GivenTheStandardConfigSettings(
			TapiClient forceClient,
			bool disableNativeLocationValidation,
			bool disableNativeValidation)
		{
			kCura.WinEDDS.Config.ConfigSettings["BadPathErrorsRetry"] = false;
			kCura.WinEDDS.Config.ConfigSettings["ForceWebUpload"] = false;
			kCura.WinEDDS.Config.ConfigSettings["PermissionErrorsRetry"] = false;
			kCura.WinEDDS.Config.ConfigSettings["TapiForceAsperaClient"] = (forceClient == TapiClient.Aspera).ToString();
			kCura.WinEDDS.Config.ConfigSettings["TapiForceFileShareClient"] = (forceClient == TapiClient.Direct).ToString();
			kCura.WinEDDS.Config.ConfigSettings["TapiForceHttpClient"] = (forceClient == TapiClient.Web).ToString();
			kCura.WinEDDS.Config.ConfigSettings["TapiMaxJobRetryAttempts"] = 1;
			kCura.WinEDDS.Config.ConfigSettings["TapiMaxJobParallelism"] = 1;
			kCura.WinEDDS.Config.ConfigSettings["TapiLogEnabled"] = true;
			kCura.WinEDDS.Config.ConfigSettings["TapiSubmitApmMetrics"] = false;
			AppSettings.Instance.IoErrorWaitTimeInSeconds = 0;
			AppSettings.Instance.IoErrorNumberOfRetries = 0;
			kCura.WinEDDS.Config.ConfigSettings["UsePipeliningForFileIdAndCopy"] = false;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			// Note: there's no longer a BCP sub-folder.
			kCura.WinEDDS.Config.ConfigSettings["TapiAsperaBcpRootFolder"] = string.Empty;
			kCura.WinEDDS.Config.ConfigSettings["TapiAsperaNativeDocRootLevels"] = 1;
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Teardown();
			}
		}

		/// <summary>
		/// Given the dataset path to import.
		/// </summary>
		/// <param name="file">
		/// The file to import.
		/// </param>
		protected void GivenTheDatasetPathToImport(string file)
		{
			var controlId = Path.GetFileName(file) + " - " + this.Timestamp.Ticks;
			this.SourceData.Rows.Add(controlId + " " + 1, file);
		}

		/// <summary>
		/// Given the source files are locked.
		/// </summary>
		/// <param name="index">
		/// Specify the zero-based index.
		/// </param>
		protected void GivenTheSourceFileIsLocked(int index)
		{
			var filePath = this.SourceData.Rows[index][1].ToString();
			ChangeFileFullPermissions(filePath, false);
		}

		/// <summary>
		/// Given the import job.
		/// </summary>
		protected void GivenTheImportJob()
		{
			var iapi = new ImportAPI(
				AssemblySetup.TestParameters.RelativityUserName,
				AssemblySetup.TestParameters.RelativityPassword,
				AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
			this.InitializeDefaultImportJob(iapi);
		}

		/// <summary>
		/// Given the import job with integrated authentication.
		/// </summary>
		protected void GivenTheImportJobWithIntegratedAuthentication()
		{
			var iapi = new ImportAPI(AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
			this.InitializeDefaultImportJob(iapi);
		}

		protected void WhenExecutingTheJob()
		{
			var sw = Stopwatch.StartNew();
			this.importJob.Execute();
			sw.Stop();
			Console.WriteLine("Import API elapsed time: {0}", sw.Elapsed);
		}

		protected void ThenTheImportJobIsSuccessful()
		{
			Assert.That(this.errorRows.Count, Is.EqualTo(0));
			Assert.That(this.jobFatalExceptions.Count, Is.EqualTo(0));
			Assert.That(this.completedJobReport, Is.Not.Null);
			Assert.That(this.completedJobReport.ErrorRows.Count, Is.EqualTo(0));
			Assert.That(this.completedJobReport.FatalException, Is.Null);
			Assert.That(this.completedJobReport.TotalRows, Is.EqualTo(this.SourceData.Rows.Count));
		}

		/// <summary>
		/// Then the import job is successful.
		/// </summary>
		/// <param name="expectedErrorRows">
		/// The expected number of error rows.
		/// </param>
		/// <param name="expectedTotalRows">
		/// The expected number of total rows.
		/// </param>
		/// <param name="fatalExceptions">
		/// Specify whether fatal exceptions are expected.
		/// </param>
		protected void ThenTheImportJobIsNotSuccessful(int expectedErrorRows, int expectedTotalRows, bool fatalExceptions)
		{
			Assert.That(this.errorRows.Count, Is.EqualTo(expectedErrorRows));
			if (fatalExceptions)
			{
				Assert.That(this.jobFatalExceptions.Count, Is.GreaterThan(0));
				Assert.That(this.completedJobReport.FatalException, Is.Not.Null);

				// Note: the exact number of expected rows can vary over a range when expecting an error.
				Assert.That(this.completedJobReport.TotalRows, Is.AtLeast(1).And.LessThanOrEqualTo(expectedTotalRows));
			}
			else
			{
				Assert.That(this.jobFatalExceptions.Count, Is.EqualTo(0));
				Assert.That(this.completedJobReport.FatalException, Is.Null);
				Assert.That(this.completedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));
			}

			Assert.That(this.completedJobReport, Is.Not.Null);
			Assert.That(this.completedJobReport.ErrorRows.Count, Is.EqualTo(expectedErrorRows));
		}

		/// <summary>
		/// Then the import progress events are raised.
		/// </summary>
		protected void ThenTheImportProgressEventsAreRaised()
		{
			this.ThenTheImportProgressEventsCountShouldEqual(this.SourceData.Rows.Count);
		}

		/// <summary>
		/// Then the import progress events count should equal the specified value.
		/// </summary>
		/// <param name="expected">
		/// The expected count.
		/// </param>
		protected void ThenTheImportProgressEventsCountShouldEqual(int expected)
		{
			Assert.That(this.progressCompletedRows.Count, Is.EqualTo(expected));
		}

		protected void ThenTheImportProgressEventsCountIsNonZero()
		{
			Assert.That(this.progressCompletedRows.Count, Is.GreaterThan(0));
		}

		/// <summary>
		/// Then the import message count is non zero.
		/// </summary>
		protected void ThenTheImportMessageCountIsNonZero()
		{
			Assert.That(this.jobMessages.Count, Is.GreaterThan(0));
		}

		/// <summary>
		/// Then the import messages contains the specified message.
		/// </summary>
		/// <param name="message">
		/// The message to check.
		/// </param>
		protected void ThenTheImportMessagesContains(string message)
		{
			Assert.That(this.jobMessages.Any(x => x.Contains(message)), Is.True);
		}

		/// <summary>
		/// Creates the unique control identifier.
		/// </summary>
		/// <param name="file">
		/// The full path to the source file.
		/// </param>
		/// <returns>
		/// The control identifier.
		/// </returns>
		protected string CreateUniqueControlId(string file)
		{
			return Path.GetFileName(file) + " - " + this.Timestamp.Ticks;
		}

		/// <summary>
		/// Given the dataset is auto-generated by the number of specified files.
		/// </summary>
		/// <param name="maxFiles">
		/// The file limit.
		/// </param>
		/// <param name="includeReadOnlyFiles">
		/// Specify whether to include read-only files in the dataset.
		/// </param>
		protected void GivenTheAutoGeneratedDatasetToImport(int maxFiles, bool includeReadOnlyFiles)
		{
			for (var i = 0; i < maxFiles; i++)
			{
				RandomHelper.NextTextFile(
					MinTestFileLength,
					MaxTestFileLength,
					this.TempDirectory.Directory,
					includeReadOnlyFiles && i % 2 == 0);
			}

			this.GivenTheDatasetPathToImport(this.TempDirectory.Directory, "*", SearchOption.AllDirectories);
		}

		/// <summary>
		/// Given the dataset path to import.
		/// </summary>
		/// <param name="path">
		/// The dataset path to import.
		/// </param>
		/// <param name="searchPattern">
		/// Specify the search pattern.
		/// </param>
		/// <param name="searchOption">
		/// Specify the search option.
		/// </param>
		protected void GivenTheDatasetPathToImport(string path, string searchPattern, SearchOption searchOption)
		{
			var number = 1;
			foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
			{
				var controlId = this.CreateUniqueControlId(file);
				this.SourceData.Rows.Add(controlId + " " + number, file);
			}
		}

		/// <summary>
		/// The import job complete handler.
		/// </summary>
		/// <param name="jobReport">
		/// The job report.
		/// </param>
		private void ImportJob_OnComplete(JobReport jobReport)
		{
			lock (SyncRoot)
			{
				this.completedJobReport = jobReport;
				Console.WriteLine("[Job Complete]");
			}
		}

		/// <summary>
		/// The import job progress handler.
		/// </summary>
		/// <param name="completedRow">
		/// The completed row.
		/// </param>
		private void ImportJob_OnProgress(long completedRow)
		{
			lock (SyncRoot)
			{
				this.progressCompletedRows.Add(completedRow);
				Console.WriteLine("[Job Progress]: " + completedRow);
			}
		}

		/// <summary>
		/// The import job message handler.
		/// </summary>
		/// <param name="status">
		/// The status.
		/// </param>
		private void ImportJob_OnMessage(Status status)
		{
			lock (SyncRoot)
			{
				this.jobMessages.Add(status.Message);
				Console.WriteLine("[Job Message]: " + status.Message);
			}
		}

		/// <summary>
		/// The import job fatal exception handler.
		/// </summary>
		/// <param name="jobReport">
		/// The job report.
		/// </param>
		private void ImportJob_OnFatalException(JobReport jobReport)
		{
			lock (SyncRoot)
			{
				this.jobFatalExceptions.Add(jobReport.FatalException);
				Console.WriteLine("[Job Fatal Exception]: " + jobReport.FatalException);
			}
		}

		/// <summary>
		/// The import job error handler.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		private void ImportJob_OnError(IDictionary row)
		{
			lock (SyncRoot)
			{
				this.errorRows.Add(row);
				StringBuilder rowMetaData = new StringBuilder();
				foreach (string key in row.Keys)
				{
					if (rowMetaData.Length > 0)
					{
						rowMetaData.Append(",");
					}

					rowMetaData.AppendFormat("{0}={1}", key, row[key]);
				}

				Console.WriteLine("[Job Error Metadata]: " + rowMetaData);
			}
		}

		/// <summary>
		/// Creates import job and sets default parameters.
		/// </summary>
		/// <param name="importApi"><see cref="ImportAPI"/> instance used to create a job.</param>
		private void InitializeDefaultImportJob(ImportAPI importApi)
		{
			this.importJob = importApi.NewNativeDocumentImportJob();
			this.importJob.Settings.WebServiceURL = AssemblySetup.TestParameters.RelativityWebApiUrl.ToString();
			this.importJob.Settings.CaseArtifactId = AssemblySetup.TestParameters.WorkspaceId;
			this.importJob.Settings.ArtifactTypeId = 10;
			this.importJob.Settings.ExtractedTextFieldContainsFilePath = false;
			this.importJob.Settings.NativeFilePathSourceFieldName = WellKnownFields.FilePath;
			this.importJob.Settings.SelectedIdentifierFieldName = WellKnownFields.ControlNumber;
			this.importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
			this.importJob.Settings.OverwriteMode = OverwriteModeEnum.Append;
			this.importJob.Settings.OIFileIdMapped = true;
			this.importJob.Settings.OIFileIdColumnName = WellKnownFields.OutsideInFileId;
			this.importJob.Settings.OIFileTypeColumnName = WellKnownFields.OutsideInFileType;
			this.importJob.Settings.ExtractedTextEncoding = Encoding.Unicode;
			this.importJob.Settings.FileSizeMapped = true;
			this.importJob.Settings.FileSizeColumn = WellKnownFields.NativeFileSize;
			this.importJob.SourceData.SourceData = this.SourceData.CreateDataReader();
			this.importJob.OnError += this.ImportJob_OnError;
			this.importJob.OnFatalException += this.ImportJob_OnFatalException;
			this.importJob.OnMessage += this.ImportJob_OnMessage;
			this.importJob.OnComplete += this.ImportJob_OnComplete;
			this.importJob.OnProgress += this.ImportJob_OnProgress;
		}
	}
}