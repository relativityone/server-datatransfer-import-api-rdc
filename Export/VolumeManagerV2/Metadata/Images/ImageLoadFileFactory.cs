using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class ImageLoadFileFactory
	{
		private readonly IFilePathProvider _filePathProvider;
		private readonly ILoadFileEntry _loadFileEntry;

		public ImageLoadFileFactory(IFilePathProvider filePathProvider, ILoadFileEntry loadFileEntry)
		{
			_filePathProvider = filePathProvider;
			_loadFileEntry = loadFileEntry;
		}

		public ImageLoadFile Create(ExportFile exportSettings)
		{
			//TODO validate arguments
			if (exportSettings.TypeOfImage == ExportFile.ImageType.SinglePage)
			{
				return new SinglePageImageLoadFile(exportSettings, _filePathProvider, _loadFileEntry);
			}
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				return new MultiPageOpticonImageLoadFile(exportSettings, _filePathProvider, _loadFileEntry);
			}
			return new MultiPageNotOpticonImageLoadFile(exportSettings, _filePathProvider, _loadFileEntry);
		}
	}
}