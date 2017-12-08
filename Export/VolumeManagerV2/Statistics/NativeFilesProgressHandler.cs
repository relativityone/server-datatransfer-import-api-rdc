using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class NativeFilesProgressHandler : ProgressHandler
	{
		public NativeFilesProgressHandler(DownloadStatistics downloadStatistics) : base(downloadStatistics)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadStatistics.MarkNativeAsDownloaded(id);
		}
	}
}