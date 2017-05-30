using kCura.EDDS.WebAPI.BulkImportManagerBase;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class BulkImportCompletedEventArgs
	{
		public BulkImportCompletedEventArgs(MassImportResults results, long time)
		{
			Results = results;
			Time = time;
		}

		public MassImportResults Results { get; }
		public long Time { get; }
	}
}