using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class MetadataStatisticsHandler : IMetadataStatisticsHandler
	{
		public event EventHandler<FileMetadataEventArgs> FileMetadataProcessed;
		public event EventHandler<MetadataFileUploadEventArgs> UploadingMetadataFile;
		public event EventHandler BulkImportMetadataStarted;

		public void RaiseFileMetadataProcessedEvent(string recordId, int lineNumber)
		{
			FileMetadataProcessed?.Invoke(this, new FileMetadataEventArgs(recordId, lineNumber));
		}

		public void RaiseUploadingMetadataFileEvent(int metadataFileChunks, int currentChunk)
		{
			UploadingMetadataFile?.Invoke(this, new MetadataFileUploadEventArgs(metadataFileChunks, currentChunk));
		}

		public void RaiseBulkImportMetadataStartedEvent()
		{
			BulkImportMetadataStarted?.Invoke(this, EventArgs.Empty);
		}
	}
}