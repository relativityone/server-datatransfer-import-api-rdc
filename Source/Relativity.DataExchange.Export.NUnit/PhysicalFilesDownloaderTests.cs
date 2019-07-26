// -----------------------------------------------------------------------------------------------------
// <copyright file="PhysicalFilesDownloaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;
	using Relativity.Transfer;

	[TestFixture]
	public class PhysicalFilesDownloaderTests
	{
		private const int _DEFAULT_FILES_PER_FILESHARE = 3;
		private const int _DEFAULT_TASK_COUNT = 2;
		private readonly object _lockToken = new object();
		private Dictionary<string, IRelativityFileShareSettings> _fileShareSettingsCache;
		private ILog _logger;
		private List<Mock<IDownloadTapiBridge>> _mockTapiBridges;
		private Mock<IExportConfig> _mockExportConfig;
		private Mock<IFileShareSettingsService> _fileshareSettingsService;
		private Mock<IFileTapiBridgePool> _fileTapiBridgePool;
		private PhysicalFilesDownloader _downloader;
		private SafeIncrement _safeIncrement;
		private string[] _availableFileShares;

		[SetUp]
		public void SetUp()
		{
			this._fileShareSettingsCache = new Dictionary<string, IRelativityFileShareSettings>();
			this._logger = new NullLogger();
			this._safeIncrement = new SafeIncrement();
			this._availableFileShares = new[] { @"\\fileShare.one", @"\\fileShare.two", @"\\fileShare.three" };
			this._fileshareSettingsService = new Mock<IFileShareSettingsService>();
			this._fileshareSettingsService.Setup(s => s.GetSettingsForFileShare(It.IsAny<int>(), It.IsAny<string>()))
				.Returns((int artifactId, string val) => this.ReturnSettingsForFileshare(val));
			this._mockTapiBridges = new List<Mock<IDownloadTapiBridge>>();
			this._fileTapiBridgePool = new Mock<IFileTapiBridgePool>();
			this._fileTapiBridgePool
				.Setup(pool => pool.Request(It.IsAny<IRelativityFileShareSettings>(), It.IsAny<CancellationToken>()))
				.Returns(this.ReturnNewMockTapiBridge);
			this._mockExportConfig = new Mock<IExportConfig>();
			this._mockExportConfig.SetupGet(config => config.MaxNumberOfFileExportTasks).Returns(_DEFAULT_TASK_COUNT);
			this._downloader = new PhysicalFilesDownloader(this._fileshareSettingsService.Object, this._fileTapiBridgePool.Object, this._mockExportConfig.Object, this._safeIncrement, this._logger);
		}

		[Test]
		public void ItShouldNotThrowOnEmptyRequestList()
		{
			List<ExportRequest> emptyRequestList = new List<ExportRequest>();

			Assert.DoesNotThrowAsync(async () => await this._downloader.DownloadFilesAsync(emptyRequestList, CancellationToken.None).ConfigureAwait(false));
		}

		[Test]
		public async Task ItShouldCreateTapiBridgesAccordingly()
		{
			List<ExportRequest> requests = this.CreateThreeExportRequestsPerFileShare(this._availableFileShares).ToList();

			await this._downloader.DownloadFilesAsync(requests, CancellationToken.None).ConfigureAwait(false);

			this._fileshareSettingsService.Verify(
				svc => svc.GetSettingsForFileShare(It.IsAny<int>(), It.IsAny<string>()),
				Times.Exactly(requests.Count));
			this._fileTapiBridgePool.Verify(pool => pool.Request(It.IsAny<IRelativityFileShareSettings>(), It.IsAny<CancellationToken>()), Times.Exactly(this._availableFileShares.Length));
		}

		[Test]
		public async Task ItShouldManageTapiBridgeLifecycleCorrectly()
		{
			List<ExportRequest> requests = this.CreateThreeExportRequestsPerFileShare(this._availableFileShares).ToList();

			await this._downloader.DownloadFilesAsync(requests, CancellationToken.None).ConfigureAwait(false);

			this.VerifyBridges(m => m.Verify(b => b.QueueDownload(It.IsAny<TransferPath>()), Times.Exactly(_DEFAULT_FILES_PER_FILESHARE)));
			this.VerifyBridges(m => m.Verify(b => b.WaitForTransfers(), Times.Once));
		}

		[Test]
		public void ItShouldThrowTaskCanceledExceptionWhenDownloaderThrowsOne()
		{
			var mockTapiBridge = new Mock<IDownloadTapiBridge>();
			mockTapiBridge.Setup(b => b.WaitForTransfers()).Throws<TaskCanceledException>();
			this._fileTapiBridgePool
				.Setup(pool => pool.Request(It.IsAny<IRelativityFileShareSettings>(), It.IsAny<CancellationToken>()))
				.Returns(mockTapiBridge.Object);
			List<ExportRequest> requests = this.CreateThreeExportRequestsPerFileShare(this._availableFileShares).ToList();

			var downloader = new PhysicalFilesDownloader(this._fileshareSettingsService.Object, this._fileTapiBridgePool.Object, this._mockExportConfig.Object, this._safeIncrement, this._logger);

			Assert.ThrowsAsync<TaskCanceledException>(async () => await downloader.DownloadFilesAsync(requests, CancellationToken.None).ConfigureAwait(false));
		}

		private IEnumerable<ExportRequest> CreateThreeExportRequestsPerFileShare(IEnumerable<string> fileShares)
		{
			return fileShares.SelectMany(f => Enumerable.Repeat(f, _DEFAULT_FILES_PER_FILESHARE)).Select(this.CreatePhysicalFileExportRequest);
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
			string fileshare = this._availableFileShares.First(fileUrl.Contains);
			if (!this._fileShareSettingsCache.ContainsKey(fileshare))
			{
				this._fileShareSettingsCache[fileshare] = new Mock<IRelativityFileShareSettings>().Object;
			}

			return this._fileShareSettingsCache[fileshare];
		}

		private IDownloadTapiBridge ReturnNewMockTapiBridge()
		{
			lock (this._lockToken)
			{
				this._mockTapiBridges.Add(new Mock<IDownloadTapiBridge>());
				return this._mockTapiBridges[this._mockTapiBridges.Count - 1].Object;
			}
		}

		private void VerifyBridges(Action<Mock<IDownloadTapiBridge>> verifyAction)
		{
			foreach (var bridge in this._mockTapiBridges)
			{
				verifyAction(bridge);
			}
		}
	}
}
