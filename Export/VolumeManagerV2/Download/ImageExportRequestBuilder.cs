using System.Collections.Generic;
using System.Linq;
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

		public IList<FileExportRequest> Create(ObjectExportInfo artifact)
		{
			var fileExportRequests = new List<FileExportRequest>();
			_logger.LogVerbose("Creating image files ExportRequests for artifact {artifactId}.", artifact.ArtifactID);
			foreach (var image in artifact.Images.Cast<ImageExportInfo>())
			{
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
			_logger.LogVerbose("Creating image file ExportRequest for image {image}.", image.FileName);
			string destinationLocation = GetExportDestinationLocation(image);

			string warningInCaseOfOverwriting = $"Overwriting image for {image.BatesNumber}.";
			if (!_validator.CanExport(destinationLocation, warningInCaseOfOverwriting))
			{
				exportRequest = null;
				return false;
			}

			_logger.LogVerbose("Image file will be export to {destinationLocation}.", destinationLocation);
			image.TempLocation = destinationLocation;

			exportRequest = new FileExportRequest(image, destinationLocation);
			return true;
		}

		private string GetExportDestinationLocation(ImageExportInfo image)
		{
			string fileName = image.FileName;
			return _filePathProvider.GetPathForFile(fileName);
		}
	}
}