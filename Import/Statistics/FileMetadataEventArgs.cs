using System;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class FileMetadataEventArgs : EventArgs
	{
		public FileMetadataEventArgs(string recordId, int lineNumber)
		{
			RecordId = recordId;
			LineNumber = lineNumber;
		}

		public string RecordId { get; }
		public int LineNumber { get; }
	}
}