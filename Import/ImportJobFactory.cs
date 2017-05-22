
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportJobFactory : IImportJobFactory
	{
		private readonly IImportBatchJobFactory _importBatchJobFactory;

		public ImportJobFactory(IImportBatchJobFactory importBatchJobFactory)
		{
			_importBatchJobFactory = importBatchJobFactory;
		}

		public ImportJob Create(ITransferConfig transferConfig, IImportStatusManager importStatusManager,
			IImportMetadata importMetadata, IImporterSettings importerSettings, IErrorContainer errorContainer)
		{
			return new ImportJob(transferConfig, _importBatchJobFactory, errorContainer, importStatusManager, importMetadata, importerSettings);
		}
	}
}
