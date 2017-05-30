using System;
using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public interface IBulkImportStatisticsHandler
	{
		event EventHandler<BulkImportCompletedEventArgs> BulkImportCompleted;

		void RaiseBulkImportCompleted(long time, MassImportResults results);
	}
}