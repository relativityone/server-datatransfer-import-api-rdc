namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface IDownloadProgressManager
	{
		void MarkImageAsDownloaded(string id);
		void MarkLongTextAsDownloaded(string id);
		void MarkNativeAsDownloaded(string id);
	}
}