using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class LongTextProgressHandler : ProgressHandler
	{
		public LongTextProgressHandler(DownloadStatistics downloadStatistics) : base(downloadStatistics)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadStatistics.MarkLongTextAsDownloaded(id);
		}
	}
}