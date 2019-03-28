using System;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileMetadataForArtifactBuilderFactory
	{
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly IImageLoadFileEntry _imageLoadFileEntry;
		private readonly IFullTextLoadFileEntry _fullTextLoadFileEntry;
		private readonly ILog _logger;

		public ImageLoadFileMetadataForArtifactBuilderFactory(IFilePathTransformer filePathTransformer, IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry, ILog logger)
		{
			_filePathTransformer = filePathTransformer;
			_imageLoadFileEntry = imageLoadFileEntry;
			_fullTextLoadFileEntry = fullTextLoadFileEntry;
			_logger = logger;
		}

		public IImageLoadFileMetadataForArtifactBuilder Create(ExportFile exportSettings)
		{
			if (exportSettings.TypeOfImage == ExportFile.ImageType.SinglePage)
			{
				return new SinglePageMetadataForArtifactBuilder(exportSettings, _filePathTransformer, _imageLoadFileEntry, _fullTextLoadFileEntry, _logger);
			}

			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				return new MultiPageOpticonMetadataForArtifactBuilder(exportSettings, _filePathTransformer, _imageLoadFileEntry, _fullTextLoadFileEntry, _logger);
			}

			if ((exportSettings.TypeOfImage == ExportFile.ImageType.MultiPageTiff || exportSettings.TypeOfImage == ExportFile.ImageType.Pdf) &&
				(exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO || exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText))
			{
				return new MultiPageNotOpticonMetadataForArtifactBuilder(exportSettings, _filePathTransformer, _imageLoadFileEntry, _fullTextLoadFileEntry, _logger);
			}

			_logger.LogError("Invalid configuration for images. Image load file type {loadFileType}. Images format {imageFormat}.", exportSettings.LogFileFormat,
				exportSettings.TypeOfImage);
			throw new ArgumentException($"Invalid configuration for images. Image load file type {exportSettings.LogFileFormat}. Images format {exportSettings.TypeOfImage}.");
		}

		public IImageLoadFileMetadataForArtifactBuilder CreateForUnsuccessfulRollup(ExportFile exportSettings)
		{
			return new SinglePageMetadataForArtifactBuilder(exportSettings, _filePathTransformer, _imageLoadFileEntry, _fullTextLoadFileEntry, _logger);
		}
	}
}