// ----------------------------------------------------------------------------
// <copyright file="ImportJobTestBase.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security.AccessControl;
    using System.Security.Principal;
    using System.Text;

    using kCura.Relativity.DataReaderClient;
    using kCura.Relativity.ImportAPI;

    using global::NUnit.Framework;
    using global::Relativity.Transfer;
    using global::Relativity.Transfer.UnitTestFramework;

    /// <summary>
    /// Represents an abstract load-file base class.
    /// </summary>
    public abstract class ImportJobTestBase
    {
        /// <summary>
        /// The minimum test file length [1KB]
        /// </summary>
        internal const int MinTestFileLength = 1024;

        /// <summary>
        /// The maximum test file length [10KB]
        /// </summary>
        internal const int MaxTestFileLength = 10 * MinTestFileLength;

        /// <summary>
        /// The thread synchronization backing.
        /// </summary>
        private static readonly object SyncRoot = new object();

        /// <summary>
        /// The job messages.
        /// </summary>
        private readonly global::System.Collections.Generic.List<string> jobMessages = new global::System.Collections.Generic.List<string>();

        /// <summary>
        /// The job fatal exceptions.
        /// </summary>
        private readonly global::System.Collections.Generic.List<Exception> jobFatalExceptions = new global::System.Collections.Generic.List<Exception>();

        /// <summary>
        /// The error rows.
        /// </summary>
        private readonly global::System.Collections.Generic.List<IDictionary> errorRows = new global::System.Collections.Generic.List<IDictionary>();

        /// <summary>
        /// The progress completed rows.
        /// </summary>
        private readonly global::System.Collections.Generic.List<long> progressCompletedRows = new global::System.Collections.Generic.List<long>();

        /// <summary>
        /// The import job.
        /// </summary>
        private ImportBulkArtifactJob importJob;

        /// <summary>
        /// The completed job report.
        /// </summary>
        private JobReport completedJobReport;

        /// <summary>
        /// The workspace id.
        /// </summary>
        private int workspaceId;

        /// <summary>
        /// The Relativity Logging logger used to capture logs for this integration test.
        /// </summary>
        private TestRelativityLog logger;

        /// <summary>
        /// Gets or sets the relativity import URL.
        /// </summary>
        /// <value>
        /// The relativity import URL.
        /// </value>
        protected string RelativityImportUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets source data.
        /// </summary>
        /// <value>
        /// The <see cref="DataTable"/> instance.
        /// </value>
        protected DataTable SourceData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the temp directory.
        /// </summary>
        /// <value>
        /// The temp directory.
        /// </value>
        protected TempDirectory TempDirectory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the test instance.
        /// </summary>
        /// <value>
        /// The test instance.
        /// </value>
        protected TestInstance TestInstance
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets timestamp.
        /// </summary>
        /// <value>
        /// The <see cref="DateTime"/> instance.
        /// </value>
        protected DateTime Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            this.logger = TestRelativityLogFactory.Create();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;
            this.Timestamp = DateTime.Now;
            this.SourceData = new DataTable();
            this.SourceData.Columns.Add("Control Number", typeof(string));
            this.SourceData.Columns.Add("File Path", typeof(string));
            this.jobMessages.Clear();
            this.jobFatalExceptions.Clear();
            this.errorRows.Clear();
            this.progressCompletedRows.Clear();
            this.TestInstance = null;
            this.RelativityImportUrl = null;
            this.importJob = null;
            this.completedJobReport = null;
            this.workspaceId = 0;
            this.TempDirectory = new TempDirectory();
            this.TempDirectory.Create();
            this.OnSetup();
        }

        /// <summary>
        /// The test tear down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.OnPreTearDown();
            this.SourceData?.Dispose();
            if (this.importJob != null)
            {
                this.importJob.OnError -= this.ImportJob_OnError;
                this.importJob.OnFatalException -= this.ImportJob_OnFatalException;
                this.importJob.OnMessage -= this.ImportJob_OnMessage;
                this.importJob.OnComplete -= this.ImportJob_OnComplete;
                this.importJob.OnProgress -= this.ImportJob_OnProgress;
            }

            this.TempDirectory?.Dispose();
            this.logger?.Dispose();
            this.OnTearDown();
        }

        /// <summary>
        /// Changes the file full permissions.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="grant">
        /// Specify whether to grant or deny access.
        /// </param>
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

        /// <summary>
        /// Called when the test setup occurs.
        /// </summary>
        protected virtual void OnSetup()
        {
        }

        /// <summary>
        /// Called before the test tear down occurs.
        /// </summary>
        protected virtual void OnPreTearDown()
        {
        }

        /// <summary>
        /// Called when the test tear down occurs.
        /// </summary>
        protected virtual void OnTearDown()
        {
        }

        /// <summary>
        /// Given the test instance.
        /// </summary>
        /// <param name="testEnvironment">
        /// The test environment.
        /// </param>
        protected void GivenTheTestInstance(TestEnvironment testEnvironment)
        {
            this.TestInstance = TestInstanceHelper.GetTestInstance(testEnvironment);
            this.RelativityImportUrl = string.Concat(this.TestInstance.RelativityUrl, "RelativityWebApi");
        }

        /// <summary>
        /// Given the workspace id.
        /// </summary>
        /// <param name="value">
        /// The workspace id.
        /// </param>
        protected void GivenTheWorkspaceId(int value)
        {
            this.workspaceId = value;
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
        protected void GivenTheSourceFilesAreLocked()
        {
            for (var i = 0; i < this.SourceData.Rows.Count; i++)
            {
                this.GivenTheSourceFileIsLocked(i);
            }
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
        /// Givens the standard configuration settings.
        /// </summary>
        /// <param name="forceClient">
        /// Sppecify whichi client to force.
        /// </param>
        /// <param name="disableNativeLocationValidation">
        /// Specify whether to disable native location validation.
        /// </param>
        /// <param name="disableNativeValidation">
        /// Specify whether to disable native validation.
        /// </param>
        protected void GivenTheStandardConfigSettings(
            TapiClient forceClient,
            bool disableNativeLocationValidation,
            bool disableNativeValidation)
        {
            this.GivenTheForceWebUploadSetting(false);
            this.GivenTheTapiForceAsperaClientSetting(forceClient == TapiClient.Aspera);
            this.GivenTheTapiForceFileShareClientSetting(forceClient == TapiClient.Direct);
            this.GivenTheTapiForceHttpClientSetting(forceClient == TapiClient.Web);
            this.GivenTheMaxJobRetryAttemptsSetting(1);
            this.GivenTheMaxJobParallelismSetting(1);
            this.GivenTheLogEnabledSetting(true);
            this.GivenTheIoErrorWaitTimeInSeconds(0);
            this.GivenTheUsePipeliningForFileIdAndCopySetting(false);
            this.GivenTheDisableNativeLocationValidationSetting(disableNativeLocationValidation);
            this.GivenTheDisableNativeValidationSetting(disableNativeValidation);

            // Note: there's no longer a BCP sub-folder.
            this.GivenTheAsperaBcpRootFolder(string.Empty);
            this.GivenTheAsperaNativeDocRootLevels(1);
        }

        /// <summary>
        /// Given the import batch size setting.
        /// </summary>
        /// <param name="value">
        /// The import batch size.
        /// </param>
        protected void GivenTheImportBatchSizeSetting(int value)
        {

            Config.ConfigSettings["ImportBatchSize"] = value;
        }

        /// <summary>
        /// Given the force web upload setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheForceWebUploadSetting(bool value)
        {
            Config.ConfigSettings["ForceWebUpload"] = value;
        }

        /// <summary>
        /// Given the force Aspera client setting.
        /// </summary>
        /// <param name="value">
        /// The force setting.
        /// </param>
        protected void GivenTheTapiForceAsperaClientSetting(bool value)
        {
            Config.ConfigSettings["TapiForceAsperaClient"] = value.ToString();
        }

        /// <summary>
        /// Given the force file share client setting.
        /// </summary>
        /// <param name="value">
        /// The force setting.
        /// </param>
        protected void GivenTheTapiForceFileShareClientSetting(bool value)
        {
            Config.ConfigSettings["TapiForceFileShareClient"] = value.ToString();
        }

        /// <summary>
        /// Given the force HTTP client setting.
        /// </summary>
        /// <param name="value">
        /// The force setting.
        /// </param>
        protected void GivenTheTapiForceHttpClientSetting(bool value)
        {
            Config.ConfigSettings["TapiForceHttpClient"] = value.ToString();
        }

        /// <summary>
        /// Given the max job parallelism setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheMaxJobParallelismSetting(int value)
        {
            Config.ConfigSettings["TapiMaxJobParallelism"] = value;
        }

        /// <summary>
        /// Given the log enabled setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheLogEnabledSetting(bool value)
        {
            Config.ConfigSettings["TapiLogEnabled"] = value;
        }

        /// <summary>
        /// Given the Aspera native files doc-root levels.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheAsperaNativeDocRootLevels(int value)
        {
            Config.ConfigSettings["TapiAsperaNativeDocRootLevels"] = value;
        }

        /// <summary>
        /// Given the Aspera BCP root folder.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheAsperaBcpRootFolder(string value)
        {
            Config.ConfigSettings["TapiAsperaBcpRootFolder"] = value;
        }

        /// <summary>
        /// Given the max job retry attempts setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheMaxJobRetryAttemptsSetting(int value)
        {
            Config.ConfigSettings["TapiMaxJobRetryAttempts"] = value;
        }

        /// <summary>
        /// Given the disable native location validation setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheDisableNativeLocationValidationSetting(bool value)
        {
            Config.ConfigSettings["DisableNativeLocationValidation"] = value;
        }

        /// <summary>
        /// Given the disable native validation setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheDisableNativeValidationSetting(bool value)
        {
            Config.ConfigSettings["DisableNativeValidation"] = value;
        }

        /// <summary>
        /// Given the pipelining for file id and copy setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        /// <remarks>
        /// This is the key configuration setting that has created instability with negative test cases.
        /// </remarks>
        protected void GivenTheUsePipeliningForFileIdAndCopySetting(bool value)
        {
            Config.ConfigSettings["UsePipeliningForFileIdAndCopy"] = value;
        }

        /// <summary>
        /// Given the time to wait when an I/O error occurs setting.
        /// </summary>
        /// <param name="value">
        /// The setting value.
        /// </param>
        protected void GivenTheIoErrorWaitTimeInSeconds(int value)
        {
            Config.ConfigSettings["IOErrorWaitTimeInSeconds"] = value;
            kCura.Utility.Config.ConfigSettings["IOErrorWaitTimeInSeconds"] = value;
            if (value == 0)
            {
                kCura.Utility.Config.ConfigSettings["IOErrorNumberOfRetries"] = 0;
            }
        }

        /// <summary>
        /// Given the import job.
        /// </summary>
        protected void GivenTheImportJob()
        {
            var iapi = new ImportAPI(
                this.TestInstance.RelativityUsername,
                this.TestInstance.RelativityPassword,
                this.RelativityImportUrl);
            this.importJob = iapi.NewNativeDocumentImportJob();
            this.importJob.Settings.WebServiceURL = this.RelativityImportUrl;
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
            this.importJob.SourceData.SourceData = this.SourceData.CreateDataReader();
            this.importJob.OnError += this.ImportJob_OnError;
            this.importJob.OnFatalException += this.ImportJob_OnFatalException;
            this.importJob.OnMessage += this.ImportJob_OnMessage;
            this.importJob.OnComplete += this.ImportJob_OnComplete;
            this.importJob.OnProgress += this.ImportJob_OnProgress;
        }

        /// <summary>
        /// When executing the import job.
        /// </summary>
        protected void WhenExecutingTheJob()
        {
            var sw = Stopwatch.StartNew();
            this.importJob.Execute();
            sw.Stop();
            Console.WriteLine("Import API elapsed time: {0}", sw.Elapsed);
        }

        /// <summary>
        /// Then the import job is successful.
        /// </summary>
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

        /// <summary>
        /// Then the import progress events count should be greater than zero.
        /// </summary>
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
    }
}