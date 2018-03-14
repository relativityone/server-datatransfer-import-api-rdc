using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	[TestFixture]
	public class PhysicalFilesDownloaderTests
	{
		private ExportTapyBridgeFactoryStub _exportTapiBridgeFactoryStub;
		private FileshareSettingsServiceStub _fileshareSettingsServiceStub;
		private ILog _logger;
		private Mock<IDownloadTapiBridge> _mockTapiBridge;
		private Mock<IExportConfig> _mockExportConfig;
		private SafeIncrement _safeIncrement;
		private string[] _availableFileshares;
		private const int _DEFAULT_FILES_PER_FILESHARE = 3;
		private const int _DEFAULT_TASK_COUNT = 2;

		[SetUp]
		public void SetUp()
		{
			_logger = new NullLogger();
			_safeIncrement = new SafeIncrement();
			_availableFileshares = new[] { @"\\fileshare.one", @"\\fileshare.two", @"\\fileshare.three" };
			_fileshareSettingsServiceStub = new FileshareSettingsServiceStub(_availableFileshares);
			_exportTapiBridgeFactoryStub = new ExportTapyBridgeFactoryStub();
			_mockTapiBridge = new Mock<IDownloadTapiBridge>();
			_mockExportConfig = new Mock<IExportConfig>();
			_mockExportConfig.SetupGet(config => config.MaxNumberOfFileExportTasks).Returns(_DEFAULT_TASK_COUNT);
		}

		[Test]
		public async Task ItShouldCreateTapiBridgesAccordingly()
		{
			Mock<IExportTapiBridgeFactory> mockExportTapiBridgeFactory = new Mock<IExportTapiBridgeFactory>();
			mockExportTapiBridgeFactory.Setup(f => f.CreateForFiles(It.IsAny<RelativityFileShareSettings>(), It.IsAny<CancellationToken>()))
				.Returns(_mockTapiBridge.Object);
			List<ExportRequest> requests = CreateThreeExportRequestsPerFileshares(_availableFileshares).ToList();

			var downloader = new PhysicalFilesDownloader(_fileshareSettingsServiceStub, mockExportTapiBridgeFactory.Object, _mockExportConfig.Object, _safeIncrement, _logger);

			await downloader.DownloadFilesAsync(requests, CancellationToken.None);

			mockExportTapiBridgeFactory.Verify(f => f.CreateForFiles(It.IsAny<RelativityFileShareSettings>(), It.IsAny<CancellationToken>()), Times.Exactly(_availableFileshares.Length));
		}

		[Test]
		public async Task ItShouldManageTapiBridgeLifecycleCorrectly()
		{
			List<ExportRequest> requests = CreateThreeExportRequestsPerFileshares(_availableFileshares).ToList();

			var downloader = new PhysicalFilesDownloader(_fileshareSettingsServiceStub, _exportTapiBridgeFactoryStub, _mockExportConfig.Object, _safeIncrement, _logger);

			await downloader.DownloadFilesAsync(requests, CancellationToken.None);

			_exportTapiBridgeFactoryStub.VerifyBridges(m => m.Verify(b => b.QueueDownload(It.IsAny<TransferPath>()), Times.Exactly(_DEFAULT_FILES_PER_FILESHARE)));
			_exportTapiBridgeFactoryStub.VerifyBridges(m => m.Verify(b => b.WaitForTransferJob(), Times.Once));
			_exportTapiBridgeFactoryStub.VerifyBridges(m => m.Verify(b => b.Dispose(), Times.Once));
		}

		[Test]
		public void ItShouldThrowTaskCanceledExceptionWhenDownloaderThrowsOne()
		{
			_mockTapiBridge.Setup(b => b.WaitForTransferJob()).Throws<TaskCanceledException>();

			Mock<IExportTapiBridgeFactory> mockExportTapiBridgeFactory = new Mock<IExportTapiBridgeFactory>();
			mockExportTapiBridgeFactory.Setup(f => f.CreateForFiles(It.IsAny<RelativityFileShareSettings>(), It.IsAny<CancellationToken>()))
				.Returns(_mockTapiBridge.Object);


			List<ExportRequest> requests = CreateThreeExportRequestsPerFileshares(_availableFileshares).ToList();

			var downloader = new PhysicalFilesDownloader(_fileshareSettingsServiceStub, mockExportTapiBridgeFactory.Object, _mockExportConfig.Object, _safeIncrement, _logger);

			Assert.ThrowsAsync<TaskCanceledException>(async () => await downloader.DownloadFilesAsync(requests, CancellationToken.None));
		}

		private IEnumerable<ExportRequest> CreateThreeExportRequestsPerFileshares(IEnumerable<string> fileshares)
		{
			return fileshares.SelectMany(f => Enumerable.Repeat(f, _DEFAULT_FILES_PER_FILESHARE)).Select(CreatePhysicalFileExportRequest);
		}

		private PhysicalFileExportRequest CreatePhysicalFileExportRequest(string fileshareAddress)
		{
			return new PhysicalFileExportRequest(
				new ObjectExportInfo
				{
					NativeSourceLocation = new UriBuilder(fileshareAddress) {Path = Guid.NewGuid().ToString()}.ToString()
				}, "whatever.txt");
		}
	}
}
