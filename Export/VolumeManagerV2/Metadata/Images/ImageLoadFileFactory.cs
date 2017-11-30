using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileFactory
	{
		private readonly IFilePathProvider _filePathProvider;
		private readonly ILoadFileEntry _loadFileEntry;
		private readonly IFullTextLoadFileEntry _fullTextLoadFileEntry;

		public ImageLoadFileFactory(IFilePathProvider filePathProvider, ILoadFileEntry loadFileEntry, IFullTextLoadFileEntry fullTextLoadFileEntry)
		{
			_filePathProvider = filePathProvider;
			_loadFileEntry = loadFileEntry;
			_fullTextLoadFileEntry = fullTextLoadFileEntry;
		}

		public ImageLoadFile Create(ExportFile exportSettings)
		{
			//TODO validate arguments
			if (exportSettings.TypeOfImage == ExportFile.ImageType.SinglePage)
			{
				return new SinglePageImageLoadFile(exportSettings, _filePathProvider, _loadFileEntry, _fullTextLoadFileEntry);
			}
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				return new MultiPageOpticonImageLoadFile(exportSettings, _filePathProvider, _loadFileEntry, _fullTextLoadFileEntry);
			}
			return new MultiPageNotOpticonImageLoadFile(exportSettings, _filePathProvider, _loadFileEntry, _fullTextLoadFileEntry);
		}
	}
}