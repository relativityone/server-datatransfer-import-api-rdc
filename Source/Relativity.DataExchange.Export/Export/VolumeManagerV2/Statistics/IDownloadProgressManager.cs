namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	public interface IDownloadProgressManager
	{
		void MarkArtifactAsError(int artifactId, string message);
		void MarkLongTextAsCompleted(string id, int lineNumber);
		void MarkFileAsCompleted(string id, int lineNumber);
	}
}