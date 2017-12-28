using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class ImageFilesProgressHandler : ProgressHandler
	{
		public ImageFilesProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger) : base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadProgressManager.MarkImageAsDownloaded(id);
		}
	}
}