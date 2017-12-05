using System.Collections.Generic;
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class ImageExportRequestBuilder : IFileExportRequestBuilder
	{
		private readonly ExportFile _exportSettings;
		private readonly IFilePathProvider _filePathProvider;
		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImageExportRequestBuilder(ExportFile exportSettings, ImageFilePathProvider filePathProvider, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_exportSettings = exportSettings;
			_filePathProvider = filePathProvider;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public IEnumerable<FileExportRequest> Create(ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Creating image files ExportRequests for artifact {artifactId}.", artifact.ArtifactID);
			foreach (var image in artifact.Images.Cast<ImageExportInfo>())
			{
				FileExportRequest exportRequest;
				if (TryCreate(image, out exportRequest))
				{
					yield return exportRequest;
				}
			}
		}

		private bool TryCreate(ImageExportInfo image, out FileExportRequest exportRequest)
		{
			_logger.LogVerbose("Creating image file ExportRequest for image {image}.", image.FileName);
			string destinationLocation = GetExportDestinationLocation(image);

			if (!CanExport(image, destinationLocation))
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

		private bool CanExport(ImageExportInfo image, string destinationLocation)
		{
			if (_fileHelper.Exists(destinationLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_logger.LogVerbose($"Overwriting document {destinationLocation}. Removing already existing file.");
					_fileHelper.Delete(destinationLocation);
					_status.WriteStatusLine(EventType.Status, $"Overwriting image for {image.BatesNumber}.", false);
				}
				else
				{
					_logger.LogVerbose($"{destinationLocation} already exists. Skipping file export.");
					_status.WriteWarning($"{destinationLocation} already exists. Skipping file export.");
					return false;
				}
			}
			return true;
		}
	}
}