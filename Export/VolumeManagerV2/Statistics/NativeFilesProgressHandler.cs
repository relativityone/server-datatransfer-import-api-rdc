using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class NativeFilesProgressHandler : ProgressHandler
	{
		public NativeFilesProgressHandler(DownloadStatistics downloadStatistics, ILog logger) : base(downloadStatistics, logger)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadStatistics.MarkNativeAsDownloaded(id);
		}
	}
}