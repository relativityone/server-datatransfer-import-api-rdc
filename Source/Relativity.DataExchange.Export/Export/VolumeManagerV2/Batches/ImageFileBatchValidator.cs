namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Resources;
	using Relativity.Logging;

	public class ImageFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		public ImageFileBatchValidator(IErrorFileWriter errorFileWriter, IFile fileWrapper, ILog logger)
		{
			_errorFileWriter = errorFileWriter.ThrowIfNull(nameof(errorFileWriter));
			_fileWrapper = fileWrapper.ThrowIfNull(nameof(fileWrapper));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			for (int i = 0; i < artifacts.Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				ValidateImagesForArtifact(artifacts[i]);
			}
		}

		private void ValidateImagesForArtifact(ObjectExportInfo artifact)
		{
			if (artifact.ImageCount == 0)
			{
				return;
			}

			List<ImageExportInfo> images = artifact.Images.Cast<ImageExportInfo>().ToList();

			if (images[0].SuccessfulRollup)
			{
				_logger.LogVerbose("Image {image} successfully rollup, so checking single image.", images[0].BatesNumber);
				ValidateSingleImage(artifact, images[0]);
			}
			else
			{
				_logger.LogVerbose("Image {image} wasn't rollup, so checking multiple images.", images[0].BatesNumber);
				foreach (ImageExportInfo image in images)
				{
					this.ValidateSingleImage(artifact, image);
				}
			}
		}

		private void ValidateSingleImage(ObjectExportInfo artifact, ImageExportInfo image)
		{
			if (string.IsNullOrWhiteSpace(image.FileGuid))
			{
				return;
			}

			bool fileExists = _fileWrapper.Exists(image.TempLocation);
			if (!fileExists || _fileWrapper.GetFileSize(image.TempLocation) == 0)
			{
				string errorMessage = string.Format(
					CultureInfo.CurrentCulture,
					fileExists ? ExportStrings.FileValidationZeroByteFile : ExportStrings.FileValidationFileMissing,
					artifact.ArtifactID);
				if (string.IsNullOrWhiteSpace(artifact.NativeSourceLocation))
				{
					errorMessage = string.Format(
						CultureInfo.CurrentCulture,
						ExportStrings.FileValidationEmptyRemoteSourcePath,
						artifact.ArtifactID);
					_logger.LogError(
						"Image file remote source path is empty for image artifact {ArtifactId} and suggests a back-end database issue.",
						artifact.ArtifactID);
				}
				else
				{
					this._logger.LogError(
						fileExists
							? "Image file contains zero bytes for for image {BatesNumber} in {ArtifactId}."
							: "Image file is missing for for image {BatesNumber} in {ArtifactId}.",
						image.BatesNumber,
						artifact.ArtifactID);
				}

				_errorFileWriter.Write(
					ErrorFileWriter.ExportFileType.Image,
					artifact,
					image.TempLocation,
					errorMessage);
			}
		}
	}
}