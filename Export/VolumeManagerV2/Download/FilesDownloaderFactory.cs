using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FilesDownloaderFactory
	{
		private readonly IFileHelper _fileHelper;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly IStatus _status;
		private readonly LongTextExportRequestBuilder _longTextExportRequestBuilder;
		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;
		private readonly IDirectoryManager _directoryManager;
		private readonly ILog _logger;
		private readonly FileDownloader _fileDownloader;
		private readonly NativeFilePathProvider _nativeFilePathProvider;
		private readonly ImageFilePathProvider _imageFilePathProvider;

		public FilesDownloaderFactory(IFileHelper fileHelper, IFileNameProvider fileNameProvider, IStatus status,
			LongTextExportRequestBuilder longTextExportRequestBuilder, ExportTapiBridgeFactory exportTapiBridgeFactory, IDirectoryManager directoryManager, ILog logger,
			FileDownloader fileDownloader, NativeFilePathProvider nativeFilePathProvider, ImageFilePathProvider imageFilePathProvider)
		{
			_fileHelper = fileHelper;
			_fileNameProvider = fileNameProvider;
			_status = status;
			_longTextExportRequestBuilder = longTextExportRequestBuilder;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_directoryManager = directoryManager;
			_logger = logger;
			_fileDownloader = fileDownloader;
			_nativeFilePathProvider = nativeFilePathProvider;
			_imageFilePathProvider = imageFilePathProvider;
		}


		public FilesDownloader Create(ExportFile exportSettings)
		{
			IExportRequestBuilder nativeExportRequestBuilder;
			if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
			{
				_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(NativeExportRequestBuilder));
				nativeExportRequestBuilder = new NativeExportRequestBuilder(exportSettings, _nativeFilePathProvider, _fileHelper, _fileNameProvider, _status, _logger);
			}
			else
			{
				_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(EmptyExportRequestBuilder));
				nativeExportRequestBuilder = new EmptyExportRequestBuilder();
			}
			IExportRequestBuilder imageExportRequestBuilder;
			if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
			{
				_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(ImageExportRequestBuilder));
				imageExportRequestBuilder = new ImageExportRequestBuilder(exportSettings, _imageFilePathProvider, _fileHelper, _status, _logger);
			}
			else
			{
				_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(EmptyExportRequestBuilder));
				imageExportRequestBuilder = new EmptyExportRequestBuilder();
			}

			return new FilesDownloader(nativeExportRequestBuilder, imageExportRequestBuilder, _longTextExportRequestBuilder, _exportTapiBridgeFactory, _directoryManager, _logger,
				_fileDownloader, exportSettings);
		}
	}
}