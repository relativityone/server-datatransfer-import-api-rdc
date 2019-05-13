namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.Logging;

	public class LongTextProgressHandler : ProgressHandler
	{
		public LongTextProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger) : base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsDownloaded(string fileName, int lineNumber)
		{
			DownloadProgressManager.MarkLongTextAsDownloaded(fileName, lineNumber);
		}
	}
}