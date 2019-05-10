﻿namespace Relativity.Export.VolumeManagerV2.Download.TapiHelpers
{
	using Relativity.Export.VolumeManagerV2.Statistics;
	using Relativity.Logging;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.Export.VolumeManagerV2.Statistics.ITransferStatistics;

	public class DownloadTapiBridgeForFiles : DownloadTapiBridgeAdapter
	{
		private bool _isEmpty;

		private readonly ITransferClientHandler _transferClientHandler;
		private readonly ILog _logger;

		public DownloadTapiBridgeForFiles(ITapiBridge downloadTapiBridge, IProgressHandler progressHandler, IMessagesHandler messagesHandler,
			ITransferStatistics transferStatistics,
			ITransferClientHandler transferClientHandler, ILog logger) : base(downloadTapiBridge, progressHandler, messagesHandler, transferStatistics)
		{
			_transferClientHandler = transferClientHandler;
			_logger = logger;
			_transferClientHandler.Attach(downloadTapiBridge);
			_isEmpty = true;
		}

		public override string QueueDownload(TransferPath transferPath)
		{
			_isEmpty = false;
			return TapiBridge.AddPath(transferPath);
		}

		public override void WaitForTransferJob()
		{
			if (_isEmpty)
			{
				_logger.LogVerbose("Files transfer bridge is empty, so skipping waiting.");
				return;
			}

			TapiBridge.WaitForTransferJob();
		}

		public override void Dispose()
		{
			_transferClientHandler.Detach();
			base.Dispose();
		}
	}
}