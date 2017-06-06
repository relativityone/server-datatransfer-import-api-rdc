using System;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class BulkImportStatisticsHandler : IBulkImportStatisticsHandler
	{
		public event EventHandler<BulkImportCompletedEventArgs> BulkImportCompleted;
		public event EventHandler<Exception> IoWarningOccurred;

		public void RaiseBulkImportCompleted(long time, MassImportResults results)
		{
			BulkImportCompleted?.Invoke(this, new BulkImportCompletedEventArgs(results, time));
		}

		public void RaiseIoWarning(Exception ex)
		{
			IoWarningOccurred?.Invoke(this, ex);
		}
	}
}