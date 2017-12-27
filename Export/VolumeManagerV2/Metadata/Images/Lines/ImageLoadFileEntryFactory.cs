using System;
using Castle.Windsor;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public class ImageLoadFileEntryFactory
	{
		private readonly ILog _logger;

		public ImageLoadFileEntryFactory(ILog logger)
		{
			_logger = logger;
		}

		public IImageLoadFileEntry Create(ExportFile exportSettings, IWindsorContainer container)
		{
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				return container.Resolve<OpticonLoadFileEntry>();
			}

			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO || exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
			{
				return container.Resolve<IproLoadFileEntry>();
			}

			_logger.LogError("Unknown image load file format {type}. Cannot create load file entry builder.", exportSettings.LogFileFormat);
			throw new ArgumentException($"Unknown image load file format {exportSettings.LogFileFormat}.");
		}
	}
}