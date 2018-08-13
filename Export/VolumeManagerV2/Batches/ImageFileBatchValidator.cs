using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class ImageFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImageFileBatchValidator(IErrorFileWriter errorFileWriter, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			// for (int i = 0; i < artifacts.Length; i++)
			// {
			// 	if (cancellationToken.IsCancellationRequested)
			// 	{
			// 		return;
			// 	}
   //
			// 	ValidateImagesForArtifact(artifacts[i], predictions[i]);
			// }
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
				//_logger.LogVerbose("Image {image} successfully rollup, so checking single image.", images[0].BatesNumber);
				//ValidateSingleImage(artifact, images[0], volumePredictions);
			}
			else
			{
				//_logger.LogVerbose("Image {image} wasn't rollup, so checking multiple images.", images[0].BatesNumber);
				//ValidateAllImages(artifact, images, volumePredictions);
			}
		}

		private void ValidateSingleImage(ObjectExportInfo artifact, ImageExportInfo image, VolumePredictions predictions)
		{
			if (string.IsNullOrWhiteSpace(image.FileGuid))
			{
				return;
			}

			if (!_fileHelper.Exists(image.TempLocation) || _fileHelper.GetFileSize(image.TempLocation) == 0)
			{
				_logger.LogError("Image file {file} missing or empty for image {image.BatesNumber} in artifact {artifactId}.", image.TempLocation, image.BatesNumber, artifact.ArtifactID);
				string errorMessage = _fileHelper.Exists(image.TempLocation) ? "File empty." : "File missing.";
				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Image, artifact.IdentifierValue, image.TempLocation, errorMessage);
			}
			else if (_fileHelper.GetFileSize(image.TempLocation) != predictions.ImageFilesSize)
			{
				long actualFileSize = _fileHelper.GetFileSize(image.TempLocation);
				//_logger.LogWarning("Image file {file} size {actualSize} is different from expected {expectedSize} for image {batesNumber} in artifact {artifactId}.", image.TempLocation,
				//	actualFileSize, predictions.ImageFilesSize, image.BatesNumber, artifact.ArtifactID);
				if (actualFileSize > 0 && actualFileSize < predictions.ImageFilesSize)
				{
					//_status.WriteUpdate(
					//	$"Image file {image.TempLocation} size {actualFileSize} is different from expected {predictions.ImageFilesSize} for image {image.BatesNumber} in artifact {artifact.ArtifactID}, but images have been merged into multi-page format.");
				}
				else
				{
					//_status.WriteWarning(
					//	$"Image file {image.TempLocation} size {actualFileSize} is different from expected {predictions.ImageFilesSize} for image {image.BatesNumber} in artifact {artifact.ArtifactID}.");
				}
			}
		}

		private void ValidateAllImages(ObjectExportInfo artifact, List<ImageExportInfo> images, VolumePredictions predictions)
		{
			bool imageMissing = false;
			for (int i = 0; i < images.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(images[i].FileGuid))
				{
					continue;
				}

				if (!_fileHelper.Exists(images[i].TempLocation) || _fileHelper.GetFileSize(images[i].TempLocation) == 0)
				{
					_logger.LogWarning("Image file {file} missing or empty for image {image.BatesNumber} in artifact {artifactId}.", images[i].TempLocation, images[i].BatesNumber,
						artifact.ArtifactID);
					string errorMessage = _fileHelper.Exists(images[i].TempLocation) ? "File empty." : "File missing.";
					_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Image, artifact.IdentifierValue, images[i].TempLocation, errorMessage);
					imageMissing = true;
				}
			}

			if (imageMissing)
			{
				_logger.LogWarning("One or more images missing - skipping size validation.");
			}
			else
			{
				long imagesSize = images.Where(x => !string.IsNullOrWhiteSpace(x.TempLocation)).Sum(x => _fileHelper.GetFileSize(x.TempLocation));
				if (imagesSize != predictions.ImageFilesSize)
				{
					//_logger.LogWarning("Image files total size {actualSize} is different from expected {expectedSize} for images in artifact {artifactId}.", imagesSize,
					//	predictions.ImageFilesSize, artifact.ArtifactID);
					//_status.WriteWarning($"Image files total size {imagesSize} is different from expected {predictions.ImageFilesSize} for images in artifact {artifact.ArtifactID}.");
				}
			}
		}
	}
}