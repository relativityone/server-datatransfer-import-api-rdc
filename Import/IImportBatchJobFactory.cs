
namespace kCura.WinEDDS.Core.Import
{
	public interface IImportBatchJobFactory
	{
		IImportBatchJob Create(ImportBatchContext batchContext);
	}
}
