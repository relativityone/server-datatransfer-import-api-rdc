using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class LongTextProgressHandler : ProgressHandler
	{
		public LongTextProgressHandler(DownloadStatistics downloadStatistics, ILog logger) : base(downloadStatistics, logger)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadStatistics.MarkLongTextAsDownloaded(id);
		}
	}
}