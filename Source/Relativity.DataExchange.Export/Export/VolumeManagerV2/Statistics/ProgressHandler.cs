namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System;

	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	public abstract class ProgressHandler : IProgressHandler
	{
		private ITapiBridge _tapiBridge;

		private readonly ILog _logger;

		protected IDownloadProgressManager DownloadProgressManager { get; }

		protected ProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger)
		{
			DownloadProgressManager = downloadProgressManager;
			_logger = logger;
		}

		public void Attach(ITapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiProgress += OnFileProgress;
		}

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
		protected abstract void MarkAsDownloaded(string id, int lineNumber);

		public void Detach()
		{
			_tapiBridge.TapiProgress -= OnFileProgress;
		}
	}
}