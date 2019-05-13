namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Collections.Generic;
	using System.Threading;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

	using kCura.WinEDDS.Exporters;

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
			_logger.LogDebug("Preparing batch sequentially");

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