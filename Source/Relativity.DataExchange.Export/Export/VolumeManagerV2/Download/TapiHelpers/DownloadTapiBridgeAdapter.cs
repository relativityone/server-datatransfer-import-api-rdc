namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.DataExchange.Export.VolumeManagerV2.Statistics.ITransferStatistics;

	public abstract class DownloadTapiBridgeAdapter : IDownloadTapiBridge
	{
		private readonly IProgressHandler _progressHandler;
		private readonly IMessagesHandler _messageHandler;
		private readonly ITransferStatistics _transferStatistics;

		protected DownloadTapiBridgeAdapter(
			ITapiBridge bridge,
			IProgressHandler progressHandler,
			IMessagesHandler messageHandler,
			ITransferStatistics transferStatistics)
		{
			bridge.ThrowIfNull(nameof(bridge));
			progressHandler.ThrowIfNull(nameof(progressHandler));
			messageHandler.ThrowIfNull(nameof(messageHandler));
			transferStatistics.ThrowIfNull(nameof(transferStatistics));
			this.TapiBridge = bridge;
			_progressHandler = progressHandler;
			_messageHandler = messageHandler;
			_transferStatistics = transferStatistics;

			_messageHandler.Attach(this.TapiBridge);
			_progressHandler.Attach(this.TapiBridge);
			_transferStatistics.Attach(this.TapiBridge);
		}

		public TapiClient Client => this.TapiBridge.Client;

		public TapiBridgeParameters2 Parameters => this.TapiBridge.Parameters;

		protected ITapiBridge TapiBridge { get; }

		public virtual void Dispose()
		{
			_progressHandler.Detach();
			_messageHandler.Detach();
			_transferStatistics.Detach();
			this.TapiBridge.Dispose();
		}

		public abstract string QueueDownload(TransferPath transferPath);

		public abstract void WaitForTransfers();
	}
}