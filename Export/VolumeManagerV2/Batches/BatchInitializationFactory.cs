using System.Collections.Generic;
using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchInitializationFactory
	{
		private readonly ILog _logger;

		public BatchInitializationFactory(ILog logger)
		{
			_logger = logger;
		}

		public IBatchInitialization Create(ExportFile exportSettings, IWindsorContainer container)
		{
			IList<IRepositoryBuilder> repositoryBuilders = new List<IRepositoryBuilder>();

			repositoryBuilders.Add(container.Resolve<LongTextRepositoryBuilderFactory>().Create(exportSettings, container));
			repositoryBuilders.Add(container.Resolve<NativeRepositoryBuilderFactory>().Create(exportSettings, container));
			repositoryBuilders.Add(container.Resolve<ImageRepositoryBuilderFactory>().Create(exportSettings, container));

			IDirectoryManager directoryManager = container.Resolve<IDirectoryManager>();

			return new BatchInitialization(repositoryBuilders, directoryManager, _logger);
		}
	}
}