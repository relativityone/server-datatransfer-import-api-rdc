using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize
{
	public class ImageExportableSize
	{
		private readonly ExportFile _exportSettings;

		public ImageExportableSize(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public void CalculateImagesSize(VolumePredictions volumeSize)
		{
			bool areImageFilesBeingExported = _exportSettings.ExportImages && _exportSettings.VolumeInfo.CopyImageFilesFromRepository;
			if (areImageFilesBeingExported)
			{
				bool areImagesBeingMergedIntoMultiPage = volumeSize.ImageFileCount > 0 &&
														(_exportSettings.TypeOfImage == ExportFile.ImageType.MultiPageTiff || _exportSettings.TypeOfImage == ExportFile.ImageType.Pdf);
				if (areImagesBeingMergedIntoMultiPage)
				{
					//TODO image size will probably be changed after merging - another issue with size calculations? REL-185531
					//After merging size will be probably smaller, so calculation isn't precise, but we can live with that
					//What about changing format from tif to i.e. pdf?
					
					volumeSize.ImageFileCount = 1;
				}
			}
			else
			{
				volumeSize.ImageFileCount = 0;
				volumeSize.ImageFilesSize = 0;
			}
		}
	}
}