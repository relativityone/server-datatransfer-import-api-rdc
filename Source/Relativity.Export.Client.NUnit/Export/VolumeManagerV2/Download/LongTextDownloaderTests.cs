using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
	public class LongTextDownloaderTests
	{
		private LongTextDownloader _instance;

		private Mock<ILongTextTapiBridgePool> _longTextTapiBridgePool;
		private Mock<IDownloadTapiBridge> _bridge;

		[SetUp]
		public void SetUp()
		{
			_longTextTapiBridgePool = new Mock<ILongTextTapiBridgePool>();
			_bridge = new Mock<IDownloadTapiBridge>();

			_longTextTapiBridgePool.Setup(x => x.Request(CancellationToken.None)).Returns(_bridge.Object);

			_instance = new LongTextDownloader(new SafeIncrement(), _longTextTapiBridgePool.Object, new NullLogger());
		}

		[Test]
		public async Task ItShouldRequestAndReleaseBridge()
		{
			// ACT
			await _instance.DownloadAsync(new List<LongTextExportRequest>(), CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			_longTextTapiBridgePool.Verify(x => x.Request(CancellationToken.None), Times.Once);
			_longTextTapiBridgePool.Verify(x => x.Release(_bridge.Object), Times.Once);
		}

		[Test]
		public async Task ItShouldDownloadAllRequests()
		{
			ObjectExportInfo artifact = new ObjectExportInfo();
			List<LongTextExportRequest> longTextExportRequests = new List<LongTextExportRequest>
			{
				LongTextExportRequest.CreateRequestForFullText(artifact, 1, "a"),
				LongTextExportRequest.CreateRequestForFullText(artifact, 2, "b")
			};

			// ACT
			await _instance.DownloadAsync(longTextExportRequests, CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			_bridge.Verify(x => x.WaitForTransferJob(), Times.Once);
			_bridge.Verify(x => x.QueueDownload(It.Is<TransferPath>(t => t.Order == 1)));
			_bridge.Verify(x => x.QueueDownload(It.Is<TransferPath>(t => t.Order == 2)));
		}

		[Test]
		public async Task ItShouldSetTextFileName()
		{
			const string expectedFileName = "file name";
			_bridge.Setup(x => x.QueueDownload(It.IsAny<TransferPath>())).Returns(expectedFileName);

			ObjectExportInfo artifact = new ObjectExportInfo();
			List<LongTextExportRequest> longTextExportRequests = new List<LongTextExportRequest>
			{
				LongTextExportRequest.CreateRequestForFullText(artifact, 1, "a")
			};

			// ACT
			await _instance.DownloadAsync(longTextExportRequests, CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			Assert.That(longTextExportRequests[0].FileName, Is.EqualTo(expectedFileName));
		}
	}
}