using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class ParallelBatchInitialization : IBatchInitialization
	{
		private readonly IList<IRepositoryBuilder> _repositoryBuilders;
		private readonly ILabelManagerForArtifact _labelManagerForArtifact;
		private readonly ILog _logger;

		public ParallelBatchInitialization(IList<IRepositoryBuilder> repositoryBuilders, ILabelManagerForArtifact labelManagerForArtifact, ILog logger)
		{
			_repositoryBuilders = repositoryBuilders;
			_labelManagerForArtifact = labelManagerForArtifact;
			_logger = logger;
		}

		public void PrepareBatch(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_labelManagerForArtifact.InitializeFor(artifacts, volumePredictions, cancellationToken);

			PrepareArtifactsInParallel(artifacts, cancellationToken);
		}

		private void PrepareArtifactsInParallel(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			_logger.LogDebug("Preparing batch in parallel");

			artifacts.AsParallel().ForAll(artifact =>
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}

					_logger.LogVerbose("Adding artifact {artifactId} to repositories.", artifact.ArtifactID);
					PrepareArtifact(artifact, cancellationToken);
				}
			);
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