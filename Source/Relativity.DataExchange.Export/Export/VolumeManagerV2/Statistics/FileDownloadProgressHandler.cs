namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.Logging;

	public class FileDownloadProgressHandler : ProgressHandler
	{
		public FileDownloadProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger) : base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsTransferCompleted(string id, int lineNumber)
		{
			DownloadProgressManager.MarkFileAsCompleted(id, lineNumber);
		}
	}
}