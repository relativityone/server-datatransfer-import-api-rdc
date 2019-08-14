namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.Logging;

	public class LongTextProgressHandler : ProgressHandler
	{
		public LongTextProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger) : base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsTransferCompleted(string fileName, int lineNumber)
		{
			DownloadProgressManager.MarkLongTextAsCompleted(fileName, lineNumber);
		}
	}
}