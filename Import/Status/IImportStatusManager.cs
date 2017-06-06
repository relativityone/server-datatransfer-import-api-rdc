using System;
using kCura.Utility;
using kCura.WinEDDS.Core.Import.Errors;

namespace kCura.WinEDDS.Core.Import.Status
{
	public interface IImportStatusManager
	{
		event EventHandler<ImportEventArgs> EventOccurred;

		event EventHandler<ImportStatusUpdateEventArgs> UpdateStatus;

		event EventHandler<RobustIoReporter.IoWarningEventArgs> IoWarningOccurred;

		void OnSetJobContext(object sender, ImportContext importContext);

		void RaiseStartImportEvent(object sender);

		void RaiseEndImportEvent(object sender);

		void RaiseTransferModeChangedEvent(object sender, string message);

		void RaiseErrorImportEvent(object sender, LineError lineError);

		void RaiseFatalErrorImportEvent(object sender, string message, int recordIndex, Exception ex);

		void RaiseStatusUpdateEvent(object sender, StatusUpdateType type, string msg, int lineNumber);

		void RaiseStatusUpdateEvent(object sender, StatusUpdateType type, string msg);

		void RaiseCustomStatusUpdateEvent(object sender, StatusUpdateType type, string msg, int lineNumber);

		void RaiseIoWarningEvent(object sender, int waitTime, int currentLine, Exception ex);
	}
}