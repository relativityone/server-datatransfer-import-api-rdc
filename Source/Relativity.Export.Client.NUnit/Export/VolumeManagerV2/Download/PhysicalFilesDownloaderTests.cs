using System;
using System.Collections.Concurrent;
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
		private Dictionary<string, IRelativityFileShareSettings> _fileShareSettingsCache;
		private ILog _logger;
		private List<Mock<IDownloadTapiBridge>> _mockTapiBridges;
		private Mock<IExportConfig> _mockExportConfig;
		private Mock<IFileShareSettingsService> _fileshareSettingsService;
		private Mock<IFileTapiBridgePool> _fileTapiBridgePool;
		private PhysicalFilesDownloader _downloader;
		private SafeIncrement _safeIncrement;
		private string[] _availableFileShares;
		private const int _DEFAULT_FILES_PER_FILESHARE = 3;
		private const int _DEFAULT_TASK_COUNT = 2;
		private readonly object _lockToken = new object();

		[SetUp]
		public void SetUp()
		{
			_fileShareSettingsCache = new Dictionary<string, IRelativityFileShareSettings>();
			_logger = new NullLogger();
			_safeIncrement = new SafeIncrement();
			_availableFileShares = new[] { @"\\fileShare.one", @"\\fileShare.two", @"\\fileShare.three" };
			_fileshareSettingsService = new Mock<IFileShareSettingsService>();
			_fileshareSettingsService.Setup(s => s.GetSettingsForFileshare(It.IsAny<string>())).Returns((string val) => ReturnSettingsForFileshare(val));
			_mockTapiBridges = new List<Mock<IDownloadTapiBridge>>();
			_fileTapiBridgePool = new Mock<IFileTapiBridgePool>();
			_fileTapiBridgePool
				.Setup(pool => pool.Request(It.IsAny<IRelativityFileShareSettings>(), It.IsAny<CancellationToken>()))
				.Returns(ReturnNewMockTapiBridge);
			_mockExportConfig = new Mock<IExportConfig>();
			_mockExportConfig.SetupGet(config => config.MaxNumberOfFileExportTasks).Returns(_DEFAULT_TASK_COUNT);
			_downloader = new PhysicalFilesDownloader(_fileshareSettingsService.Object, _fileTapiBridgePool.Object, _mockExportConfig.Object, _safeIncrement, _logger);
		}

		[Test]
		public void ItShouldNotThrowOnEmptyRequestList()
		{
			List<ExportRequest> emptyRequestList = new List<ExportRequest>();

			Assert.DoesNotThrowAsync(async () => await _downloader.DownloadFilesAsync(emptyRequestList, CancellationToken.None).ConfigureAwait(false));
		}

		[Test]
		public async Task ItShouldCreateTapiBridgesAccordingly()
		{
			List<ExportRequest> requests = CreateThreeExportRequestsPerFileShare(_availableFileShares).ToList();

			await _downloader.DownloadFilesAsync(requests, CancellationToken.None).ConfigureAwait(false);

			_fileshareSettingsService.Verify(svc => svc.GetSettingsForFileshare(It.IsAny<string>()), Times.Exactly(requests.Count));
			_fileTapiBridgePool.Verify(pool => pool.Request(It.IsAny<IRelativityFileShareSettings>(), It.IsAny<CancellationToken>()), Times.Exactly(_availableFileShares.Length));
			_fileTapiBridgePool.Verify(pool => pool.Release(It.IsAny<IDownloadTapiBridge>()), Times.Exactly(_availableFileShares.Length));
		}

		[Test]
		public async Task ItShouldManageTapiBridgeLifecycleCorrectly()
		{
			List<ExportRequest> requests = CreateThreeExportRequestsPerFileShare(_availableFileShares).ToList();

			await _downloader.DownloadFilesAsync(requests, CancellationToken.None).ConfigureAwait(false);

			VerifyBridges(m => m.Verify(b => b.QueueDownload(It.IsAny<TransferPath>()), Times.Exactly(_DEFAULT_FILES_PER_FILESHARE)));
			VerifyBridges(m => m.Verify(b => b.WaitForTransferJob(), Times.Once));
		}

		[Test]
		public void ItShouldThrowTaskCanceledExceptionWhenDownloaderThrowsOne()
		{
			var mockTapiBridge = new Mock<IDownloadTapiBridge>();
			mockTapiBridge.Setup(b => b.WaitForTransferJob()).Throws<TaskCanceledException>();
			_fileTapiBridgePool
				.Setup(pool => pool.Request(It.IsAny<IRelativityFileShareSettings>(), It.IsAny<CancellationToken>()))
				.Returns(mockTapiBridge.Object);
			List<ExportRequest> requests = CreateThreeExportRequestsPerFileShare(_availableFileShares).ToList();

			var downloader = new PhysicalFilesDownloader(_fileshareSettingsService.Object, _fileTapiBridgePool.Object, _mockExportConfig.Object, _safeIncrement, _logger);

			Assert.ThrowsAsync<TaskCanceledException>(async () => await downloader.DownloadFilesAsync(requests, CancellationToken.None).ConfigureAwait(false));
		}

		private IEnumerable<ExportRequest> CreateThreeExportRequestsPerFileShare(IEnumerable<string> fileShares)
		{
			return fileShares.SelectMany(f => Enumerable.Repeat(f, _DEFAULT_FILES_PER_FILESHARE)).Select(CreatePhysicalFileExportRequest);
		}

		private PhysicalFileExportRequest CreatePhysicalFileExportRequest(string fileshareAddress)
		{
			return new PhysicalFileExportRequest(
				new ObjectExportInfo
				{
					NativeSourceLocation = $"{fileshareAddress}\\{Guid.NewGuid()}"
				}, "whatever.txt");
		}

		private IRelativityFileShareSettings ReturnSettingsForFileshare(string fileUrl)
		{
			string fileshare = _availableFileShares.First(fileUrl.Contains);
			if (!_fileShareSettingsCache.ContainsKey(fileshare))
			{
				_fileShareSettingsCache[fileshare] = new Mock<IRelativityFileShareSettings>().Object;
			}

			return _fileShareSettingsCache[fileshare];
		}

		private IDownloadTapiBridge ReturnNewMockTapiBridge()
		{
			lock (_lockToken)
			{
				_mockTapiBridges.Add(new Mock<IDownloadTapiBridge>());
				return _mockTapiBridges[_mockTapiBridges.Count - 1].Object;
			}
		}

		private void VerifyBridges(Action<Mock<IDownloadTapiBridge>> verifyAction)
		{
			foreach (var bridge in _mockTapiBridges)
			{
				verifyAction(bridge);
			}
		}
	}
}
