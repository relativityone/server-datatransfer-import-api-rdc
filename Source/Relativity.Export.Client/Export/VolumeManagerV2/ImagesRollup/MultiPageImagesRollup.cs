﻿namespace Relativity.Export.VolumeManagerV2.ImagesRollup
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exceptions;
	using kCura.WinEDDS.Exporters;

	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	using global::Relativity.Import.Export;

	using Relativity.Import.Export.Media;

	public abstract class MultiPageImagesRollup : IImagesRollup
	{
		private const string _TEMP_FILE_EXTENSION = ".tmp";

		private readonly ExportFile _exportSettings;

		private readonly IFile _fileWrapper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		protected readonly IImage ImageConverter;

		protected MultiPageImagesRollup(ExportFile exportSettings, IFile fileWrapper, IStatus status, ILog logger, IImage imageConverter)
		{
			_exportSettings = exportSettings;
			_fileWrapper = fileWrapper;
			_status = status;
			_logger = logger;
			ImageConverter = imageConverter;
		}

		public void RollupImages(ObjectExportInfo artifact)
		{
			if (artifact.Images.Count == 0)
			{
				_logger.LogVerbose("No images found for artifact {artifactId}. Skipping rollup.", artifact.ArtifactID);
				return;
			}

			var destinationImage = (ImageExportInfo) artifact.Images[0];

			IList<string> imagesLocations = artifact.Images.Cast<ImageExportInfo>().Select(x => x.TempLocation).ToList();

			string rollupTempLocation = GetTempLocation();

			try
			{
				_logger.LogVerbose("Attempting to rollup images in temporary location {tempLocation}. List of images {images}.", rollupTempLocation, string.Join(",", imagesLocations));
				ConvertImage(imagesLocations, rollupTempLocation);

				_logger.LogVerbose("Attempting to delete images.");
				DeleteImages(imagesLocations);

				_logger.LogVerbose("Attempting to update images location.");
				UpdateImageLocation(destinationImage);

				_logger.LogVerbose("Attempting to move temporary image {tempImage} to destination location {destinationLocation}.", rollupTempLocation, destinationImage.TempLocation);
				MoveFileFromTempToDestination(destinationImage, rollupTempLocation);
			}
			catch (ImageRollupException ex)
			{
				HandleImageRollupException(artifact, ex, rollupTempLocation);
				destinationImage.SuccessfulRollup = false;
				return;
			}

			_logger.LogVerbose("Images rollup finished.");
			destinationImage.SuccessfulRollup = true;
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
				_fileWrapper.Delete(imageLocation);
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
			if (_fileWrapper.Exists(image.TempLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_logger.LogVerbose("Overwriting image {image} with image from {tempLocation}.", image.TempLocation, rollupTempLocation);

					_fileWrapper.Delete(image.TempLocation);
					_fileWrapper.Move(rollupTempLocation, image.TempLocation);
				}
				else
				{
					_logger.LogWarning("File {file} exists - skipping. Removing temp file.", image.TempLocation);
					_status.WriteWarning($"File exists - file copy skipped: {image.TempLocation}");
					_fileWrapper.Delete(rollupTempLocation);
				}
			}
			else
			{
				_logger.LogVerbose("Moving file from {tempLocation} to {destinationLocation}.", rollupTempLocation, image.TempLocation);
				_fileWrapper.Move(rollupTempLocation, image.TempLocation);
			}
		}

		private void HandleImageRollupException(ObjectExportInfo artifact, ImageRollupException ex, string rollupTempLocation)
		{
			_logger.LogError(ex, "Error occurred during image rollup.");
			try
			{
				if (!string.IsNullOrEmpty(rollupTempLocation) && _fileWrapper.Exists(rollupTempLocation))
				{
					_logger.LogVerbose("Removing unfinished image {image}.", rollupTempLocation);
					_fileWrapper.Delete(rollupTempLocation);
				}

				_status.WriteImgProgressError(artifact, ex.PageNumber, ex, "Document exported in single-page image mode.");
			}
			catch (IOException ioEx)
			{
				_logger.LogError(ioEx, "Failed to delete image temp file {tempFile}.", rollupTempLocation);
				throw new FileWriteException(FileWriteException.DestinationFile.Errors, ioEx);
			}
		}

		protected abstract string GetExtension();
	}
}