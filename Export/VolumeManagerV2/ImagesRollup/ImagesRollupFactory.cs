using System;
using Castle.Windsor;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup
{
	public class ImagesRollupFactory
	{
		private readonly ILog _logger;

		public ImagesRollupFactory(ILog logger)
		{
			_logger = logger;
		}

		public IImagesRollup Create(ExportFile exportSettings, IWindsorContainer container)
		{
			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
			{
				return container.Resolve<EmptyImagesRollup>();
			}
			if (!exportSettings.ExportImages || !exportSettings.VolumeInfo.CopyImageFilesFromRepository || exportSettings.TypeOfImage == ExportFile.ImageType.SinglePage)
			{
				_logger.LogVerbose("Creating SinglePage rollup.");
				return container.Resolve<SinglePageImagesRollup>();
			}
			if (exportSettings.TypeOfImage == ExportFile.ImageType.MultiPageTiff)
			{
				_logger.LogVerbose("Creating MultiPageTiff rollup.");
				return container.Resolve<MultiPageTiffImagesRollup>();
			}
			if (exportSettings.TypeOfImage == ExportFile.ImageType.Pdf)
			{
				_logger.LogVerbose("Creating PDF rollup.");
				return container.Resolve<PdfImagesRollup>();
			}
			_logger.LogError("Exporting images, but image type {type} is unknown.", exportSettings.TypeOfImage);
			throw new ArgumentException($"Unknown image type {exportSettings.TypeOfImage}.");
		}
	}
}