// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportJobTest.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ImportAPI"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Net;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.Relativity.ImportAPI;

	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;
	using Relativity.Transfer;

	/// <summary>
	/// Tests an import job.
	/// </summary>
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportJobTest
	{
		/// <summary>
		/// The minimum test file length [1KB].
		/// </summary>
		private const int MinTestFileLength = 1024;

		/// <summary>
		/// The maximum test file length [10KB].
		/// </summary>
		private const int MaxTestFileLength = 10 * MinTestFileLength;

		/// <summary>
		/// The thread synchronization backing.
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// The job messages.
		/// </summary>
		private readonly List<string> jobMessages = new List<string>();

		/// <summary>
		/// The job fatal exceptions.
		/// </summary>
		private readonly List<Exception> jobFatalExceptions = new List<Exception>();

		/// <summary>
		/// The error rows.
		/// </summary>
		private readonly List<IDictionary> errorRows = new List<IDictionary>();

		/// <summary>
		/// The progress completed rows.
		/// </summary>
		private readonly List<long> progressCompletedRows = new List<long>();

		/// <summary>
		/// The test directory backing.
		/// </summary>
		private TempDirectory2 testDirectory;

		/// <summary>
		/// The source data.
		/// </summary>
		private System.Data.DataTable sourceData;

		/// <summary>
		/// The import job.
		/// </summary>
		private ImportBulkArtifactJob importJob;

		/// <summary>
		/// The completed job report.
		/// </summary>
		private JobReport completedJobReport;

		/// <summary>
		/// The time the job was run.
		/// </summary>
		private DateTime runTime;

		/// <summary>
		/// The integration test parameters.
		/// </summary>
		private IntegrationTestParameters testParameters;

		/// <summary>
		/// The test setup.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			ServicePointManager.SecurityProtocol =
				SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
				| SecurityProtocolType.Tls12;
			this.testParameters = AssemblySetup.TestParameters.DeepCopy();
			this.testDirectory = new TempDirectory2 { ClearReadOnlyAttributes = true };
			this.testDirectory.Create();
			this.sourceData = new System.Data.DataTable();
			this.sourceData.Columns.Add(WellKnownFields.ControlNumber, typeof(string));
			this.sourceData.Columns.Add(WellKnownFields.FilePath, typeof(string));
			this.jobMessages.Clear();
			this.jobFatalExceptions.Clear();
			this.errorRows.Clear();
			this.progressCompletedRows.Clear();
			this.importJob = null;
			this.completedJobReport = null;
			this.runTime = DateTime.Now;
		}

		/// <summary>
		/// The test tear down.
		/// </summary>
		[TearDown]
		public void TearDown()
		{
			if (this.sourceData != null)
			{
				this.sourceData.Dispose();
			}

			if (this.importJob != null)
			{
				this.importJob.OnError -= this.ImportJob_OnError;
				this.importJob.OnFatalException -= this.ImportJob_OnFatalException;
				this.importJob.OnMessage -= this.ImportJob_OnMessage;
				this.importJob.OnComplete -= this.ImportJob_OnComplete;
				this.importJob.OnProgress -= this.ImportJob_OnProgress;
			}

			this.testDirectory.Dispose();
		}

		/// <summary>
		/// Should import the files.
		/// </summary>
		/// <param name="clientId">
		/// The transfer client identifier.
		/// </param>
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("0029b51b-11ae-427a-8e53-4528755e974b", "00000000-0000-0000-0000-000000000000")]
		[IdentifiedTestCase("47ae1c92-8c67-4d6e-ae46-f682a580a8a1", TransferClientConstants.FileShareClientId)]
		[IdentifiedTestCase("0802b898-7591-4758-b3db-6d15d4a8d7ea", TransferClientConstants.HttpClientId)]
		[IdentifiedTestCase("df35f6c6-d1f8-44bd-8360-38e5cf41c6dd", TransferClientConstants.AsperaClientId)]
		public void ShouldImportTheFiles(string clientId)
		{
			const bool ForceWebUpload = false;
			if ((clientId == TransferClientConstants.AsperaClientId && this.testParameters.SkipAsperaModeTests) ||
				(clientId == TransferClientConstants.FileShareClientId && this.testParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{clientId}");
			}

			this.GivenTheAutoGeneratedDatasetToImport(5, false);
			this.GivenTheImportJob();
			this.GivenTheImportBatchSizeSetting(10);
			this.GivenTheForceWebUploadSetting(ForceWebUpload);
			this.GivenTheForceTransferClientIdSetting(new Guid(clientId));
			this.GivenTheMaxJobParallelismSetting(1);
			this.GivenTheIoErrorWaitTimeInSeconds(0);
			this.GivenTheNumberOfRetries(0);
			this.GivenTheDisableNativeLocationValidationSetting(true);
			this.WhenExecutingTheJob();
			this.ThenTheImportJobIsSuccessful();
			this.ThenTheImportMessageCountIsNonZero();
			this.ThenTheImportProgressEventsAreRaised();
		}

		/// <summary>
		/// Should import the files.
		/// </summary>
		/// <param name="clientId">
		/// The transfer client identifier.
		/// </param>
		/// <param name="disableNativeLocationValidation">
		/// Specify whether to disable validation for file not found.
		/// </param>
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("64395709-7afb-42e5-b1b3-836f9aab85da", TransferClientConstants.FileShareClientId, false)]
		[IdentifiedTestCase("b1099cea-daa2-4265-869b-0e8808837721", TransferClientConstants.FileShareClientId, true)]
		[IdentifiedTestCase("2b1c7fcb-9c91-4d6a-8e33-13d79c9a2f5e", TransferClientConstants.HttpClientId, false)]
		[IdentifiedTestCase("301239b3-37cb-4fae-a4fb-d8db26a38319", TransferClientConstants.HttpClientId, true)]
		[IdentifiedTestCase("50556c44-cd3a-4a9d-b849-70e686215ebd", TransferClientConstants.AsperaClientId, false)]
		[IdentifiedTestCase("613a03f9-49e2-45fd-b274-13c715e14ae0", TransferClientConstants.AsperaClientId, true)]
		public void ShouldNotImportTheFiles(string clientId, bool disableNativeLocationValidation)
		{
			const bool ForceWebUpload = false;
			const int AutoGeneratedSourceFiles = 5;
			if ((clientId == TransferClientConstants.AsperaClientId && this.testParameters.SkipAsperaModeTests) ||
				(clientId == TransferClientConstants.FileShareClientId && this.testParameters.SkipDirectModeTests))
			{
				Assert.Ignore(TestStrings.SkipTestMessage, $"{clientId}");
			}

			// Intentionally provide an invalid file before adding valid ones.
			this.GivenTheDatasetPathToImport(@"C:\abcdefghijklmnop\out.txt");
			this.GivenTheAutoGeneratedDatasetToImport(AutoGeneratedSourceFiles, true);
			this.GivenTheImportJob();
			this.GivenTheImportBatchSizeSetting(10);
			this.GivenTheForceWebUploadSetting(ForceWebUpload);
			this.GivenTheForceTransferClientIdSetting(new Guid(clientId));
			this.GivenTheMaxJobParallelismSetting(1);
			this.GivenTheIoErrorWaitTimeInSeconds(0);
			this.GivenTheNumberOfRetries(0);

			// This setting should cause a failure.
			this.GivenTheDisableNativeLocationValidationSetting(disableNativeLocationValidation);
			this.WhenExecutingTheJob();
			this.ThenTheImportJobIsNotSuccessful(disableNativeLocationValidation ? 0 : 1, AutoGeneratedSourceFiles + 1, false);
			this.ThenTheImportProgressEventsCountShouldEqual(AutoGeneratedSourceFiles + 1);
			this.ThenTheImportMessageCountIsNonZero();
		}

		/// <summary>
		/// Given the dataset is auto-generated by the number of specified files.
		/// </summary>
		/// <param name="maxFiles">
		/// The file limit.
		/// </param>
		/// <param name="includeReadOnlyFiles">
		/// Specify whether to include read-only files within the dataset.
		/// </param>
		private void GivenTheAutoGeneratedDatasetToImport(int maxFiles, bool includeReadOnlyFiles)
		{
			for (var i = 0; i < maxFiles; i++)
			{
				RandomHelper.NextTextFile(
					MinTestFileLength,
					MaxTestFileLength,
					this.testDirectory.Directory,
					includeReadOnlyFiles && i % 2 == 0);
			}

			this.GivenTheDatasetPathToImport(this.testDirectory.Directory, "*");
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
		private void GivenTheDatasetPathToImport(string path, string searchPattern)
		{
			var number = 1;
			foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.AllDirectories))
			{
				var controlId = "iapi /tapi tests - " + Guid.NewGuid() + " - " + this.runTime;
				this.sourceData.Rows.Add(controlId + " " + number, file);
			}
		}

		/// <summary>
		/// Given the dataset path to import.
		/// </summary>
		/// <param name="file">
		/// The file to import.
		/// </param>
		private void GivenTheDatasetPathToImport(string file)
		{
			var controlId = "iapi /tapi tests - " + Guid.NewGuid() + " - " + this.runTime;
			this.sourceData.Rows.Add(controlId + " " + 1, file);
		}

		/// <summary>
		/// Given the import batch size setting.
		/// </summary>
		/// <param name="value">
		/// The import batch size.
		/// </param>
		private void GivenTheImportBatchSizeSetting(int value)
		{
			kCura.WinEDDS.Config.ConfigSettings["ImportBatchSize"] = value;
		}

		/// <summary>
		/// Given the force web upload setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		private void GivenTheForceWebUploadSetting(bool value)
		{
			AppSettings.Instance.ForceWebUpload = value;
		}

		/// <summary>
		/// Given the force transfer client identifier setting.
		/// </summary>
		/// <param name="value">
		/// The force transfer client identifier setting.
		/// </param>
		private void GivenTheForceTransferClientIdSetting(Guid value)
		{
			// TODO: This has never been supported?
			kCura.WinEDDS.Config.ConfigSettings["TapiForceClientId"] = value.ToString();
		}

		/// <summary>
		/// Given the max job parallelism setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		private void GivenTheMaxJobParallelismSetting(int value)
		{
			AppSettings.Instance.TapiMaxJobParallelism = value;
		}

		/// <summary>
		/// Given the disable native location validation setting.
		/// </summary>
		/// <param name="value">
		/// The setting value.
		/// </param>
		private void GivenTheDisableNativeLocationValidationSetting(bool value)
		{
			AppSettings.Instance.DisableThrowOnIllegalCharacters = value;
		}

		/// <summary>
		/// Given the time to wait when an I/O error occurs setting.
		/// </summary>
		/// <param name="value">
		/// The wait time setting value.
		/// </param>
		private void GivenTheIoErrorWaitTimeInSeconds(int value)
		{
			AppSettings.Instance.IoErrorWaitTimeInSeconds = value;
		}

		/// <summary>
		/// Givens the total number of retries.
		/// </summary>
		/// <param name="value">
		/// The number of retries value.
		/// </param>
		private void GivenTheNumberOfRetries(int value)
		{
			AppSettings.Instance.IoErrorNumberOfRetries = value;
		}

		/// <summary>
		/// Given the import job.
		/// </summary>
		private void GivenTheImportJob()
		{
			Console.WriteLine(
				"Attempting login to " + this.testParameters.RelativityWebApiUrl + ". Username:"
				+ this.testParameters.RelativityUserName + ", Password:" + this.testParameters.RelativityPassword);
			var iapi = new ImportAPI(
				this.testParameters.RelativityUserName,
				this.testParameters.RelativityPassword,
				this.testParameters.RelativityWebApiUrl.ToString());
			this.importJob = iapi.NewNativeDocumentImportJob();
			this.importJob.Settings.WebServiceURL = this.testParameters.RelativityWebApiUrl.ToString();
			this.importJob.Settings.CaseArtifactId = this.testParameters.WorkspaceId;
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
			this.importJob.SourceData.SourceData = this.sourceData.CreateDataReader();
			this.importJob.OnError += this.ImportJob_OnError;
			this.importJob.OnFatalException += this.ImportJob_OnFatalException;
			this.importJob.OnMessage += this.ImportJob_OnMessage;
			this.importJob.OnComplete += this.ImportJob_OnComplete;
			this.importJob.OnProgress += this.ImportJob_OnProgress;
		}

		/// <summary>
		/// When executing the import job.
		/// </summary>
		private void WhenExecutingTheJob()
		{
			var sw = Stopwatch.StartNew();
			this.importJob.Execute();
			sw.Stop();
			Console.WriteLine("Import API elapsed time: {0}", sw.Elapsed);
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
		/// Then the import job is successful.
		/// </summary>
		private void ThenTheImportJobIsSuccessful()
		{
			Assert.That(this.errorRows.Count, Is.EqualTo(0));
			Assert.That(this.jobFatalExceptions.Count, Is.EqualTo(0));
			Assert.That(this.completedJobReport, Is.Not.Null);
			Assert.That(this.completedJobReport.ErrorRows.Count, Is.EqualTo(0));
			Assert.That(this.completedJobReport.FatalException, Is.Null);
			Assert.That(this.completedJobReport.TotalRows, Is.EqualTo(this.sourceData.Rows.Count));
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
		private void ThenTheImportJobIsNotSuccessful(int expectedErrorRows, int expectedTotalRows, bool fatalExceptions)
		{
			Assert.That(this.errorRows.Count, Is.EqualTo(expectedErrorRows));
			if (fatalExceptions)
			{
				Assert.That(this.jobFatalExceptions.Count, Is.GreaterThan(0));
				Assert.That(this.completedJobReport.FatalException, Is.Not.Null);
			}
			else
			{
				Assert.That(this.jobFatalExceptions.Count, Is.EqualTo(0));
				Assert.That(this.completedJobReport.FatalException, Is.Null);
			}

			Assert.That(this.completedJobReport, Is.Not.Null);
			Assert.That(this.completedJobReport.ErrorRows.Count, Is.EqualTo(expectedErrorRows));
			Assert.That(this.completedJobReport.TotalRows, Is.EqualTo(expectedTotalRows));
		}

		/// <summary>
		/// Then the import progress events are raised.
		/// </summary>
		private void ThenTheImportProgressEventsAreRaised()
		{
			this.ThenTheImportProgressEventsCountShouldEqual(this.sourceData.Rows.Count);
		}

		/// <summary>
		/// Then the import progress events count should equal the specified value.
		/// </summary>
		/// <param name="expected">
		/// The expected count.
		/// </param>
		private void ThenTheImportProgressEventsCountShouldEqual(int expected)
		{
			Assert.That(this.progressCompletedRows.Count, Is.EqualTo(expected));
		}

		/// <summary>
		/// Then the import message count is non zero.
		/// </summary>
		private void ThenTheImportMessageCountIsNonZero()
		{
			Assert.That(this.jobMessages.Count, Is.GreaterThan(0));
		}
	}
}