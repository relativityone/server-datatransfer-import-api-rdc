using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class NativeRepositoryBuilder : IRepositoryBuilder
	{
		private readonly NativeRepository _nativeRepository;
		private readonly ILabelManagerForArtifact _labelManagerForArtifact;
		private readonly IExportRequestBuilder _fileExportRequestBuilder;
		private readonly ILog _logger;

		public NativeRepositoryBuilder(NativeRepository nativeRepository,
			ILabelManagerForArtifact labelManagerForArtifact, IExportRequestBuilder fileExportRequestBuilder,
			ILog logger)
		{
			_nativeRepository = nativeRepository;
			_labelManagerForArtifact = labelManagerForArtifact;
			_fileExportRequestBuilder = fileExportRequestBuilder;
			_logger = logger;
		}

		public void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding artifact {artifactId} to repository.", artifact.ArtifactID);

			artifact.DestinationVolume = _labelManagerForArtifact.GetVolumeLabel(artifact.ArtifactID);
			_logger.LogVerbose("Current volume set to {volume}.", artifact.DestinationVolume);

			IList<ExportRequest> exportRequests = _fileExportRequestBuilder.Create(artifact, cancellationToken);

			_logger.LogVerbose("{count} export request for natives found.", exportRequests.Count);

			var native = new Native(artifact)
			{
				ExportRequest = exportRequests.FirstOrDefault(),
				HasBeenDownloaded = exportRequests.Count == 0
			};

			_nativeRepository.Add(native);
		}
	}
}