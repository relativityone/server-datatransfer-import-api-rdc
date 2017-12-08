using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class ImageFilesProgressHandler : ProgressHandler
	{
		public ImageFilesProgressHandler(DownloadStatistics downloadStatistics, ILog logger) : base(downloadStatistics, logger)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadStatistics.MarkImageAsDownloaded(id);
		}
	}
}