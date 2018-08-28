using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class ImageFileParallelBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		public ImageFileParallelBatchValidator(IErrorFileWriter errorFileWriter, IFileHelper fileHelper, ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			ParallelQuery<Action> validationResults = Enumerable.Range(0, artifacts.Length)
				.AsParallel()
				.AsOrdered()
				.Select(i => ValidateImagesForArtifact(artifacts[i], cancellationToken));

			foreach (var validationAction in validationResults.Where(result => result != null))
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				validationAction();
			}
		}

		private Action ValidateImagesForArtifact(ObjectExportInfo artifact, CancellationToken cancellationToken)
		{
			if (artifact.ImageCount == 0 || cancellationToken.IsCancellationRequested)
			{
				return null;
			}

			List<ImageExportInfo> images = artifact.Images.Cast<ImageExportInfo>().ToList();

			if (images[0].SuccessfulRollup)
			{
				_logger.LogVerbose("Image {image} successfully rollup, so checking single image.", images[0].BatesNumber);
				return ValidateSingleImage(artifact, images[0], cancellationToken);
			}
			else
			{
				_logger.LogVerbose("Image {image} wasn't rollup, so checking multiple images.", images[0].BatesNumber);
				return ValidateAllImages(artifact, images, cancellationToken);
			}
		}

		private Action ValidateSingleImage(ObjectExportInfo artifact, ImageExportInfo image, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(image.FileGuid) || cancellationToken.IsCancellationRequested)
			{
				return null;
			}

			bool imageFileExists = _fileHelper.Exists(image.TempLocation);
			if (!imageFileExists || _fileHelper.GetFileSize(image.TempLocation) == 0)
			{
				return GetFileMissingAction(artifact, image, imageFileExists);
			}

			return null;
		}

		private Action ValidateAllImages(ObjectExportInfo artifact, List<ImageExportInfo> images, CancellationToken cancellationToken)
		{
			Action actions = null;

			foreach (ImageExportInfo image in images)
			{
				Action imageValidationResult = ValidateSingleImage(artifact, image, cancellationToken);
				actions = ConcatActions(actions, imageValidationResult);
			}

			return actions;
		}

		private Action GetFileMissingAction(ObjectExportInfo artifact, ImageExportInfo image, bool imageFileExists)
		{
			return () =>
			{
				_logger.LogError(
					"Image file {file} missing or empty for image {image.BatesNumber} in artifact {artifactId}.",
					image.TempLocation, image.BatesNumber, artifact.ArtifactID);
				string errorMessage = imageFileExists ? "File empty." : "File missing.";
				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Image, artifact.IdentifierValue,
					image.TempLocation, errorMessage);
			};
		}

		private Action ConcatActions(Action parent, Action child)
		{
			if (parent == null)
			{
				return child;
			}

			if (child == null)
			{
				return parent;
			}

			return parent + child;
		}
	}
}
