using System;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize
{
	public class ImageExportableSize
	{
		private const double _PDF_MERGE_SIZE_ERROR_THRESHOLD = 1.03;
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
					//TODO REL-185531 image size will probably be changed after merging - another issue with size calculations? REL-185531
					//After merging size will be probably smaller, so calculation isn't precise, but we can live with that
					
					volumeSize.ImageFileCount = 1;

					if (_exportSettings.TypeOfImage == ExportFile.ImageType.Pdf)
					{
						//TODO REL-185531 images merge to PDF will be a little bigger than single tiffs so we're applying 3% factor
						volumeSize.ImageFilesSize = (long) Math.Ceiling(volumeSize.ImageFilesSize * _PDF_MERGE_SIZE_ERROR_THRESHOLD);
					}
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