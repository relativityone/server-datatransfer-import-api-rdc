using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public abstract class ImageLoadFileMetadataBuilder
	{
		private IList<KeyValuePair<string, string>> _lines;

		private readonly IImageLoadFileEntry _imageLoadFileEntry;
		private readonly ExportFile _exportSettings;
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly IFullTextLoadFileEntry _fullTextLoadFileEntry;
		private readonly ILog _logger;

		protected ImageLoadFileMetadataBuilder(ExportFile exportSettings, IFilePathTransformer filePathTransformer, IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry, ILog logger)
		{
			_exportSettings = exportSettings;
			_filePathTransformer = filePathTransformer;
			_imageLoadFileEntry = imageLoadFileEntry;
			_fullTextLoadFileEntry = fullTextLoadFileEntry;
			_logger = logger;
		}

		public IList<KeyValuePair<string, string>> CreateLoadFileEntries(ObjectExportInfo[] artifacts)
		{
			_logger.LogVerbose("Creating metadata for image load file for current batch.");

			_lines = new List<KeyValuePair<string, string>>();

			foreach (var artifact in artifacts)
			{
				_logger.LogVerbose("Creating image load file entry for artifact {artifactId}.", artifact.ArtifactID);
				CreateLoadFileEntry(artifact);
			}

			_logger.LogVerbose("Successfully create metadata for images.");
			return _lines;
		}

		protected abstract List<ImageExportInfo> GetImagesToProcess(ObjectExportInfo artifact);

		private void CreateLoadFileEntry(ObjectExportInfo artifact)
		{
			int numberOfPages = artifact.Images.Count;
			List<ImageExportInfo> images = GetImagesToProcess(artifact);

			_logger.LogVerbose("Number of pages in image {numberOfPages}. Actual number of images to process {imagesCount}.", numberOfPages, images.Count);

			for (int i = 0; i < images.Count; i++)
			{
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
				KeyValuePair<string, string> fullTextEntry;
				if (_fullTextLoadFileEntry.TryCreateFullTextLine(artifact, image.BatesNumber, i, pageOffset, out fullTextEntry))
				{
					_logger.LogVerbose("Full text entry for image created.");
					_lines.Add(fullTextEntry);
				}
				else
				{
					_logger.LogVerbose("Full text entry not required for this image.");
				}

				string localFilePath = GetLocalFilePath(images, i);
				_logger.LogVerbose("Creating image load file entry using image file path {path}.", localFilePath);
				KeyValuePair<string, string> loadFileEntry = _imageLoadFileEntry.Create(image.BatesNumber, localFilePath, artifact.DestinationVolume, i + 1, pageOffset, numberOfPages);
				_lines.Add(loadFileEntry);
			}
		}

		private string GetLocalFilePath(List<ImageExportInfo> images, int i)
		{
			int baseImageIndex = GetBaseImageIndex(i);
			string localFilePath = images[baseImageIndex].SourceLocation;
			if (_exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				localFilePath = _filePathTransformer.TransformPath(images[baseImageIndex].TempLocation);
			}
			return localFilePath;
		}

		protected abstract int GetBaseImageIndex(int i);
	}
}