using System;
using kCura.WinEDDS.Core.Import.Errors;

namespace kCura.WinEDDS.Core.Import.Status
{
	public class ImportStatusManager : IImportStatusManager
	{
		private ImportContext _importContext;

		public event EventHandler<ImportEventArgs> EventOccurred;
		public event EventHandler<ImportStatusUpdateEventArgs> UpdateStatus;

		#region Interface Methods

		public void RaiseStartImportEvent(object sender)
		{
			EventOccurred?.Invoke(sender, CreateImportEventArgs(ImportEventType.Start, string.Empty));
		}

		public void RaiseEndImportEvent(object sender)
		{
			EventOccurred?.Invoke(sender, CreateImportEventArgs(ImportEventType.End, string.Empty));
		}		

		public void RaiseErrorImportEvent(object sender, LineError lineError)
		{
			EventOccurred?.Invoke(sender, new ImportEventArgs(lineError));
		}

		public void RaiseFatalErrorImportEvent(object sender, string message, int recordIndex, Exception ex)
		{
			ImportEventArgs args = CreateImportEventArgs(ImportEventType.FatalError, message, recordIndex, ex);
			args.JobRunId = _importContext?.Settings.RunId;
			EventOccurred?.Invoke(sender, args);
		}

		public void RaiseTranserModeChangedEvent(object sender, string message)
		{
			EventOccurred?.Invoke(sender, CreateImportEventArgs(ImportEventType.TransferModeChanged, message));
		}

		public void OnSetJobContext(object sender, ImportContext importContext)
		{
			_importContext = importContext;
		}

		public void RaiseStatusUpdateEvent(object sender, StatusUpdateType type, string msg, int lineNumber)
		{
			UpdateStatus?.Invoke(sender, CreateImportUpdateEventArgs(type, $"{msg} [line {lineNumber}]", lineNumber));
		}

		public void RaiseStatusUpdateEvent(object sender, StatusUpdateType type, string msg)
		{
			UpdateStatus?.Invoke(sender, CreateImportUpdateEventArgs(type, msg, 0));
		}

		public void RaiseCustomStatusUpdateEvent(object sender, StatusUpdateType type, string msg, int lineNumber)
		{
			UpdateStatus?.Invoke(sender, CreateImportUpdateEventArgs(type, msg, lineNumber));
		}
		
		#endregion Interface Methods

		#region Private Methods

		private ImportEventArgs CreateImportEventArgs(ImportEventType type, string msg)
		{
			return new ImportEventArgs(type, msg);
		}

		private ImportEventArgs CreateImportEventArgs(ImportEventType type, string msg, int recordIndex, Exception ex = null)
		{
			return new ImportEventArgs(type, msg, recordIndex, ex);
		}

		private ImportStatusUpdateEventArgs CreateImportUpdateEventArgs(StatusUpdateType type, string msg, int lineNumber)
		{
			return new ImportStatusUpdateEventArgs(type, msg, lineNumber);
		}

		#endregion Private Methods
	}
}
