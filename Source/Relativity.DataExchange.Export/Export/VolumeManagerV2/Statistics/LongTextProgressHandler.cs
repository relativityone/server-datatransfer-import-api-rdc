namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.Logging;

	public class LongTextProgressHandler : ProgressHandler
	{
		public LongTextProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger)
			: base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsTransferCompleted(string targetFile, int lineNumber, bool transferResult)
		{
			this.DownloadProgressManager.MarkLongTextAsCompleted(targetFile, lineNumber, transferResult);
		}
	}
}