using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class ImageFilesProgressHandler : ProgressHandler
	{
		public ImageFilesProgressHandler(DownloadStatistics downloadStatistics) : base(downloadStatistics)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadStatistics.MarkImageAsDownloaded(id);
		}
	}
}