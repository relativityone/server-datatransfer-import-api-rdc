using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using Moq;
using NUnit.Framework;
using Relativity.Transfer;
using ITransferStatistics = kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics.ITransferStatistics;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download.TapiHelpers
{
	[TestFixture]
	public abstract class DownloadTapiBridgeAdapterTests
	{
		protected DownloadTapiBridgeAdapter Instance { get; set; }

		protected Mock<ITapiBridge> TapiBridge { get; private set; }
		protected Mock<IProgressHandler> ProgressHandler { get; private set; }
		protected Mock<IMessagesHandler> MessagesHandler { get; private set; }
		protected Mock<ITransferStatistics> TransferStatistics { get; private set; }

		protected void SetUpMocks()
		{
			TapiBridge = new Mock<ITapiBridge>();
			ProgressHandler = new Mock<IProgressHandler>();
			MessagesHandler = new Mock<IMessagesHandler>();
			TransferStatistics = new Mock<ITransferStatistics>();
		}

		[Test]
		public void ItShouldAttachAndDetach()
		{
			Instance.Dispose();

			//ASSERT
			ProgressHandler.Verify(x => x.Attach(TapiBridge.Object), Times.Once);
			ProgressHandler.Verify(x => x.Detach(), Times.Once);

			MessagesHandler.Verify(x => x.Attach(TapiBridge.Object), Times.Once);
			MessagesHandler.Verify(x => x.Detach(), Times.Once);

			TransferStatistics.Verify(x => x.Attach(TapiBridge.Object), Times.Once);
			TransferStatistics.Verify(x => x.Detach(), Times.Once);

			TapiBridge.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void ItShouldWaitForTransferJob()
		{
			//ACT
			Instance.QueueDownload(new TransferPath());
			Instance.WaitForTransferJob();

			//ASSERT
			TapiBridge.Verify(x => x.WaitForTransferJob(), Times.Once);
		}

		[Test]
		public void ItShouldNotWaitForTransferJobWhenNothingHasBeenAddedToQueue()
		{
			//ACT
			Instance.WaitForTransferJob();

			//ASSERT
			TapiBridge.Verify(x => x.WaitForTransferJob(), Times.Never);
		}

		[Test]
		public void ItShouldAddTransferPath()
		{
			TransferPath transferPath = new TransferPath();

			//ACT
			Instance.QueueDownload(transferPath);

			//ASSERT
			TapiBridge.Verify(x => x.AddPath(transferPath), Times.Once);
		}

		[Test]
		public void ItShouldDisconnect()
		{
			//ACT
			Instance.Disconnect();

			//ASSERT
			TapiBridge.Verify(x => x.Disconnect(), Times.Once);
		}
	}
}