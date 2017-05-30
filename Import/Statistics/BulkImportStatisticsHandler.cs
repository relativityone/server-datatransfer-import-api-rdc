using System;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class BulkImportStatisticsHandler : IBulkImportStatisticsHandler
	{
		public event EventHandler<BulkImportCompletedEventArgs> BulkImportCompleted;

		public void RaiseBulkImportCompleted(long time, MassImportResults results)
		{
			BulkImportCompleted?.Invoke(this, new BulkImportCompletedEventArgs(results, time));
		}
	}
}