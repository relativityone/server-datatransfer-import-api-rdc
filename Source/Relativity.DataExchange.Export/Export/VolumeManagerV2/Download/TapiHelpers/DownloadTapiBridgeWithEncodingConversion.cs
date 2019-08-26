namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;
	using Relativity.Transfer;

	using ITransferStatistics = Relativity.DataExchange.Export.VolumeManagerV2.Statistics.ITransferStatistics;

	public class DownloadTapiBridgeWithEncodingConversion : DownloadTapiBridgeAdapter
	{
		private readonly object _syncRoot = new object();
		private readonly ILongTextEncodingConverter _longTextEncodingConverter;
		private readonly ILog _logger;
		private bool _initialized;

		public DownloadTapiBridgeWithEncodingConversion(
			ITapiBridge downloadTapiBridge,
			IProgressHandler progressHandler,
			IMessagesHandler messagesHandler,
			ITransferStatistics transferStatistics,
			ILongTextEncodingConverter longTextEncodingConverter,
			ILog logger)
			: base(downloadTapiBridge, progressHandler, messagesHandler, transferStatistics)
		{
			_longTextEncodingConverter = longTextEncodingConverter;
			_logger = logger;
			_initialized = false;
		}

		public override string QueueDownload(TransferPath transferPath)
		{
			lock (_syncRoot)
			{
				if (!_initialized)
				{
					_logger.LogVerbose("Initializing long text encoding converter...");
					_longTextEncodingConverter.Subscribe(this.TapiBridge);
					_logger.LogVerbose("Initialized long text encoding converter.");
					_initialized = true;
				}
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
					return;
				}
			}

			try
			{
				// REL-344406: The LongTextEncodingConverter implementation is entirely dependent on awaiting completion of the transfer job.
				// TODO: Decouple encoding conversion entirely from the batch and into an independent queue.
				const bool KeepJobAlive = false;
				this.TapiBridge.WaitForTransfers(
					"Waiting for all long files to download...",
					"Long file downloads completed.",
					"Failed to wait for all pending long file downloads.",
					KeepJobAlive);
			}
			finally
			{
				try
				{
					_longTextEncodingConverter.Unsubscribe(this.TapiBridge);
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Error occurred when trying to stop LongText encoding conversion after TAPI client failure.");
				}
			}

			_longTextEncodingConverter.WaitForConversionCompletion();
		}
	}
}