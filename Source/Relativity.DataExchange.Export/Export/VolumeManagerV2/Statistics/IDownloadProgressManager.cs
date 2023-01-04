namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	public interface IDownloadProgressManager
	{
		void MarkArtifactAsError(int artifactId, string message);
		void MarkLongTextAsCompleted(string targetFile, int lineNumber, bool transferResult);
		void MarkFileAsCompleted(string targetFile, int lineNumber, bool transferResult);
	}
}