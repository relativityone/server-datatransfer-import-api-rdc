using System;

namespace kCura.WinEDDS.Core.Import.Status
{
	public enum ImportProcessStatus
	{
		Start,
		End,
		Update,
		Stopped,
		TransferModeChanged,
		Warning,
		Error,
		FatalError
	}
	public class ImportStatusEventArgs
	{
		public ImportProcessStatus ProcessStatus { private set; get; }
		public string Message { private set; get; }
		public int CurrentRecordIndex { private set; get; }
		public Exception Exception { private set; get; }

		public string JobRunId { set; get; }

		public ImportStatusEventArgs(ImportProcessStatus processStatus, string message)
		{
			ProcessStatus = processStatus;
			Message = message;
		}

		public ImportStatusEventArgs(ImportProcessStatus processStatus, string message, int recordIndex)
		{
			ProcessStatus = processStatus;
			Message = message;
			CurrentRecordIndex = recordIndex;
		}
		public ImportStatusEventArgs(ImportProcessStatus processStatus, string message, int recordIndex, Exception ex)
		{
			ProcessStatus = processStatus;
			Message = message;
			CurrentRecordIndex = recordIndex;
			Exception = ex;
		}
	}
}
