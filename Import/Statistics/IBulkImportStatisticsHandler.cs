using System;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface IBulkImportStatisticsHandler
	{
		event EventHandler<BulkImportCompletedEventArgs> BulkImportCompleted;

		event EventHandler<Exception> IoWarningOccurred;

		void RaiseBulkImportCompleted(long time, MassImportResults results);

		void RaiseIoWarning(Exception ex);
	}
}