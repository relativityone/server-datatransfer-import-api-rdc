using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.Utility;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public abstract class MultiPageImagesRollup : IImagesRollup
	{
		private const string _TEMP_FILE_EXTENSION = ".tmp";

		private readonly ExportFile _exportSettings;

		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		protected readonly Image ImageConverter;

		protected MultiPageImagesRollup(ExportFile exportSettings, IFileHelper fileHelper, IStatus status, ILog logger, Image imageConverter)
		{
			_exportSettings = exportSettings;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
			ImageConverter = imageConverter;
		}

		public bool RollupImages(ObjectExportInfo artifact)
		{
			if (artifact.Images.Count == 0)
			{
				_logger.LogVerbose("No images found for artifact {artifactId}.", artifact.ArtifactID);
				return false;
			}

			IList<string> imagesLocations = artifact.Images.Cast<ImageExportInfo>().Select(x => x.TempLocation).ToList();
			var destinationImage = (ImageExportInfo) artifact.Images[0];

			string rollupTempLocation = GetTempLocation();

			try
			{
				ConvertImage(imagesLocations, rollupTempLocation);

				DeleteImages(imagesLocations);

				UpdateImageLocation(destinationImage);

				MoveFileFromTempToDestination(destinationImage, rollupTempLocation);
			}
			catch (Image.ImageRollupException ex)
			{
				HandleImageRollupException(artifact, ex, rollupTempLocation);
				return false;
			}

			return true;
		}

		private string GetTempLocation()
		{
			string tempFileName = Path.ChangeExtension(Guid.NewGuid().ToString(), _TEMP_FILE_EXTENSION);
			string tempFilePath = Path.Combine(_exportSettings.FolderPath, tempFileName);
			_logger.LogVerbose("Temp file {tempFile} for images rollup created.", tempFilePath);
			return tempFilePath;
		}

		protected abstract void ConvertImage(IList<string> imageList, string tempLocation);

		private void DeleteImages(IEnumerable<string> imageList)
		{
			foreach (var imageLocation in imageList)
			{
				_logger.LogVerbose("Removing image {image}.", imageLocation);
				_fileHelper.Delete(imageLocation);
			}
		}

		private void UpdateImageLocation(ImageExportInfo image)
		{
			string extension = GetExtension();

			image.TempLocation = Path.ChangeExtension(image.TempLocation, extension);
			image.FileName = Path.ChangeExtension(image.FileName, extension);
		}

		private void MoveFileFromTempToDestination(ImageExportInfo image, string rollupTempLocation)
		{
			if (_fileHelper.Exists(image.TempLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_logger.LogVerbose("Overwriting image {image} with image from {tempLocation}.", image.TempLocation, rollupTempLocation);

					_fileHelper.Delete(image.TempLocation);
					_fileHelper.Move(rollupTempLocation, image.TempLocation);
				}
				else
				{
					_logger.LogWarning("File {file} exists - skipping.", image.TempLocation);
					_status.WriteWarning($"File exists - file copy skipped: {image.TempLocation}");
				}
			}
			else
			{
				_logger.LogVerbose("Moving file from {tempLocation} to {destinationLocation}.", rollupTempLocation, image.TempLocation);
				_fileHelper.Move(rollupTempLocation, image.TempLocation);
			}
		}

		private void HandleImageRollupException(ObjectExportInfo artifact, Image.ImageRollupException ex, string rollupTempLocation)
		{
			_logger.LogError(ex, "Error occurred during image rollup.");
			try
			{
				if (!string.IsNullOrEmpty(rollupTempLocation) && _fileHelper.Exists(rollupTempLocation))
				{
					_logger.LogVerbose("Removing unfinished image {image}.", rollupTempLocation);
					_fileHelper.Delete(rollupTempLocation);
				}
				_status.WriteImgProgressError(artifact, ex.ImageIndex, ex, "Document exported in single-page image mode.");
			}
			catch (IOException ioEx)
			{
				_logger.LogError(ioEx, "Failed to delete image temp file.");
				throw new FileWriteException(FileWriteException.DestinationFile.Errors, ioEx);
			}
		}

		protected abstract string GetExtension();
	}
}