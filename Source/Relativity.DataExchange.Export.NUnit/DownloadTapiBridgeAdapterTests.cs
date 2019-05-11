// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridgeAdapterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.DataExchange.Export.VolumeManagerV2.Statistics.ITransferStatistics;

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
			this.TapiBridge = new Mock<ITapiBridge>();
			this.ProgressHandler = new Mock<IProgressHandler>();
			this.MessagesHandler = new Mock<IMessagesHandler>();
			this.TransferStatistics = new Mock<ITransferStatistics>();
		}

		[Test]
		public void ItShouldAttachAndDetach()
		{
			this.Instance.Dispose();

			// ASSERT
			this.ProgressHandler.Verify(x => x.Attach(this.TapiBridge.Object), Times.Once);
			this.ProgressHandler.Verify(x => x.Detach(), Times.Once);

			this.MessagesHandler.Verify(x => x.Attach(this.TapiBridge.Object), Times.Once);
			this.MessagesHandler.Verify(x => x.Detach(), Times.Once);

			this.TransferStatistics.Verify(x => x.Attach(this.TapiBridge.Object), Times.Once);
			this.TransferStatistics.Verify(x => x.Detach(), Times.Once);

			this.TapiBridge.Verify(x => x.Dispose(), Times.Once);
		}

		[Test]
		public void ItShouldWaitForTransferJob()
		{
			// ACT
			this.Instance.QueueDownload(new TransferPath());
			this.Instance.WaitForTransferJob();

			// ASSERT
			this.TapiBridge.Verify(x => x.WaitForTransferJob(), Times.Once);
		}

		[Test]
		public void ItShouldNotWaitForTransferJobWhenNothingHasBeenAddedToQueue()
		{
			// ACT
			this.Instance.WaitForTransferJob();

			// ASSERT
			this.TapiBridge.Verify(x => x.WaitForTransferJob(), Times.Never);
		}

		[Test]
		public void ItShouldAddTransferPath()
		{
			TransferPath transferPath = new TransferPath();

			// ACT
			this.Instance.QueueDownload(transferPath);

			// ASSERT
			this.TapiBridge.Verify(x => x.AddPath(transferPath), Times.Once);
		}

		[Test]
		public void ItShouldDisconnect()
		{
			// ACT
			this.Instance.Disconnect();

			// ASSERT
			this.TapiBridge.Verify(x => x.Disconnect(), Times.Once);
		}
	}
}