using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileFactory
	{
		private readonly IFilePathTransformer _filePathTransformer;
		private readonly ILoadFileEntry _loadFileEntry;
		private readonly IFullTextLoadFileEntry _fullTextLoadFileEntry;

		public ImageLoadFileFactory(IFilePathTransformer filePathTransformer, ILoadFileEntry loadFileEntry, IFullTextLoadFileEntry fullTextLoadFileEntry)
		{
			_filePathTransformer = filePathTransformer;
			_loadFileEntry = loadFileEntry;
			_fullTextLoadFileEntry = fullTextLoadFileEntry;
		}

		public ImageLoadFile Create(ExportFile exportSettings)
		{
			//TODO validate arguments
			if (exportSettings.TypeOfImage == ExportFile.ImageType.SinglePage)
			{
				return new SinglePageImageLoadFile(exportSettings, _filePathTransformer, _loadFileEntry, _fullTextLoadFileEntry);
			}
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				return new MultiPageOpticonImageLoadFile(exportSettings, _filePathTransformer, _loadFileEntry, _fullTextLoadFileEntry);
			}
			return new MultiPageNotOpticonImageLoadFile(exportSettings, _filePathTransformer, _loadFileEntry, _fullTextLoadFileEntry);
		}
	}
}