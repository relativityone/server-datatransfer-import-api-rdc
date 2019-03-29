﻿namespace Relativity.Export.VolumeManagerV2.Metadata.Images
{
	using System.Collections.Generic;
	using System.Threading;

	using Relativity.Logging;
	using Relativity.Export.VolumeManagerV2.Directories;
	using Relativity.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.Export.VolumeManagerV2.Metadata.Writers;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS;

	public abstract class ImageLoadFileMetadataForArtifactBuilder : IImageLoadFileMetadataForArtifactBuilder
	{
		private readonly IImageLoadFileEntry _imageLoadFileEntry;
		private readonly ExportFile _exportSettings;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly IFullTextLoadFileEntry _fullTextLoadFileEntry;
		private readonly ILog _logger;

		protected ImageLoadFileMetadataForArtifactBuilder(ExportFile exportSettings, IFilePathTransformer filePathTransformer, IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry, ILog logger)
		{
			_exportSettings = exportSettings;
			_filePathTransformer = filePathTransformer;
			_imageLoadFileEntry = imageLoadFileEntry;
			_fullTextLoadFileEntry = fullTextLoadFileEntry;
			_logger = logger;
		}

		public void WriteLoadFileEntry(ObjectExportInfo artifact, IRetryableStreamWriter writer, CancellationToken cancellationToken)
		{
			int numberOfPages = artifact.Images.Count;
			List<ImageExportInfo> images = GetImagesToProcess(artifact);

			_logger.LogVerbose("Number of pages in image {numberOfPages}. Actual number of images to process {imagesCount}.", numberOfPages, images.Count);

			for (int i = 0; i < images.Count; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				ImageExportInfo image = images[i];

				_logger.LogVerbose("Processing image {image}.", image.FileName);

				long pageOffset;
				if (i == 0 && image.PageOffset == null || i == images.Count - 1)
				{
					pageOffset = long.MinValue;
				}
				else
				{
					ImageExportInfo nextImage = images[i + 1];
					pageOffset = nextImage.PageOffset ?? long.MinValue;
				}

				_logger.LogVerbose("Attempting to create full text entry for image.");
				_fullTextLoadFileEntry.WriteFullTextLine(artifact, image.BatesNumber, i, pageOffset, writer, cancellationToken);

				string localFilePath = GetLocalFilePath(images, i);
				_logger.LogVerbose("Creating image load file entry using image file path {path}.", localFilePath);
				string loadFileEntry = _imageLoadFileEntry.Create(image.BatesNumber, localFilePath, artifact.DestinationVolume, i + 1, numberOfPages);
				writer.WriteEntry(loadFileEntry, cancellationToken);
			}
		}

		protected abstract List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact);

		private string GetLocalFilePath(List<ImageExportInfo> images, int i)
		{
			int baseImageIndex = GetBaseImageIndex(i);
			string localFilePath = images[baseImageIndex].SourceLocation;
			if (_exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				localFilePath = string.IsNullOrWhiteSpace(images[baseImageIndex].TempLocation)
					? images[baseImageIndex].TempLocation
					: _filePathTransformer.TransformPath(images[baseImageIndex].TempLocation);
			}

			return localFilePath;
		}

		protected abstract int GetBaseImageIndex(int i);
	}
}