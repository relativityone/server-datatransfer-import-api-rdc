using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class ImageRepositoryBuilder
	{
		private readonly IFileExportRequestBuilder _imageExportRequestBuilder;
		private readonly ImageRepository _imageRepository;

		public ImageRepositoryBuilder(ImageRepository imageRepository, IFileExportRequestBuilder imageExportRequestBuilder)
		{
			_imageExportRequestBuilder = imageExportRequestBuilder;
			_imageRepository = imageRepository;
		}

		public void AddToRepository(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			IList<FileExportRequest> exportRequests = _imageExportRequestBuilder.Create(artifact, cancellationToken);

			IList<Image> images = new List<Image>();

			foreach (var imageExportInfo in artifact.Images.Cast<ImageExportInfo>())
			{
				FileExportRequest imageExportRequest = exportRequests.FirstOrDefault(x => x.DestinationLocation == imageExportInfo.TempLocation);

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