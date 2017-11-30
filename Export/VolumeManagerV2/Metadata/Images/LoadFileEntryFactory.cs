using System;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public class LoadFileEntryFactory
	{
		private readonly ILog _logger;

		public LoadFileEntryFactory(ILog logger)
		{
			_logger = logger;
		}

		public ILoadFileEntry Create(ExportFile exportSettings)
		{
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.Opticon)
			{
				return new OpticonLoadFileEntry();
			}
			if (exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO || exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
			{
				return new IproLoadFileEntry(exportSettings);
			}
			_logger.LogError("Unknown image load file format. Cannot create load file entry builder.");
			throw new ArgumentException("Unknown image load file format.");
		}
	}
}