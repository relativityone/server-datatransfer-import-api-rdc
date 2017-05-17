using System;

namespace kCura.WinEDDS.Core.Import.Status
{
	public interface IImportStatusManager
	{
		event EventHandler<ImportStatusEventArgs> StatusChanged;

		void OnSetJobContext(object sender, ImportContext importContext);

		void ReiseStatusChangedEvent(object sender, ImportStatusEventArgs args);

		void RaiseStartImportEvent(object sender);

		void RaiseEndImportEvent(object sender);

		void ReiseWarningImportEvent(object sender, string message, int recordIndex);

		void RaiseTranserModeChangedEvent(object sender, string message);

		void RaiseUpdateImportEvent(object sender, string message, int recordIndex);

		void RaiseErrorImportEvent(object sender, string message, int recordIndex);

		void RaiseFatalErrorImportEvent(object sender, string message, int recordIndex, Exception ex);
	}	
}
