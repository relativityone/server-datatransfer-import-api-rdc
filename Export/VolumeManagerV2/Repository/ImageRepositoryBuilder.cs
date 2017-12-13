using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class ImageRepositoryBuilder
	{
		private readonly IFileExportRequestBuilder _imageExportRequestBuilder;
		private readonly ImageRepository _imageRepository;
		private readonly ILog _logger;

		public ImageRepositoryBuilder(ImageRepository imageRepository, IFileExportRequestBuilder imageExportRequestBuilder, ILog logger)
		{
			_imageExportRequestBuilder = imageExportRequestBuilder;
			_logger = logger;
			_imageRepository = imageRepository;
		}

		public void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding {count} images to repository for artifact {artifactId}.", artifact.Images.Count, artifact.ArtifactID);

			IList<FileExportRequest> exportRequests = _imageExportRequestBuilder.Create(artifact, cancellationToken);

			_logger.LogVerbose("{count} export request for images found.", exportRequests.Count);

			IList<Image> images = new List<Image>();

			foreach (var imageExportInfo in artifact.Images.Cast<ImageExportInfo>())
			{
				FileExportRequest imageExportRequest = exportRequests.FirstOrDefault(x => x.DestinationLocation == imageExportInfo.TempLocation);

				_logger.LogVerbose("For image {batesNumber} export request has been found: {result}.", imageExportInfo.BatesNumber, imageExportRequest != null);

				Image image = new Image(imageExportInfo)
				{
					ExportRequest = imageExportRequest,
					HasBeenDownloaded = imageExportRequest == null
				};

				images.Add(image);
			}

			_imageRepository.Add(images);
		}
	}
}