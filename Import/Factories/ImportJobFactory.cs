using Castle.MicroKernel.Registration;
using Castle.Windsor;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Factories
{
	public class ImportJobFactory : IImportJobFactory
	{
		private readonly ITransferConfig _transferConfig;
		private readonly IImportStatusManager _importStatusManager;
		private readonly IImportBatchJobFactory _batchJobBatchJobFactory;
		private readonly IWindsorContainer _container;
		private readonly IJobFinishStatisticsHandler _jobFinishStatisticsHandler;
		private readonly ILog _log;

		public ImportJobFactory(ITransferConfig transferConfig, IImportStatusManager importStatusManager,
			IImportBatchJobFactory batchJobBatchJobFactory, IWindsorContainer container, IJobFinishStatisticsHandler jobFinishStatisticsHandler, ILog log)
		{
			_transferConfig = transferConfig;
			_importStatusManager = importStatusManager;
			_batchJobBatchJobFactory = batchJobBatchJobFactory;
			_container = container;
			_jobFinishStatisticsHandler = jobFinishStatisticsHandler;
			_log = log;
		}

		public IImportJob Create(IImportMetadata importMetadata, IImporterSettings importerSettings, IImporterManagers importerManagers,
			ICancellationProvider cancellationProvider)
		{
			_container.Register(Component.For<IImporterManagers>().UsingFactoryMethod(k => importerManagers).LifestyleTransient());
			_container.Register(Component.For<IImportMetadata>().UsingFactoryMethod(k => importMetadata).LifestyleTransient());
			_container.Register(Component.For<IImporterSettings>().UsingFactoryMethod(k => importerSettings).LifestyleTransient());

			var importExceptionHandlerExec = _container.Resolve<IImportExceptionHandlerExec>();

			return new ImportJob(_transferConfig, _batchJobBatchJobFactory, _importStatusManager, importMetadata, importerSettings,
				importExceptionHandlerExec, cancellationProvider, _jobFinishStatisticsHandler, _log, _container);
		}
	}
}