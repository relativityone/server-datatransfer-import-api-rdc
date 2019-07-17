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
		private bool _initialized;

		private readonly ILongTextEncodingConverter _longTextEncodingConverter;

		private readonly ILog _logger;

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
			if (!_initialized)
			{
				_logger.LogVerbose("Initializing long text encoding converter.");
				_initialized = true;
				_longTextEncodingConverter.StartListening(this.TapiBridge);
			}

			return TapiBridge.AddPath(transferPath);
		}

		public override void WaitForTransfers()
		{
			if (!_initialized)
			{
				_logger.LogVerbose("Long text encoding conversion bridge hasn't been initialized, so skipping waiting.");
				return;
			}

			try
			{
				const bool KeepJobAlive = true;
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
					_longTextEncodingConverter.StopListening(this.TapiBridge);
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