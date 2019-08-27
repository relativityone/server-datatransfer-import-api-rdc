namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.DataExchange.Export.VolumeManagerV2.Statistics.ITransferStatistics;


	public class DownloadTapiBridgeWithEncodingConversion : DownloadTapiBridgeAdapter
	{
		private readonly object _syncRoot = new object();
		private bool _initialized;

		public DownloadTapiBridgeWithEncodingConversion(
			ITapiBridge downloadTapiBridge,
			IProgressHandler progressHandler,
			IMessagesHandler messagesHandler,
			ITransferStatistics transferStatistics,
			ILog logger)
			: base(downloadTapiBridge, progressHandler, messagesHandler, transferStatistics, logger)
		{
			_initialized = false;
		}


		public override string QueueDownload(TransferPath transferPath)
		{
			lock (this._syncRoot)
			{
				_initialized = true;
			}
			return this.TapiBridge.AddPath(transferPath);
		}

		public override void WaitForTransfers()
		{
			lock (_syncRoot)
			{
				if (!_initialized)
				{
					_logger.LogVerbose(
						"Long text encoding conversion bridge hasn't been initialized, so skipping waiting.");

					this.FileDownloadCompleted.OnNext(true);
					return;
				}
			}
			try
			{
				const bool KeepJobAlive = false;
				this.TapiBridge.WaitForTransfers(
					"Waiting for all long files to download...",
					"Long file downloads completed.",
					"Failed to wait for all pending long file downloads.",
					KeepJobAlive);
			}
			catch(Exception ex)
			{
				this.FileDownloadCompleted.OnNext(false);
				_logger.LogError(ex, "Error occurred when trying to stop LongText encoding conversion after TAPI client failure.");
				throw;
			}
			this.FileDownloadCompleted.OnNext(true);
		}

	}
}