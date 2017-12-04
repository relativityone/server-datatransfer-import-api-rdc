using System;
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

		public IImageLoadFileEntry Create(ExportFile exportSettings)
		{
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				return new OpticonLoadFileEntry(_logger);
			}
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO || exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
			{
				return new IproLoadFileEntry(exportSettings, _logger);
			}
			_logger.LogError("Unknown image load file format {type}. Cannot create load file entry builder.", exportSettings.LogFileFormat);
			throw new ArgumentException($"Unknown image load file format {exportSettings.LogFileFormat}.");
		}
	}
}