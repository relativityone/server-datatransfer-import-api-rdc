using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface IMetadataStatisticsHandler
	{
		event EventHandler<FileMetadataEventArgs> FileMetadataProcessed;

		void RaiseFileMetadataProcessedEvent(string recordId, int lineNumber);
	}
}