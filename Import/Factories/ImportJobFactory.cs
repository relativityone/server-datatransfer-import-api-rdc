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
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IWindsorContainer _container;
		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec;

		public ImportJobFactory(ITransferConfig transferConfig, IImportStatusManager importStatusManager,
			IImportBatchJobFactory batchJobBatchJobFactory, IWindsorContainer container,
			IImportExceptionHandlerExec importExceptionHandlerExec)
		{
			_transferConfig = transferConfig;
			_importStatusManager = importStatusManager;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_container = container;
			_importExceptionHandlerExec = importExceptionHandlerExec;
		}

		public IImportJob Create(IImportMetadata importMetadata, IImporterSettings importerSettings, IImporterManagers importerManagers)
		{
			_container.Register(Component.For<IImporterManagers>().UsingFactoryMethod(k => importerManagers).LifestyleTransient());
			_container.Register(Component.For<IImportMetadata>().UsingFactoryMethod(k => importMetadata).LifestyleTransient());
			_container.Register(Component.For<IImporterSettings>().UsingFactoryMethod(k => importerSettings).LifestyleTransient());

			return new ImportJob(_transferConfig, _batchJobBatchJobFactory, _importStatusManager, importMetadata, importerSettings,
				_importExceptionHandlerExec);
		}
	}
}