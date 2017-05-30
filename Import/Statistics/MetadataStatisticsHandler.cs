using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class MetadataStatisticsHandler : IMetadataStatisticsHandler
	{
		public event EventHandler<FileMetadataEventArgs> FileMetadataProcessed;

		public void RaiseFileMetadataProcessedEvent(string recordId, int lineNumber)
		{
			FileMetadataProcessed?.Invoke(this, new FileMetadataEventArgs(recordId, lineNumber));
		}
	}
}