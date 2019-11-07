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
	using System.Data;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
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

		private ImportBulkArtifactJob importJob;

		protected ImportJobTestBase()
		{
			Assume.That(AssemblySetup.TestParameters.WorkspaceId, Is.Positive, "The test workspace must be created or specified in order to run this integration test.");

			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
		}

		protected ImportTestJobResult TestJobResult { get; private set; }

		protected TempDirectory2 TempDirectory { get; private set; }

		protected DateTime Timestamp { get; private set; }

		protected DataTable SourceData { get; set; }

		[SetUp]
		public void Setup()
		{
			this.Timestamp = DateTime.Now;
			this.TempDirectory = new TempDirectory2();
			this.TempDirectory.Create();
			this.SourceData = new DataTable { Locale = CultureInfo.InvariantCulture };
			this.SourceData.Columns.Add(WellKnownFields.ControlNumber, typeof(string));
			this.SourceData.Columns.Add(WellKnownFields.FilePath, typeof(string));
			this.TestJobResult = new ImportTestJobResult();
			this.importJob = null;
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
			FileSecurity accessControl = File.GetAccessControl(path);
			var sid = new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null);
			foreach (FileSystemAccessRule rule in accessControl.GetAccessRules(true, true, typeof(NTAccount)))
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

		protected void GivenTheDatasetPathToImport(string file)
		{
			string uniqueControlId = Path.GetFileName(file) + " - " + this.Timestamp.Ticks;
			this.SourceData.Rows.Add(uniqueControlId + " " + 1, file);
		}

		protected void GivenTheSourceFileIsLocked(int index)
		{
			var filePath = this.SourceData.Rows[index][1].ToString();
			ChangeFileFullPermissions(filePath, false);
		}

		protected void GivenTheImportJob()
		{
			var iapi = new ImportAPI(
				AssemblySetup.TestParameters.RelativityUserName,
				AssemblySetup.TestParameters.RelativityPassword,
				AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
			this.InitializeDefaultImportJob(iapi);
		}

		protected void GivenTheImportJobWithIntegratedAuthentication()
		{
			var iapi = new ImportAPI(AssemblySetup.TestParameters.RelativityWebApiUrl.ToString());
			this.InitializeDefaultImportJob(iapi);
		}

		protected void WhenExecutingTheJob()
		{
			this.importJob.Execute();
			Console.WriteLine("Import API elapsed time: {0}", this.TestJobResult.CompletedJobReport.EndTime - this.TestJobResult.CompletedJobReport.StartTime);
		}

		protected void ThenTheImportJobIsSuccessful()
		{
			Assert.That(this.TestJobResult.ErrorRows, Has.Count.Zero);
			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Zero);
			Assert.That(this.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.Zero);
			Assert.That(this.TestJobResult.CompletedJobReport.FatalException, Is.Null);
			Assert.That(this.TestJobResult.CompletedJobReport.TotalRows, Is.EqualTo(this.SourceData.Rows.Count));
		}

		protected void ThenTheImportJobIsNotSuccessful(int expectedErrorRows, int expectedTotalRows, bool fatalExceptions)
		{
			Assert.That(this.TestJobResult.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
			if (fatalExceptions)
			{
				Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Positive);
				Assert.That(this.TestJobResult.CompletedJobReport.FatalException, Is.Not.Null);

				// Note: the exact number of expected rows can vary over a range when expecting an error.
				Assert.That(this.TestJobResult.CompletedJobReport.TotalRows, Is.AtLeast(1).And.LessThanOrEqualTo(expectedTotalRows));
			}
			else
			{
				Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Zero);
				Assert.That(this.TestJobResult.CompletedJobReport.FatalException, Is.Null);
				Assert.That(this.TestJobResult.CompletedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));
			}

			Assert.That(this.TestJobResult.CompletedJobReport, Is.Not.Null);
			Assert.That(this.TestJobResult.CompletedJobReport.ErrorRows, Has.Count.EqualTo(expectedErrorRows));
		}

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

		protected void GivenTheDatasetPathToImport(string path, string searchPattern, SearchOption searchOption)
		{
			var number = 1;
			foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
			{
				string uniqueControlId = Path.GetFileName(file) + " - " + this.Timestamp.Ticks;
				this.SourceData.Rows.Add(uniqueControlId + " " + number, file);
			}
		}

		private void ImportJob_OnComplete(JobReport jobReport)
		{
			lock (SyncRoot)
			{
				this.TestJobResult.CompletedJobReport = jobReport;
				Console.WriteLine("[Job Complete]");
			}
		}

		private void ImportJob_OnProgress(long completedRow)
		{
			lock (SyncRoot)
			{
				this.TestJobResult.ProgressCompletedRows.Add(completedRow);
				Console.WriteLine("[Job Progress]: " + completedRow);
			}
		}

		private void ImportJob_OnMessage(Status status)
		{
			lock (SyncRoot)
			{
				this.TestJobResult.JobMessages.Add(status.Message);
				Console.WriteLine("[Job Message]: " + status.Message);
			}
		}

		private void ImportJob_OnFatalException(JobReport jobReport)
		{
			lock (SyncRoot)
			{
				this.TestJobResult.JobFatalExceptions.Add(jobReport.FatalException);
				Console.WriteLine("[Job Fatal Exception]: " + jobReport.FatalException);
			}
		}

		private void ImportJob_OnError(IDictionary row)
		{
			lock (SyncRoot)
			{
				this.TestJobResult.ErrorRows.Add(row);
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