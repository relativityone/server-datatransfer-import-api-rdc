using System;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class FilePathTransformerFactory
	{
		private readonly ILog _logger;
		private readonly FilePathHelper _filePathHelper;

		public FilePathTransformerFactory(ILog logger, FilePathHelper filePathHelper)
		{
			_logger = logger;
			_filePathHelper = filePathHelper;
		}

		public IFilePathTransformer Create(ExportFile exportSettings)
		{
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Absolute)
			{
				return new AbsoluteFilePathTransformer();
			}
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Relative)
			{
				return new RelativeFilePathTransformer(exportSettings, _filePathHelper);
			}
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Prefix)
			{
				return new PrefixFilePathTransformer(exportSettings, _filePathHelper);
			}
			_logger.LogError("Unknown file path type {type}. Unable to create IFilePathTransformer.", exportSettings.TypeOfExportedFilePath);
			throw new ArgumentException($"Unknown file path type {exportSettings.TypeOfExportedFilePath}.");
		}
	}
}