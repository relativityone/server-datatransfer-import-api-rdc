// ----------------------------------------------------------------------------
// <copyright file="TapiBridgeTests.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi.NUnit.Integration
{
    using System;
    using System.Net;
    using System.Threading;

    using Moq;

    using global::NUnit.Framework;

    using global::Relativity.Transfer;
    using global::Relativity.Transfer.UnitTestFramework;

    /// <summary>
    /// The tests for the <see cref="TapiBridge"/> class.
    /// </summary>
    [TestFixture]
    public class TapiBridgeTests
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
        /// The thread synchronization root backing.
        /// </summary>
        private static object SyncRoot = new object();

        /// <summary>
        /// The source path.
        /// </summary>
        private readonly global::System.Collections.Generic.List<string> sourcePaths = new global::System.Collections.Generic.List<string>();

        /// <summary>
        /// The test directory backing.
        /// </summary>
        private TempDirectory testDirectory;

        /// <summary>
        /// The native file transfer class.
        /// </summary>
        private TapiBridge nativeFileTransfer;

        /// <summary>
        /// The target path.
        /// </summary>
        private string targetPath;

        /// <summary>
        /// The cookie container backing.
        /// </summary>
        private CookieContainer cookieContainer;

        /// <summary>
        /// The maximum number of files per folder backing.
        /// </summary>
        private int maxFilesPerFolder;

        /// <summary>
        /// The test instance.
        /// </summary>
        private TestInstance instance;

        /// <summary>
        /// The Relativity relativityHost backing.
        /// </summary>
        private string relativityHost;

        /// <summary>
        /// The relativity connection info backing.
        /// </summary>
        private NetworkCredential credential;

        /// <summary>
        /// The workspace unique identifier backing.
        /// </summary>
        private int workspaceId;

        /// <summary>
        /// The mock repository backing.
        /// </summary>
        private MockRepository mockRepository;

        /// <summary>
        /// The transfer log backing.
        /// </summary>
        private Mock<ITransferLog> transferLog;

        /// <summary>
        /// The file count.
        /// </summary>
        private int fileCount;

        /// <summary>
        /// The number of files transferred.
        /// </summary>
        private int filesTransferred;

        /// <summary>
        /// The total number of warnings.
        /// </summary>
        private int warnings;

        /// <summary>
        /// The total number of fatal errors
        /// </summary>
        private int fatalErrors;

        /// <summary>
        /// The test setup.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            ServicePointManager.SecurityProtocol =
                SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11
                | SecurityProtocolType.Tls12;
            this.sourcePaths.Clear();
            this.maxFilesPerFolder = 1000;
            this.targetPath = null;
            this.instance = null;
            this.credential = null;
            this.relativityHost = null;
            this.nativeFileTransfer = null;
            this.mockRepository = new MockRepository(MockBehavior.Default) { DefaultValue = DefaultValue.Mock };
            this.transferLog = this.mockRepository.Create<ITransferLog>();
            this.cookieContainer = new CookieContainer();
            this.workspaceId = 0;
            this.fileCount = 0;
            this.filesTransferred = 0;
            this.fatalErrors = 0;
            this.warnings = 0;
            this.testDirectory = new TempDirectory { ClearReadOnlyAttributes = true };            
            this.testDirectory.Create();
        }

        /// <summary>
        /// The test tear down.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            this.nativeFileTransfer?.Dispose();
            this.testDirectory?.Dispose();
        }

        /// <summary>
        /// Tests an upload.
        /// </summary>
        [Test]
        public void ShouldUploadTheFiles()
        {
            this.GivenTheTestInstance(TestEnvironment.OnPremisePrivateCloud);
            this.GivenTheWorkspaceId(true);
            this.GivenTheMaxFilesPerFolder(10);
            this.GivenTheNumberOfFiles(100);
            this.GivenTheAutoGeneratedDataset(this.testDirectory.Directory);
            this.GivenTheTargetPath(this.instance.FileShareUncPath + "/NativeFileUploadTest");
            this.GivenTheNativeFileTransfer(TransferDirection.Upload);
            this.WhenExecutingTheJob();
            this.ThenTheFilesWereTransferred();
        }

        /// <summary>
        /// Tests an upload.
        /// </summary>
        [Test]
        public void ShouldDownloadTheFiles()
        {
            this.GivenTheTestInstance(TestEnvironment.OnPremisePrivateCloud);
            this.GivenTheWorkspaceId(true);
            this.GivenTheMaxFilesPerFolder(10);
            this.GivenTheNumberOfFiles(100);
            this.GivenTheAutoGeneratedDataset(this.instance.FileShareUncPath + "/NativeFileDownloadTest/" + Guid.NewGuid());
            this.GivenTheTargetPath(this.testDirectory.Directory);
            this.GivenTheNativeFileTransfer(TransferDirection.Download);
            this.WhenExecutingTheJob();
            this.ThenTheFilesWereTransferred();
        }

        /// <summary>
        /// Given the test instance.
        /// </summary>
        /// <param name="environment">
        /// The test environment.
        /// </param>
        private void GivenTheTestInstance(TestEnvironment environment)
        {
            this.instance = TestInstanceHelper.GetTestInstance(environment);
            this.credential = new NetworkCredential(this.instance.RelativityUsername, this.instance.RelativityPassword);
            this.relativityHost = this.instance.RelativityUrl.ToString();
        }

        /// <summary>
        /// Given the workspace id.
        /// </summary>
        /// <param name="useAsperaWorkspace">
        /// Should use the workspace configured with Aspera.
        /// </param>
        private void GivenTheWorkspaceId(bool useAsperaWorkspace)
        {
            this.workspaceId = useAsperaWorkspace
                ? this.instance.WorkspaceIdWithAspera
                : this.instance.WorkspaceIdWithoutAspera;
        }

        /// <summary>
        /// Given the maximum number of files per folder.
        /// </summary>
        /// <param name="value">
        /// The max value.
        /// </param>
        private void GivenTheMaxFilesPerFolder(int value)
        {
            this.maxFilesPerFolder = value;
        }

        /// <summary>
        /// The given the number of files to generate.
        /// </summary>
        /// <param name="value">
        /// The number of files.
        /// </param>
        private void GivenTheNumberOfFiles(int value)
        {
            this.fileCount = value;
        }

        /// <summary>
        /// Given the dataset is auto-generated by the number of specified files.
        /// </summary>
        /// <param name="directory">
        /// The directory.
        /// </param>
        private void GivenTheAutoGeneratedDataset(string directory)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            
            for (var i = 0; i < this.fileCount; i++)
            {
                var file = RandomHelper.NextTextFile(
                    MinTestFileLength,
                    MaxTestFileLength,
                    directory,
                    false);
                this.sourcePaths.Add(file);
            }
        }

        /// <summary>
        /// Given the target path.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        private void GivenTheTargetPath(string value)
        {
            this.targetPath = value;
        }

        /// <summary>
        /// Given the <see cref="TapiBridge"/> class.
        /// </summary>
        private void GivenTheNativeFileTransfer(TransferDirection direction)
        {
            var parameters = new TapiBridgeParameters
            {
                WebCookieContainer = this.cookieContainer,
                Credentials = this.credential,
                MaxFilesPerFolder = this.maxFilesPerFolder,
                TargetPath = this.targetPath,
                WebServiceUrl = this.relativityHost,
                WorkspaceId = this.workspaceId
            };

            this.nativeFileTransfer = new TapiBridge(parameters, direction, this.transferLog?.Object, CancellationToken.None);
            this.nativeFileTransfer.TapiFatalError += (sender, args) =>
            {
                lock (SyncRoot)
                {
                    this.fatalErrors++;
                }
            };

            this.nativeFileTransfer.TapiWarningMessage += (sender, args) =>
            {
                lock (SyncRoot)
                {
                    this.warnings++;
                }
            };

            this.nativeFileTransfer.TapiProgress += (sender, args) =>
            {
                lock (SyncRoot)
                {
                    this.filesTransferred++;
                }
            };
        }
        
        /// <summary>
        /// When executing the job.
        /// </summary>
        private void WhenExecutingTheJob()
        {
            var order = 0;
            foreach (var sourcePath in this.sourcePaths)
            {
                this.nativeFileTransfer.AddPath(sourcePath, null, order++);
            }

            this.nativeFileTransfer.WaitForTransferJob();
        }

        /// <summary>
        /// Then all the files were transferred.
        /// </summary>
        private void ThenTheFilesWereTransferred()
        {
            Console.WriteLine($"Files Transferred: {this.filesTransferred}");
            Assert.That(this.fatalErrors, Is.EqualTo(0));
            Assert.That(this.warnings, Is.EqualTo(0));
            Assert.That(this.filesTransferred, Is.EqualTo(this.fileCount));
        }
    }
}