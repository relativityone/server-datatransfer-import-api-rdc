namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images
{
	using System;
	using System.Collections.Generic;
	using System.Threading;

	using Relativity.Logging;
	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;

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

			var autoGeneratePageNumbers = AutoGeneratePageNumbers(images);

			for (int i = 0; i < images.Count; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				ImageExportInfo image = images[i];

				_logger.LogVerbose("Processing image {image}.", image.FileName);

				long pageOffset;
				int currPageNumber = i + 1;
				if (i == 0 && image.PageOffset == null || i == images.Count - 1)
				{
					pageOffset = long.MinValue;
				}
				else
				{
					ImageExportInfo nextImage = images[currPageNumber];
					pageOffset = nextImage.PageOffset ?? long.MinValue;
				}
				_logger.LogVerbose("Attempting to create full text entry for image.");
				_fullTextLoadFileEntry.WriteFullTextLine(artifact, image.BatesNumber, i, pageOffset, writer, cancellationToken);

				string localFilePath = GetLocalFilePath(images, i);
				_logger.LogVerbose("Creating image load file entry using image file path {path}.", localFilePath);
				string loadFileEntry = _imageLoadFileEntry.Create(this.CreateUniqueBates(image.BatesNumber, currPageNumber, autoGeneratePageNumbers), 
					localFilePath, artifact.DestinationVolume, currPageNumber, numberOfPages);
				writer.WriteEntry(loadFileEntry, cancellationToken);
			}
		}

		protected abstract List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact);

		private bool AutoGeneratePageNumbers(List<ImageExportInfo> images)
		{
			// If Production generates Images without Page Number than we will get the same Bates Number for each image from the server.
			// On the other hand we still need to provide unique values for each entry in Opticon file
			return images.Count > 1 && string.Compare(images[0].BatesNumber, images[1].BatesNumber, StringComparison.InvariantCultureIgnoreCase) == 0;
		}

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

		private string CreateUniqueBates(string batesNumber, int pageNumber, bool autoGenerateNumbers)
		{
			if (autoGenerateNumbers && pageNumber > 1)
			{
				return $"{batesNumber}_{pageNumber}";
			}
			return batesNumber;
		}

		protected abstract int GetBaseImageIndex(int i);
	}
}