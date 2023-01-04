// ----------------------------------------------------------------------------
// <copyright file="PdfRepositoryBuilder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.Logging;

	public class FileRepositoryBuilder : IRepositoryBuilder
	{
		private readonly FileRequestRepository _fileRepository;
		private readonly ILabelManagerForArtifact _labelManagerForArtifact;
		private readonly IExportRequestBuilder _fileExportRequestBuilder;
		private readonly ILog _logger;

		public FileRepositoryBuilder(
			FileRequestRepository fileRepository,
			ILabelManagerForArtifact labelManagerForArtifact,
			IExportRequestBuilder fileExportRequestBuilder,
			ILog logger)
		{
			this._fileRepository = fileRepository;
			this._labelManagerForArtifact = labelManagerForArtifact;
			this._fileExportRequestBuilder = fileExportRequestBuilder;
			this._logger = logger;
		}

		public void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding artifact {artifactId} to repository.", artifact.ArtifactID);

			artifact.DestinationVolume = _labelManagerForArtifact.GetVolumeLabel(artifact.ArtifactID);
			_logger.LogVerbose("Current volume set to {volume}.", artifact.DestinationVolume);

			IList<ExportRequest> exportRequests = _fileExportRequestBuilder.Create(artifact, cancellationToken);

			_logger.LogVerbose("{count} export request for the file found.", exportRequests.Count);

			var file = new FileRequest<ObjectExportInfo>(artifact)
			{
				ExportRequest = exportRequests.FirstOrDefault(),
				TransferCompleted = exportRequests.Count == 0
			};

			this._fileRepository.Add(file);
		}
	}
}