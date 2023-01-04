namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System;

	using Relativity.DataExchange.Logger;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public abstract class ProgressHandler : IProgressHandler
	{
		private readonly ILog _logger;

		protected IDownloadProgressManager DownloadProgressManager { get; }

		protected ProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger)
		{
			this.DownloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public void Subscribe(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Attached tapi bridge {TapiBridgeInstanceId} to the progress handler.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress += this.OnFileProgress;
		}

		public void Unsubscribe(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Detached tapi bridge {TapiBridgeInstanceId} from the progress handler.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress -= this.OnFileProgress;
		}

		protected abstract void MarkAsTransferCompleted(string targetFile, int lineNumber, bool transferResult);

		private void OnFileProgress(object sender, TapiProgressEventArgs e)
		{
            _logger.LogVerbose("Tapi progress event for {FileName} with status {Successful} ({LineNumber}).", e.FileName.Secure(), e.Successful, e.LineNumber);
            if (e.Completed)
			{
				try
				{
					this.MarkAsTransferCompleted(e.TargetFile, e.LineNumber, e.Successful);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error while handling Tapi progress event for {FileName} with status {Successful} ({LineNumber})", e.FileName.Secure(), e.Successful, e.LineNumber);
				}
			}
		}
	}
}