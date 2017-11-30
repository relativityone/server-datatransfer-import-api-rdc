using System;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class FilePathProviderFactory
	{
		private readonly ILog _logger;

		public FilePathProviderFactory(ILog logger)
		{
			_logger = logger;
		}

		public IFilePathProvider Create(ExportFile exportSettings)
		{
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Absolute)
			{
				return new AbsoluteFilePathProvider();
			}
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Relative)
			{
				return new RelativeFilePathProvider(exportSettings);
			}
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Prefix)
			{
				var relativeFilePathProvider = new RelativeFilePathProvider(exportSettings);
				return new PrefixFilePathProvider(exportSettings, relativeFilePathProvider);
			}
			_logger.LogError("Unknown file path type. Unable to create IFilePathProvider.");
			throw new ArgumentException("Unknown file path type.");
		}
	}
}