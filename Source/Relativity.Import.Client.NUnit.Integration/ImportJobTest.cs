// ----------------------------------------------------------------------------
// <copyright file="ImportJobTest.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi.NUnit
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Text;

    using kCura.Relativity.DataReaderClient;
    using kCura.Relativity.ImportAPI;

    using global::NUnit.Framework;

    using global::Relativity.Transfer;
    using global::Relativity.Transfer.UnitTestFramework;

    using DataTable = System.Data.DataTable;

    /// <summary>
    /// Tests an import job.
    /// </summary>
    [TestFixture]
    public class ImportJobTest
    {
        /// <summary>
        /// The minimum test file length [1KB]
        /// </summary>
        private const int MinTestFileLength = 1024;

        /// <summary>
        /// The maximum test file length [10KB]
        /// </summary>
        private const int MaxTestFileLength = 10 * MinTestFileLength;

        /// <summary>
        /// The thread synchronization backing.
        /// </summary>
        private static object SyncRoot = new object();

        /// <summary>
        /// The test directory backing.
        /// </summary>
        private TempDirectory testDirectory;

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
        /// The source data.
        /// </summary>
        private DataTable sourceData;

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
        /// The test instance.
        /// </summary>
        private TestInstance testInstance;

        /// <summary>
        /// The relativity import url.
        /// </summary>
        private string relativityImportUrl;

        /// <summary>
        /// The workspace id.
        /// </summary>
        private int workspaceId;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;

            this.testDirectory = new TempDirectory { ClearReadOnlyAttributes = true };
            this.testDirectory.Create();
            this.sourceData = new DataTable();
            this.sourceData.Columns.Add("Control Number", typeof(string));
            this.sourceData.Columns.Add("File Path", typeof(string));
            this.jobMessages.Clear();
            this.jobFatalExceptions.Clear();
            this.errorRows.Clear();
            this.progressCompletedRows.Clear();
            this.testInstance = null;
            this.relativityImportUrl = null;
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
            }

            this.testDirectory.Dispose();
        }

        /// <summary>
        /// Should import the files.
        /// </summary>
        /// <param name="clientId">
        /// The transfer client identifier.
        /// </param>
        /// <param name="workspaceWithAspera">
        /// Specify whether to use a workspace with Aspera configured.
        /// </param>
        [Test]
        [TestCase("00000000-0000-0000-0000-000000000000", false)]
        [TestCase(TransferClientConstants.FileShareClientId, false)]
        [TestCase(TransferClientConstants.HttpClientId, false)]
        [TestCase(TransferClientConstants.AsperaClientId, true)]
        public void ShouldImportTheFiles(string clientId, bool workspaceWithAspera)
        {
            const bool ForceWebUpload = false;
            this.GivenTheAutoGeneratedDatasetToImport(5, true);
            //// this.GivenTheDatasetPathToImport(@"C:\Datasets\PerfDataSet_60GB", "*", SearchOption.AllDirectories);
            //// this.GivenTheDatasetPathToImport(@"C:\Datasets\3GB PDFs", "*", SearchOption.AllDirectories);
            this.GivenTheTestInstance(TestEnvironment.OnPremisePrivateCloud);
            this.GivenTheWorkspaceId(
                workspaceWithAspera
                    ? this.testInstance.WorkspaceIdWithAspera
                    : this.testInstance.WorkspaceIdWithoutAspera);
            this.GivenTheImportJob();
            this.GivenTheImportBatchSizeSetting(10);
            this.GivenTheForceWebUploadSetting(ForceWebUpload);
            this.GivenTheForceTransferClientIdSetting(new Guid(clientId));
            this.GivenTheMaxSingleFileRetryAttemptsSetting(1);
            this.GivenTheMaxJobRetryAttemptsSetting(1);
            this.GivenTheMaxJobParallelismSetting(1);
            this.GivenTheLogEnabledSetting(true);
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
        /// <param name="workspaceWithAspera">
        /// Specify whether to use a workspace with Aspera configured.
        /// </param>
        /// <param name="disableNativeLocationValidation">
        /// Specify whether to disable validation for file not found.
        /// </param>
        [Test]
        [TestCase(TransferClientConstants.FileShareClientId, false, false)]
        [TestCase(TransferClientConstants.FileShareClientId, false, true)]
        [TestCase(TransferClientConstants.HttpClientId, false, false)]
        [TestCase(TransferClientConstants.HttpClientId, false, true)]
        [TestCase(TransferClientConstants.AsperaClientId, true, false)]
        [TestCase(TransferClientConstants.AsperaClientId, true, true)]
        public void ShouldNotImportTheFiles(string clientId, bool workspaceWithAspera, bool disableNativeLocationValidation)
        {
            const bool ForceWebUpload = false;
            const int AutoGeneratedSourceFiles = 5;

            // Intentionally provide an invalid file before adding valid ones.
            this.GivenTheDatasetPathToImport(@"C:\abcdefghijklmnop\out.txt");
            this.GivenTheAutoGeneratedDatasetToImport(AutoGeneratedSourceFiles, true);
            this.GivenTheTestInstance(TestEnvironment.OnPremisePrivateCloud);
            this.GivenTheWorkspaceId(
                workspaceWithAspera
                    ? this.testInstance.WorkspaceIdWithAspera
                    : this.testInstance.WorkspaceIdWithoutAspera);
            this.GivenTheImportJob();
            this.GivenTheImportBatchSizeSetting(10);
            this.GivenTheForceWebUploadSetting(ForceWebUpload);
            this.GivenTheForceTransferClientIdSetting(new Guid(clientId));
            this.GivenTheMaxSingleFileRetryAttemptsSetting(1);
            this.GivenTheMaxJobRetryAttemptsSetting(1);
            this.GivenTheMaxJobParallelismSetting(1);
            this.GivenTheLogEnabledSetting(true);
            this.GivenTheIoErrorWaitTimeInSeconds(0);
            this.GivenTheNumberOfRetries(0);

            // This setting should cause a failure.
            this.GivenTheDisableNativeLocationValidationSetting(disableNativeLocationValidation);
            this.WhenExecutingTheJob();

            if (disableNativeLocationValidation)
            {
                // When fatal, we won't get any errors.
                this.ThenTheImportJobIsNotSuccessful(0, 1, true);
                this.ThenTheImportProgressEventsCountShouldEqual(0);
            }
            else
            {
                this.ThenTheImportJobIsNotSuccessful(1, 6, false);
                this.ThenTheImportProgressEventsCountShouldEqual(AutoGeneratedSourceFiles);
            }

            this.ThenTheImportMessageCountIsNonZero();
        }

        /// <summary>
        /// Given the test instance.
        /// </summary>
        /// <param name="testEnvironment">
        /// The test environment.
        /// </param>
        private void GivenTheTestInstance(TestEnvironment testEnvironment)
        {
            this.testInstance = TestInstanceHelper.GetTestInstance(testEnvironment);
            this.relativityImportUrl = string.Concat(this.testInstance.RelativityUrl, "RelativityWebApi");
        }

        /// <summary>
        /// Given the workspace id.
        /// </summary>
        /// <param name="workspaceId">
        /// The workspace id.
        /// </param>
        private void GivenTheWorkspaceId(int workspaceId)
        {
            this.workspaceId = workspaceId;
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
                RandomHelper.NextFile(
                    MinTestFileLength,
                    MaxTestFileLength,
                    this.testDirectory.Directory,
                    includeReadOnlyFiles && i % 2 == 0);
            }

            this.GivenTheDatasetPathToImport(this.testDirectory.Directory, "*", SearchOption.AllDirectories);
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
        private void GivenTheDatasetPathToImport(string path, string searchPattern, SearchOption searchOption)
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
            Config.ConfigSettings["ImportBatchSize"] = value;
        }

        /// <summary>
        /// Given the force web upload setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        private void GivenTheForceWebUploadSetting(bool value)
        {
            Config.ConfigSettings["ForceWebUpload"] = value;
        }

        /// <summary>
        /// Given the force transfer client identifier setting.
        /// </summary>
        /// <param name="value">
        /// The force transfer client identifier setting.
        /// </param>
        private void GivenTheForceTransferClientIdSetting(Guid value)
        {
            Config.ConfigSettings["TapiForceClientId"] = value.ToString();
        }

        /// <summary>
        /// Given the max job parallelism setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        private void GivenTheMaxJobParallelismSetting(int value)
        {
            Config.ConfigSettings["TapiMaxJobParallelism"] = value;
        }

        /// <summary>
        /// Given the max single file retry attempts setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        private void GivenTheMaxSingleFileRetryAttemptsSetting(int value)
        {
            Config.ConfigSettings["TapiMaxSingleFileRetryAttempts"] = value;
        }

        /// <summary>
        /// Given the log enabled setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        private void GivenTheLogEnabledSetting(bool value)
        {
            Config.ConfigSettings["TapiLogEnabled"] = value;
        }

        /// <summary>
        /// Given the max job retry attempts setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        private void GivenTheMaxJobRetryAttemptsSetting(int value)
        {
            Config.ConfigSettings["TapiMaxJobRetryAttempts"] = value;
        }

        /// <summary>
        /// Given the disable native location validation setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        private void GivenTheDisableNativeLocationValidationSetting(bool value)
        {
            Config.ConfigSettings["DisableNativeLocationValidation"] = value;
        }

        /// <summary>
        /// Given the time to wait when an I/O error occurs setting.
        /// </summary>
        /// <param name="value">
        /// The wait time setting value.
        /// </param>
        private void GivenTheIoErrorWaitTimeInSeconds(int value)
        {
            Config.ConfigSettings["IOErrorWaitTimeInSeconds"] = value;
            kCura.Utility.Config.ConfigSettings["IOErrorWaitTimeInSeconds"] = value;
        }

        /// <summary>
        /// Givens the total number of retries.
        /// </summary>
        /// <param name="value">
        /// The number of retries value.
        /// </param>
        private void GivenTheNumberOfRetries(int value)
        {
            kCura.Utility.Config.ConfigSettings["IOErrorNumberOfRetries"] = value;
        }

        /// <summary>
        /// Given the import job.
        /// </summary>
        private void GivenTheImportJob()
        {
            Console.WriteLine("Attempting login to " + this.relativityImportUrl + ". Username:" + this.testInstance.RelativityUsername +
                              ", Password:" + this.testInstance.RelativityPassword);
            var iapi = new ImportAPI(this.testInstance.RelativityUsername, this.testInstance.RelativityPassword, this.relativityImportUrl);
            this.importJob = iapi.NewNativeDocumentImportJob();
            this.importJob.Settings.WebServiceURL = this.relativityImportUrl;
            this.importJob.Settings.CaseArtifactId = this.workspaceId;
            this.importJob.Settings.ArtifactTypeId = 10;
            this.importJob.Settings.ExtractedTextFieldContainsFilePath = false;
            this.importJob.Settings.NativeFilePathSourceFieldName = "File Path";
            this.importJob.Settings.SelectedIdentifierFieldName = "Control Number";
            this.importJob.Settings.NativeFileCopyMode = NativeFileCopyModeEnum.CopyFiles;
            this.importJob.Settings.OverwriteMode = OverwriteModeEnum.Append;
            this.importJob.Settings.OIFileIdMapped = true;
            this.importJob.Settings.OIFileIdColumnName = "OutsideInFileId";
            this.importJob.Settings.OIFileTypeColumnName = "OutsideInFileType";
            this.importJob.Settings.ExtractedTextEncoding = Encoding.Unicode;
            this.importJob.Settings.FileSizeMapped = true;
            this.importJob.Settings.FileSizeColumn = "NativeFileSize";
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
                Console.WriteLine(status.Message);
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
                Console.WriteLine(jobReport.FatalException.ToString());
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