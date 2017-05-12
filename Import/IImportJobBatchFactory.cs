
namespace kCura.WinEDDS.Core.Import
{
	public interface IImportJobBatchFactory
	{
		IImportBatchJob Create(ImportBatchContext batchContext);
	}
}
