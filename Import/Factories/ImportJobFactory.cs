using Castle.MicroKernel.Registration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import.Factories
{
	public class ImportJobFactory : IImportJobFactory
	{
		private readonly ITransferConfig _transferConfig;
		private readonly IImportStatusManager _importStatusManager;
		private readonly IErrorContainer _errorContainer;
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IWindsorContainer _container;

		public ImportJobFactory(ITransferConfig transferConfig, IImportStatusManager importStatusManager, IErrorContainer errorContainer,
			IImportBatchJobFactory batchJobBatchJobFactory, IWindsorContainer container)
		{
			_transferConfig = transferConfig;
			_importStatusManager = importStatusManager;
			_errorContainer = errorContainer;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_container = container;
		}

		public IImportJob Create(IImportMetadata importMetadata, IImporterSettings importerSettings, IImporterManagers importerManagers)
		{
			_container.Register(Component.For<IImporterManagers>().UsingFactoryMethod(k => importerManagers).LifestyleTransient());
			_container.Register(Component.For<IImportMetadata>().UsingFactoryMethod(k => importMetadata).LifestyleTransient());
			_container.Register(Component.For<IImporterSettings>().UsingFactoryMethod(k => importerSettings).LifestyleTransient());

			return new ImportJob(_transferConfig, _batchJobBatchJobFactory, _errorContainer, _importStatusManager, importMetadata, importerSettings);
		}
	}
}