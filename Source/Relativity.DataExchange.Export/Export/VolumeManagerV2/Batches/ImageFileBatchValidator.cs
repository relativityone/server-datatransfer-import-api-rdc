namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Resources;
	using Relativity.Logging;

	public class ImageFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImageFileBatchValidator(IErrorFileWriter errorFileWriter, IFile fileWrapper, IStatus status, ILog logger)
		{
			_errorFileWriter = errorFileWriter.ThrowIfNull(nameof(errorFileWriter));
			_fileWrapper = fileWrapper.ThrowIfNull(nameof(fileWrapper));
			_status = status.ThrowIfNull(nameof(status));
			_logger = logger.ThrowIfNull(nameof(logger));
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
				foreach (ImageExportInfo image in images)
				{
					this.ValidateSingleImage(artifact, image, volumePredictions);
				}
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
				string errorMessage = fileExists ? ExportStrings.FileValidationZeroByteFile : ExportStrings.FileValidationFileMissing;
				if (string.IsNullOrWhiteSpace(artifact.NativeSourceLocation))
				{
					errorMessage = ExportStrings.FileValidationEmptyRemoteSourcePath;
					_logger.LogError(
						"Image file {File} remote source path is empty for image artifact {ArtifactId} and suggests a back-end database issue.",
						artifact.NativeTempLocation,
						artifact.ArtifactID);
				}
				else
				{
					_logger.LogError(
						"Image file {File} missing or empty for image {BatesNumber} in artifact {ArtifactId}.",
						image.TempLocation,
						image.BatesNumber,
						artifact.ArtifactID);
				}

				_errorFileWriter.Write(
					ErrorFileWriter.ExportFileType.Image,
					artifact.IdentifierValue,
					image.TempLocation,
					errorMessage);
			}
		}
	}
}