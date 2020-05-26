namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.Logging;

	public class ImageRepositoryBuilder : IRepositoryBuilder
	{
		private readonly IExportRequestBuilder _imageExportRequestBuilder;
		private readonly ImageRepository _imageRepository;
		private readonly ILog _logger;

		public ImageRepositoryBuilder(ImageRepository imageRepository, IExportRequestBuilder imageExportRequestBuilder, ILog logger)
		{
			_imageExportRequestBuilder = imageExportRequestBuilder;
			_logger = logger;
			_imageRepository = imageRepository;
		}

		public void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding {count} images to repository for artifact {artifactId}.", artifact.Images.Count, artifact.ArtifactID);

			IList<ExportRequest> exportRequests = _imageExportRequestBuilder.Create(artifact, cancellationToken);

			_logger.LogVerbose("{count} export request for images found.", exportRequests.Count);

			IList<FileRequest<ImageExportInfo>> images = new List<FileRequest<ImageExportInfo>>();

			foreach (var imageExportInfo in artifact.Images.Cast<ImageExportInfo>())
			{
				ExportRequest imageExportRequest = exportRequests.FirstOrDefault(x => x.DestinationLocation == imageExportInfo.TempLocation);

				_logger.LogVerbose("For image {batesNumber} export request has been found: {result}.", imageExportInfo.BatesNumber, imageExportRequest != null);

				FileRequest<ImageExportInfo> image = new FileRequest<ImageExportInfo>(imageExportInfo)
				{
					ExportRequest = imageExportRequest,
					TransferCompleted = imageExportRequest == null
				};

				images.Add(image);
			}

			_imageRepository.Add(images);
		}
	}
}