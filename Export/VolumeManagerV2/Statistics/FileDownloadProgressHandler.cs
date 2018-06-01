using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class FileDownloadProgressHandler : ProgressHandler
	{
		public FileDownloadProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger) : base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsDownloaded(string id, int lineNumber)
		{
			DownloadProgressManager.MarkFileAsDownloaded(id, lineNumber);
		}
	}
}