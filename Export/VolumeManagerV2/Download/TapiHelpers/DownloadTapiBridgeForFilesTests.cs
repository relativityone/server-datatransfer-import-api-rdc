using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download.TapiHelpers
{
	[TestFixture]
	public class DownloadTapiBridgeForFilesTests : DownloadTapiBridgeAdapterTests
	{
		private Mock<ITransferClientHandler> _transferClientHandler;

		[SetUp]
		public void SetUp()
		{
			SetUpMocks();

			_transferClientHandler = new Mock<ITransferClientHandler>();

			Instance = new DownloadTapiBridgeForFiles(TapiBridge.Object, ProgressHandler.Object, MessagesHandler.Object, TransferStatistics.Object, _transferClientHandler.Object,
				new NullLogger());
		}

		[Test]
		public void ItShouldDisposeTransferClientHandler()
		{
			Instance.Dispose();

			//ASSERT
			_transferClientHandler.Verify(x => x.Detach());
		}
	}
}