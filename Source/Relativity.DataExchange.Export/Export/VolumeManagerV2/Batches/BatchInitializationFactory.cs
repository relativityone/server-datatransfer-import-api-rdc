namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Collections.Generic;

	using Castle.Windsor;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Container;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

	public class BatchInitializationFactory
	{
		private readonly ILog _logger;

		public BatchInitializationFactory(ILog logger)
		{
			_logger = logger;
		}

		public IBatchInitialization Create(ExportFile exportSettings, IExportConfig exportConfig, IWindsorContainer container)
		{
			IList<IRepositoryBuilder> repositoryBuilders = new List<IRepositoryBuilder>();

			repositoryBuilders.Add(container.Resolve<LongTextRepositoryBuilderFactory>().Create(exportSettings, container));
			repositoryBuilders.Add(container.Resolve<NativeRepositoryBuilderFactory>().Create(exportSettings, container));
			repositoryBuilders.Add(container.Resolve<ImageRepositoryBuilderFactory>().Create(exportSettings, container));
			repositoryBuilders.Add(new FileRepositoryBuilder(
				container.Resolve<FileRequestRepository>(ExportInstaller.GetServiceNameByExportType(typeof(FileRequestRepository), ExportFileTypes.Pdf)), 
				container.Resolve<ILabelManagerForArtifact>(), 
				container.Resolve<IExportRequestBuilder>(ExportInstaller.GetServiceNameByExportType(typeof(IExportRequestBuilder), ExportFileTypes.Pdf)),
				_logger));

			IDirectoryManager directoryManager = container.Resolve<IDirectoryManager>();

			return new BatchInitialization(repositoryBuilders, directoryManager, _logger);
			
		}
	}
}