using System;
using Castle.Windsor;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class FilePathTransformerFactory
	{
		private readonly ILog _logger;

		public FilePathTransformerFactory(ILog logger)
		{
			_logger = logger;
		}

		public IFilePathTransformer Create(ExportFile exportSettings, IWindsorContainer container)
		{
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Absolute)
			{
				return container.Resolve<AbsoluteFilePathTransformer>();
			}
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Relative)
			{
				return container.Resolve<RelativeFilePathTransformer>();
			}
			if (exportSettings.TypeOfExportedFilePath == ExportFile.ExportedFilePathType.Prefix)
			{
				return container.Resolve<PrefixFilePathTransformer>();
			}
			_logger.LogError("Unknown file path type {type}. Unable to create IFilePathTransformer.", exportSettings.TypeOfExportedFilePath);
			throw new ArgumentException($"Unknown file path type {exportSettings.TypeOfExportedFilePath}.");
		}
	}
}