using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class ImageExportRequestBuilder : IFileExportRequestBuilder
	{
		private readonly IFilePathProvider _filePathProvider;
		private readonly ExportFileValidator _validator;
		private readonly ILog _logger;

		public ImageExportRequestBuilder(ImageFilePathProvider filePathProvider, ExportFileValidator validator, ILog logger)
		{
			_filePathProvider = filePathProvider;
			_validator = validator;
			_logger = logger;
		}

		public IList<FileExportRequest> Create(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			var fileExportRequests = new List<FileExportRequest>();
			_logger.LogVerbose("Creating image files ExportRequests for artifact {artifactId}.", artifact.ArtifactID);
			foreach (var image in artifact.Images.Cast<ImageExportInfo>())
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return Enumerable.Empty<FileExportRequest>().ToList();
				}
				FileExportRequest exportRequest;
				if (TryCreate(image, out exportRequest))
				{
					fileExportRequests.Add(exportRequest);
				}
			}
			return fileExportRequests;
		}

		private bool TryCreate(ImageExportInfo image, out FileExportRequest exportRequest)
		{
			if (string.IsNullOrWhiteSpace(image.FileGuid))
			{
				_logger.LogVerbose("Image {imageId} has no GUID so assuming there is nothing to download.", image.ArtifactID);
			}

			_logger.LogVerbose("Creating image file ExportRequest for image {image}.", image.FileName);
			string destinationLocation = GetExportDestinationLocation(image);
			image.TempLocation = destinationLocation;

			string warningInCaseOfOverwriting = $"Overwriting image for {image.BatesNumber}.";

			if (!_validator.CanExport(destinationLocation, warningInCaseOfOverwriting))
			{
				exportRequest = null;
				return false;
			}

			_logger.LogVerbose("Image file will be export to {destinationLocation}.", destinationLocation);

			exportRequest = new NativeFileExportRequest(image, destinationLocation);
			return true;
		}

		private string GetExportDestinationLocation(ImageExportInfo image)
		{
			string fileName = image.FileName;
			return _filePathProvider.GetPathForFile(fileName);
		}
	}
}