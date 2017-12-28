using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class NativeFilesProgressHandler : ProgressHandler
	{
		public NativeFilesProgressHandler(IDownloadProgressManager downloadProgressManager, ILog logger) : base(downloadProgressManager, logger)
		{
		}

		protected override void MarkAsDownloaded(string id)
		{
			DownloadProgressManager.MarkNativeAsDownloaded(id);
		}
	}
}