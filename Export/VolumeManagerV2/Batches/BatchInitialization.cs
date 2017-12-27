using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchInitialization : IBatchInitialization
	{
		private readonly IList<IRepositoryBuilder> _repositoryBuilders;
		private readonly IDirectoryManager _directoryManager;
		private readonly ILog _logger;

		public BatchInitialization(IList<IRepositoryBuilder> repositoryBuilders, IDirectoryManager directoryManager, ILog logger)
		{
			_repositoryBuilders = repositoryBuilders;
			_directoryManager = directoryManager;
			_logger = logger;
		}

		public void PrepareBatch(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			for (int i = 0; i < artifacts.Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_directoryManager.MoveNext(volumePredictions[i]);

				_logger.LogVerbose("Adding artifact {artifactId} to repositories.", artifacts[i].ArtifactID);
				PrepareArtifact(artifacts[i], cancellationToken);
			}
		}

		private void PrepareArtifact(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			foreach (IRepositoryBuilder repositoryBuilder in _repositoryBuilders)
			{
				_logger.LogVerbose("Adding artifact {artifactId} to repository {type}.", repositoryBuilder.GetType().ToString());
				repositoryBuilder.AddToRepository(artifact, cancellationToken);
			}
		}
	}
}