using kCura.Windows.Process;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class Messenger : IMessenger
	{
		private readonly ExportFile _exportSettings;
		private readonly IDownloadProgress _downloadProgress;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public Messenger(ExportFile exportSettings, IDownloadProgress downloadProgress, IStatus status, ILog logger)
		{
			_exportSettings = exportSettings;
			_downloadProgress = downloadProgress;
			_status = status;
			_logger = logger;
		}

		public void CreatingImageLoadFileMetadata()
		{
			if (_exportSettings.ExportImages)
			{
				_logger.LogVerbose("Creating image load file.");
				_status.WriteStatusLine(EventType.Status, "Creating image load file.", true);
			}
		}

		public void CreatingLoadFileMetadata()
		{
			if (_exportSettings.ExportNative)
			{
				_logger.LogVerbose("Creating load file.");
				_status.WriteStatusLine(EventType.Status, "Creating load file.", true);
			}
		}

		public void StartingRollupImages()
		{
			bool mergingImages = _exportSettings.ExportImages && _exportSettings.VolumeInfo.CopyImageFilesFromRepository &&
								(_exportSettings.TypeOfImage == ExportFile.ImageType.MultiPageTiff || _exportSettings.TypeOfImage == ExportFile.ImageType.Pdf);
			if (mergingImages)
			{
				_logger.LogVerbose("Attempting to rollup images.");
				_status.WriteStatusLine(EventType.Status, "Attempting to rollup images.", true);
			}
		}

		public void PreparingBatchForExport()
		{
			_logger.LogVerbose("Preparing batch for export.");
			_status.WriteStatusLine(EventType.Status, "Preparing batch for export.", true);
		}

		public void ValidatingBatch()
		{
			_logger.LogVerbose("Validating batch after export.");
			_status.WriteStatusLine(EventType.Status, "Validating batch after export.", true);
		}

		public void RestoringAfterCancel()
		{
			_logger.LogVerbose("Export has been canceled. Restoring state from previous batch.");
			_status.WriteStatusLine(EventType.Status, "Export has been canceled. Restoring state from previous batch.", true);
		}

		public void BatchCompleted()
		{
			_logger.LogVerbose("Batch completed.");
			_status.WriteStatusLine(EventType.Status, "Batch completed.", true);
		}

		public void DownloadingBatch()
		{
			_logger.LogVerbose("Downloading files for batch.");
			_downloadProgress.UpdateDownloadedCount();
			_status.WriteStatusLine(EventType.Status, "Downloading files for batch.", true);
		}

		public void FilesDownloadCompleted()
		{
			_logger.LogVerbose("Documents for batch downloaded.");
			_downloadProgress.UpdateDownloadedCount();
			_status.WriteStatusLine(EventType.Progress, "Documents for batch downloaded.", true);
		}

		public void StateRestored()
		{
			_logger.LogVerbose("State from previous batch restored.");
			_status.WriteStatusLine(EventType.Status, "State from previous batch restored.", true);
		}
	}
}