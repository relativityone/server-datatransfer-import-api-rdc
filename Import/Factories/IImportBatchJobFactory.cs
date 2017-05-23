namespace kCura.WinEDDS.Core.Import.Factories
{
	public interface IImportBatchJobFactory
	{
		IImportBatchJob Create(ImportBatchContext batchContext);
	}
}