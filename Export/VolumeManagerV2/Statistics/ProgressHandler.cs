﻿using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public abstract class ProgressHandler : IProgressHandler
	{
		private TapiBridge _tapiBridge;

		private readonly ILog _logger;

		protected DownloadStatistics DownloadStatistics { get; }

		protected ProgressHandler(DownloadStatistics downloadStatistics, ILog logger)
		{
			DownloadStatistics = downloadStatistics;
			_logger = logger;
		}

		public void Attach(TapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiProgress += OnFileProgress;
		}

		private void OnFileProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Tapi progress event for {fileName} with status {status} ({lineNumber}).", e.FileName, e.Status, e.LineNumber);
			MarkAsDownloaded(e.FileName);
		}

		protected abstract void MarkAsDownloaded(string id);

		public void Detach()
		{
			_tapiBridge.TapiProgress -= OnFileProgress;
		}
	}
}