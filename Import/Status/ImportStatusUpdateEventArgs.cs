

namespace kCura.WinEDDS.Core.Import.Status
{
	public enum StatusUpdateType
	{
		Progress,
		Warning,
		Count,
		Update,
		End,
		Error
	}

	public class ImportStatusUpdateEventArgs
	{
		public StatusUpdateType Type { get; private set; }
		public string Message { get; private set; }
		public int LineNumber { get; private set; }

		public ImportStatusUpdateEventArgs(StatusUpdateType type, string message, int lineNumber)
		{
			Type = type;
			Message = message;
			LineNumber = lineNumber;
		}
		
	}
}
