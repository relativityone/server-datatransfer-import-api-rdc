using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface IMetadataStatisticsHandler
	{
		event EventHandler<FileMetadataEventArgs> FileMetadataProcessed;
		event EventHandler<MetadataFileUploadEventArgs> UploadingMetadataFile;
		event EventHandler BulkImportMetadataStarted; 

		void RaiseFileMetadataProcessedEvent(string recordId, int lineNumber);
		void RaiseUploadingMetadataFileEvent(int metadataFileChunks, int currentChunk);
		void RaiseBulkImportMetadataStartedEvent();
	}
}