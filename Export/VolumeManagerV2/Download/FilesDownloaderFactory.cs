using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FilesDownloaderFactory
	{
		private readonly IFileNameProvider _fileNameProvider;
		private readonly LongTextExportRequestBuilder _longTextExportRequestBuilder;
		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;
		private readonly IDirectoryManager _directoryManager;
		private readonly ILog _logger;
		private readonly NativeFilePathProvider _nativeFilePathProvider;
		private readonly ImageFilePathProvider _imageFilePathProvider;
		private readonly ExportFileValidator _validator;
		private readonly LabelManager _labelManager;
		private readonly ExportFileDownloaderStatus _exportFileDownloaderStatus;

		public FilesDownloaderFactory(IFileNameProvider fileNameProvider, ExportFileValidator validator, LongTextExportRequestBuilder longTextExportRequestBuilder,
			ExportTapiBridgeFactory exportTapiBridgeFactory, IDirectoryManager directoryManager, ILog logger, NativeFilePathProvider nativeFilePathProvider,
			ImageFilePathProvider imageFilePathProvider, LabelManager labelManager, ExportFileDownloaderStatus exportFileDownloaderStatus)
		{
			_fileNameProvider = fileNameProvider;
			_validator = validator;
			_longTextExportRequestBuilder = longTextExportRequestBuilder;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_directoryManager = directoryManager;
			_logger = logger;
			_nativeFilePathProvider = nativeFilePathProvider;
			_imageFilePathProvider = imageFilePathProvider;
			_labelManager = labelManager;
			_exportFileDownloaderStatus = exportFileDownloaderStatus;
		}


		public FilesDownloader Create(ExportFile exportSettings)
		{
			IFileExportRequestBuilder nativeExportRequestBuilder;
			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(NativeExportRequestBuilder));
				nativeExportRequestBuilder = new NativeExportRequestBuilder(_nativeFilePathProvider, _fileNameProvider, _validator, _logger);
			}
			else
			{
				_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(EmptyExportRequestBuilder));
				nativeExportRequestBuilder = new EmptyExportRequestBuilder();
			}
			IFileExportRequestBuilder imageExportRequestBuilder;
			if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(ImageExportRequestBuilder));
				imageExportRequestBuilder = new ImageExportRequestBuilder(_imageFilePathProvider, _validator, _logger);
			}
			else
			{
				_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(EmptyExportRequestBuilder));
				imageExportRequestBuilder = new EmptyExportRequestBuilder();
			}

			return new FilesDownloader(nativeExportRequestBuilder, imageExportRequestBuilder, _longTextExportRequestBuilder, _exportTapiBridgeFactory, _directoryManager, _logger,
				_labelManager, _exportFileDownloaderStatus);
		}
	}
}