using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
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
			_logger.LogVerbose("Tapi progress event for {fileName} with status {status} ({lineNumber}).", e.FileName, e.Status, e.LineNumber);
			if (e.Status)
			{
				try
				{
					MarkAsDownloaded(e.FileName, e.LineNumber);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error while handling Tapi progress event for {fileName} with status {status} ({lineNumber}).", e.FileName, e.Status, e.LineNumber);
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