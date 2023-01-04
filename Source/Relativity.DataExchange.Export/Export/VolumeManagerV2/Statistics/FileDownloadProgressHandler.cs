namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.Logging;

	public class FileDownloadProgressHandler : ProgressHandler
	{
		public FileDownloadProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger)
			: base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsTransferCompleted(string targetFile, int lineNumber, bool transferResult)
		{
			this.DownloadProgressManager.MarkFileAsCompleted(targetFile, lineNumber, transferResult);
		}
	}
}