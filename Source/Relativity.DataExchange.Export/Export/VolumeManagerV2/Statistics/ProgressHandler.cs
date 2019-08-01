namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System;

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

		public void Attach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Attached tapi bridge {TapiBridgeInstanceId} to the progress handler.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress += this.OnFileProgress;
		}

		public void Detach(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Detached tapi bridge {TapiBridgeInstanceId} from the progress handler.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress -= this.OnFileProgress;
		}

		protected abstract void MarkAsDownloaded(string id, int lineNumber);

		private void OnFileProgress(object sender, TapiProgressEventArgs e)
		{
            _logger.LogVerbose("Tapi progress event for {FileName} with status {Successful} ({LineNumber}).", e.FileName, e.Successful, e.LineNumber);
            if (e.Successful)
			{
				try
				{
					MarkAsDownloaded(e.FileName, e.LineNumber);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error while handling Tapi progress event for {FileName} with status {Successful} ({LineNumber})", e.FileName, e.Successful, e.LineNumber);
				}
			}
		}
	}
}