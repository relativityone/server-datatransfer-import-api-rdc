using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class LongTextProgressHandler : ProgressHandler
	{
		public LongTextProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger) : base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsDownloaded(string id, int lineNumber)
		{
			DownloadProgressManager.MarkLongTextAsDownloaded(id, lineNumber);
		}
	}
}