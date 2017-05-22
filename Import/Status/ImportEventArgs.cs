using System;
using kCura.WinEDDS.Core.Import.Errors;

namespace kCura.WinEDDS.Core.Import.Status
{
	public enum ImportEventType
	{
		Start,
		End,
		TransferModeChanged,
		Error,
		FatalError,
	}

	public class ImportEventArgs
	{
		public ImportEventType EventType { private set; get; }
		public string Message { private set; get; }
		public int LineNumber { private set; get; }
		public Exception Exception { private set; get; }
		public LineError LineError { private set; get; }

		public string JobRunId { set; get; }

		public ImportEventArgs(ImportEventType eventType, string message)
		{
			EventType = eventType;
			Message = message;
		}

		public ImportEventArgs(LineError lineError)
		{
			EventType = ImportEventType.Error;
			LineError = lineError;
		}

		public ImportEventArgs(ImportEventType eventType, string message, int recordIndex)
		{
			EventType = eventType;
			Message = message;
			LineNumber = recordIndex;
		}
		public ImportEventArgs(ImportEventType eventType, string message, int recordIndex, Exception ex)
		{
			EventType = eventType;
			Message = message;
			LineNumber = recordIndex;
			Exception = ex;
		}
	}
}
