
using kCura.WinEDDS.Core.Import.Errors;

namespace kCura.WinEDDS.Core.Import
{
	public interface IImportBatchJobFactory
	{
		IImportBatchJob Create(ImportBatchContext batchContext, IImportMetadata importMetadata, IImporterSettings importerSettings);
	}
}
