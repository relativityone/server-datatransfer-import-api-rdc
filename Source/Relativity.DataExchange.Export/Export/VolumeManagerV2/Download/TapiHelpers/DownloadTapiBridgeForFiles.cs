namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.DataExchange.Export.VolumeManagerV2.Statistics.ITransferStatistics;

	public class DownloadTapiBridgeForFiles : DownloadTapiBridgeAdapter
	{
		private readonly ITransferClientHandler _transferClientHandler;
		private readonly ILog _logger;
		private bool _isEmpty;

		public DownloadTapiBridgeForFiles(
			ITapiBridge bridge,
			IProgressHandler progressHandler,
			IMessagesHandler messagesHandler,
			ITransferStatistics transferStatistics,
			ITransferClientHandler transferClientHandler,
			ILog logger)
			: base(bridge, progressHandler, messagesHandler, transferStatistics)
		{
			_transferClientHandler = transferClientHandler.ThrowIfNull(nameof(transferClientHandler));
			_logger = logger.ThrowIfNull(nameof(logger));
			_transferClientHandler.Subscribe(bridge);
			_isEmpty = true;
		}

		public override string QueueDownload(TransferPath transferPath)
		{
			_isEmpty = false;
			return this.TapiBridge.AddPath(transferPath);
		}

		public override void WaitForTransfers()
		{
			if (_isEmpty)
			{
				_logger.LogVerbose("Files transfer bridge is empty, so skipping waiting.");
				return;
			}

			const bool KeepJobAlive = true;
			this.TapiBridge.WaitForTransfers(
				"Waiting for all native files to download...",
				"Native file downloads completed.",
				"Failed to wait for all pending native file downloads.",
				KeepJobAlive);
		}

		public override void Dispose()
		{
			_transferClientHandler.Unsubscribe(this.TapiBridge);
			base.Dispose();
		}
	}
}