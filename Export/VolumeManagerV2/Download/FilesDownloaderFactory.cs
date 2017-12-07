using Castle.Windsor;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FilesDownloaderFactory
	{
		private readonly LongTextExportRequestBuilder _longTextExportRequestBuilder;
		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;
		private readonly IDirectoryManager _directoryManager;
		private readonly ILog _logger;
		private readonly LabelManager _labelManager;

		public FilesDownloaderFactory(LongTextExportRequestBuilder longTextExportRequestBuilder, ExportTapiBridgeFactory exportTapiBridgeFactory, IDirectoryManager directoryManager,
			ILog logger, LabelManager labelManager)
		{
			_longTextExportRequestBuilder = longTextExportRequestBuilder;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_directoryManager = directoryManager;
			_logger = logger;
			_labelManager = labelManager;
		}


		public FilesDownloader Create(ExportFile exportSettings, IWindsorContainer container)
		{
			IFileExportRequestBuilder nativeExportRequestBuilder;
			IFileExportRequestBuilder imageExportRequestBuilder;

			if (exportSettings.ArtifactTypeID != (int) ArtifactType.Document)
			{
				nativeExportRequestBuilder = container.Resolve<FieldFileExportRequestBuilder>();
				imageExportRequestBuilder = container.Resolve<EmptyExportRequestBuilder>();
			}
			else
			{
				if (exportSettings.ExportNative && exportSettings.VolumeInfo.CopyNativeFilesFromRepository)
				{
					_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(NativeFileExportRequestBuilder));
					nativeExportRequestBuilder = container.Resolve<NativeFileExportRequestBuilder>();
				}
				else
				{
					_logger.LogVerbose("Creating {requestBuilder} for natives.", nameof(EmptyExportRequestBuilder));
					nativeExportRequestBuilder = container.Resolve<EmptyExportRequestBuilder>();
				}

				if (exportSettings.ExportImages && exportSettings.VolumeInfo.CopyImageFilesFromRepository)
				{
					_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(ImageExportRequestBuilder));
					imageExportRequestBuilder = container.Resolve<ImageExportRequestBuilder>();
				}
				else
				{
					_logger.LogVerbose("Creating {requestBuilder} for images.", nameof(EmptyExportRequestBuilder));
					imageExportRequestBuilder = container.Resolve<EmptyExportRequestBuilder>();
				}
			}

			return new FilesDownloader(nativeExportRequestBuilder, imageExportRequestBuilder, _longTextExportRequestBuilder, _exportTapiBridgeFactory, _directoryManager, _logger,
				_labelManager);
		}
	}
}