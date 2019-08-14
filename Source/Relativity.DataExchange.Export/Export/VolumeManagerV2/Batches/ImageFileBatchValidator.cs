﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using kCura.WinEDDS.Exporters;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.Logging;

	public class ImageFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		public ImageFileBatchValidator(IErrorFileWriter errorFileWriter, IFile fileWrapper, ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileWrapper = fileWrapper;
			_logger = logger;
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
				ValidateAllImages(artifact, images);
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
				_logger.LogError("Image file {file} missing or empty for image {image.BatesNumber} in artifact {artifactId}.", image.TempLocation, image.BatesNumber, artifact.ArtifactID);
				string errorMessage = fileExists ? "File empty." : "File missing.";
				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Image, artifact.IdentifierValue, image.TempLocation, errorMessage);
			}
		}

		private void ValidateAllImages(ObjectExportInfo artifact, List<ImageExportInfo> images)
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