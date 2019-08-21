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
					this.Attach(this.TapiBridge);
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
					this.Detach(this.TapiBridge);
				}
				catch (Exception e)
				{
					_logger.LogError(e, "Error occurred when trying to stop LongText encoding conversion after TAPI client failure.");
				}
			}

			_longTextEncodingConverter.WaitForConversionCompletion().GetAwaiter().GetResult();
		}

		private void Attach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			this._logger.LogVerbose(
				"Attached tapi bridge {TapiBridgeInstanceId} to the long text encoding converter.",
				tapiBridge.InstanceId);
			_longTextEncodingConverter.NotifyStartConversion();
			tapiBridge.TapiProgress += this.OnTapiProgress;
		}

		private void Detach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			this._logger.LogVerbose(
				"Detached tapi bridge {TapiBridgeInstanceId} from the long text encoding converter.",
				tapiBridge.InstanceId);
			_longTextEncodingConverter.NotifyStopConversion();
			tapiBridge.TapiProgress -= this.OnTapiProgress;
		}

		private void OnTapiProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose(
				"Long text encoding conversion progress event for file {FileName} with status {Successful}.",
				e.FileName,
				e.Successful);
			if (e.Successful)
			{
				try
				{

					_logger.LogVerbose("Preparing to add the '{LongTextFileName}' long text file to the queue...", e.FileName);
					_longTextEncodingConverter.AddForConversion(e.FileName);
					_logger.LogVerbose("Successfully added the '{LongTextFileName}' long text file to the queue.", e.FileName);
				}
				catch (InvalidOperationException e2)
				{
					_logger.LogError(
						e2,
						"The long text encoding converter received a transfer successful progress event but the blocking collection has already been marked as completed. This exception suggests either a logic or task switch context issue.");
					throw;
				}
			}
		}
	}
}