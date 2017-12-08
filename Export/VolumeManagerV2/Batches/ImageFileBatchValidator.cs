using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class ImageFileBatchValidator : IBatchValidator
	{
		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImageFileBatchValidator(IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_fileHelper = fileHelper;
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
			List<ImageExportInfo> images = artifact.Images.Cast<ImageExportInfo>().ToList();
			if (images.Count == 0)
			{
				return;
			}

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
			if (!_fileHelper.Exists(image.TempLocation))
			{
				_logger.LogWarning("Image file {file} missing for image {image.BatesNumber} in artifact {artifactId}.", image.TempLocation, image.BatesNumber, artifact.ArtifactID);
				_status.WriteWarning($"Image file {image.TempLocation} missing for image {image.BatesNumber} in artifact {artifact.ArtifactID}.");
			}
			else if (_fileHelper.GetFileSize(image.TempLocation) != predictions.ImageFilesSize)
			{
				long actualFileSize = _fileHelper.GetFileSize(image.TempLocation);
				_logger.LogWarning("Image file {file} size {actualSize} is different from expected {expectedSize} for image {batesNumber} in artifact {artifactId}.", image.TempLocation,
					actualFileSize, predictions.ImageFilesSize, image.BatesNumber, artifact.ArtifactID);
				if (actualFileSize > 0 && actualFileSize < predictions.ImageFilesSize)
				{
					_status.WriteUpdate(
						$"Image file {image.TempLocation} size {actualFileSize} is different from expected {predictions.ImageFilesSize} for image {image.BatesNumber} in artifact {artifact.ArtifactID}, but images have been merged into multipage format.");
				}
				else
				{
					_status.WriteWarning(
						$"Image file {image.TempLocation} size {actualFileSize} is different from expected {predictions.ImageFilesSize} for image {image.BatesNumber} in artifact {artifact.ArtifactID}.");
				}
			}
		}

		private void ValidateAllImages(ObjectExportInfo artifact, List<ImageExportInfo> images, VolumePredictions predictions)
		{
			bool imageMissing = false;
			for (int i = 0; i < images.Count; i++)
			{
				if (!_fileHelper.Exists(images[i].TempLocation))
				{
					_logger.LogWarning("Image file {file} missing for image {image.BatesNumber} in artifact {artifactId}.", images[i].TempLocation, images[i].BatesNumber, artifact.ArtifactID);
					_status.WriteWarning($"Image file {images[i].TempLocation} missing for image {images[i].BatesNumber} in artifact {artifact.ArtifactID}.");
					imageMissing = true;
				}
			}
			if (imageMissing)
			{
				_logger.LogWarning("One or more images missing - skipping size validation.");
			}
			else
			{
				long imagesSize = images.Sum(x => _fileHelper.GetFileSize(x.TempLocation));
				if (imagesSize != predictions.ImageFilesSize)
				{
					_logger.LogWarning("Image files total size {actualSize} is different from expected {expectedSize} for images in artifact {artifactId}.", imagesSize,
						predictions.ImageFilesSize, artifact.ArtifactID);
					_status.WriteWarning($"Image files total size {imagesSize} is different from expected {predictions.ImageFilesSize} for images in artifact {artifact.ArtifactID}.");
				}
			}
		}
	}
}