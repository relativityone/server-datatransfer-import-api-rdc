using System;
using kCura.WinEDDS.Core.Import.Errors;

namespace kCura.WinEDDS.Core.Import.Status
{
	public interface IImportStatusManager
	{
		event EventHandler<ImportEventArgs> EventOccurred;

		event EventHandler<ImportStatusUpdateEventArgs> UpdateStatus;

		void OnSetJobContext(object sender, ImportContext importContext);

		void RaiseStartImportEvent(object sender);

		void RaiseEndImportEvent(object sender);

		void RaiseTranserModeChangedEvent(object sender, string message);

		void RaiseErrorImportEvent(object sender, LineError lineError);

		void RaiseFatalErrorImportEvent(object sender, string message, int recordIndex, Exception ex);

		void RaiseStatusUpdateEvent(object sender, StatusUpdateType type, string msg, int lineNumber);

		void RaiseCustomStatusUpdateEvent(object sender, StatusUpdateType type, string msg, int lineNumber);
	}	
}
