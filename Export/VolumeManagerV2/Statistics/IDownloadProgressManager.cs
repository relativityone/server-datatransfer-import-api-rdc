namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface IDownloadProgressManager
	{
		void MarkLongTextAsDownloaded(string id, int lineNumber);
		void MarkFileAsDownloaded(string id, int lineNumber);
	}
}