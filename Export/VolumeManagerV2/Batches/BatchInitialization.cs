using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchInitialization : IBatchInitialization
	{
		//TODO maybe extract common interface and use list injection. order of execution should matter
		private readonly LongTextRepositoryBuilder _longTextRepositoryBuilder;
		private readonly NativeRepositoryBuilder _nativeRepositoryBuilder;
		private readonly ImageRepositoryBuilder _imageRepositoryBuilder;
		private readonly IDirectoryManager _directoryManager;
		private readonly ILog _logger;

		public BatchInitialization(LongTextRepositoryBuilder longTextRepositoryBuilder, NativeRepositoryBuilder nativeRepositoryBuilder, ImageRepositoryBuilder imageRepositoryBuilder,
			IDirectoryManager directoryManager,ILog logger)
		{
			_longTextRepositoryBuilder = longTextRepositoryBuilder;
			_nativeRepositoryBuilder = nativeRepositoryBuilder;
			_imageRepositoryBuilder = imageRepositoryBuilder;
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
			FillNativeRepository(artifact, cancellationToken);
			FillImageRepository(artifact, cancellationToken);
			FillLongTextRepository(artifact, cancellationToken);
		}

		private void FillNativeRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding artifact {artifactId} to NativeRepository.", artifact.ArtifactID);
			_nativeRepositoryBuilder.AddToRepository(artifact, cancellationToken);
		}

		private void FillImageRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding artifact {artifactId} to ImageRepository.", artifact.ArtifactID);
			_imageRepositoryBuilder.AddToRepository(artifact, cancellationToken);
		}

		private void FillLongTextRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding artifact {artifactId} to LongTextRepository.", artifact.ArtifactID);
			_longTextRepositoryBuilder.AddToRepository(artifact, cancellationToken);
		}
	}
}