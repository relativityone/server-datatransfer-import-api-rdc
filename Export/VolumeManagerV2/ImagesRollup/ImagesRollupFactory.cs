using System;
using kCura.Utility;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class ImagesRollupFactory
	{
		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImagesRollupFactory(IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public IImagesRollup Create(ExportFile exportSettings)
		{
			if (!exportSettings.ExportImages || !exportSettings.VolumeInfo.CopyImageFilesFromRepository || exportSettings.TypeOfImage == ExportFile.ImageType.SinglePage)
			{
				return new SinglePageImagesRollup();
			}
			if (exportSettings.TypeOfImage == ExportFile.ImageType.MultiPageTiff)
			{
				return new MultiPageTiffImagesRollup(exportSettings, _fileHelper, _status, _logger, new Image());
			}
			if (exportSettings.TypeOfImage == ExportFile.ImageType.Pdf)
			{
				return new PdfImagesRollup(exportSettings, _fileHelper, _status, _logger, new Image());
			}
			_logger.LogError("Exporting images, but image type is unknown.");
			throw new ArgumentException("Unknown image type.");
		}
	}
}