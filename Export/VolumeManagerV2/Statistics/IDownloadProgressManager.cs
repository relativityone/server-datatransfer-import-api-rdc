namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface IDownloadProgressManager
	{
		void MarkImageAsDownloaded(string id, int lineNumber);
		void MarkLongTextAsDownloaded(string id, int lineNumber);
		void MarkNativeAsDownloaded(string id, int lineNumber);
	}
}