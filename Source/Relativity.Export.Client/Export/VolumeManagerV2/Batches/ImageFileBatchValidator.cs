﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Import.Export.Io;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class ImageFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImageFileBatchValidator(IErrorFileWriter errorFileWriter, IFile fileWrapper, IStatus status, ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileWrapper = fileWrapper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			for (int i = 0; i < artifacts.Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				ValidateImagesForArtifact(artifacts[i], predictions[i]);
			}
		}

		private void ValidateImagesForArtifact(ObjectExportInfo artifact, VolumePredictions volumePredictions)
		{
			if (artifact.ImageCount == 0)
			{
				return;
			}

			List<ImageExportInfo> images = artifact.Images.Cast<ImageExportInfo>().ToList();

			if (images[0].SuccessfulRollup)
			{
				_logger.LogVerbose("Image {image} successfully rollup, so checking single image.", images[0].BatesNumber);
				ValidateSingleImage(artifact, images[0], volumePredictions);
			}
			else
			{
				_logger.LogVerbose("Image {image} wasn't rollup, so checking multiple images.", images[0].BatesNumber);
				ValidateAllImages(artifact, images, volumePredictions);
			}
		}

		private void ValidateSingleImage(ObjectExportInfo artifact, ImageExportInfo image, VolumePredictions predictions)
		{
			if (string.IsNullOrWhiteSpace(image.FileGuid))
			{
				return;
			}

			bool fileExists = _fileWrapper.Exists(image.TempLocation);
			if (!fileExists || _fileWrapper.GetFileSize(image.TempLocation) == 0)
			{
				_logger.LogError("Image file {file} missing or empty for image {image.BatesNumber} in artifact {artifactId}.", image.TempLocation, image.BatesNumber, artifact.ArtifactID);
				string errorMessage = fileExists ? "File empty." : "File missing.";
				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Image, artifact.IdentifierValue, image.TempLocation, errorMessage);
			}
		}

		private void ValidateAllImages(ObjectExportInfo artifact, List<ImageExportInfo> images, VolumePredictions predictions)
		{
			for (int i = 0; i < images.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(images[i].FileGuid))
				{
					continue;
				}

				bool fileExists = _fileWrapper.Exists(images[i].TempLocation);
				if (!fileExists || _fileWrapper.GetFileSize(images[i].TempLocation) == 0)
				{
					_logger.LogWarning("Image file {file} missing or empty for image {image.BatesNumber} in artifact {artifactId}.", images[i].TempLocation, images[i].BatesNumber, artifact.ArtifactID);
					string errorMessage = fileExists ? "File empty." : "File missing.";
					_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Image, artifact.IdentifierValue, images[i].TempLocation, errorMessage);
				}
			}
		}
	}
}