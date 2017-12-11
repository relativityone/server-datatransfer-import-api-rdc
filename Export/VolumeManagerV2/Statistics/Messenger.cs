using kCura.Windows.Process;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class Messenger : IMessenger
	{
		private readonly ExportFile _exportSettings;
		private readonly IDownloadProgress _downloadProgress;
		private readonly IStatus _status;

		public Messenger(ExportFile exportSettings, IDownloadProgress downloadProgress, IStatus status)
		{
			_exportSettings = exportSettings;
			_downloadProgress = downloadProgress;
			_status = status;
		}

		public void CreatingImageLoadFileMetadata()
		{
			if (_exportSettings.ExportImages)
			{
				_status.WriteStatusLine(EventType.Status, "Creating image load file.", true);
			}
		}

		public void CreatingLoadFileMetadata()
		{
			if (_exportSettings.ExportNative)
			{
				_status.WriteStatusLine(EventType.Status, "Creating load file.", true);
			}
		}

		public void StartingRollupImages()
		{
			bool mergingImages = _exportSettings.ExportImages && _exportSettings.VolumeInfo.CopyImageFilesFromRepository &&
								(_exportSettings.TypeOfImage == ExportFile.ImageType.MultiPageTiff || _exportSettings.TypeOfImage == ExportFile.ImageType.Pdf);
			if (mergingImages)
			{
				_status.WriteStatusLine(EventType.Status, "Attempting to rollup images.", true);
			}
		}

		public void PreparingBatchForExport()
		{
			_status.WriteStatusLine(EventType.Status, "Preparing batch for export.", true);
		}

		public void ValidatingBatch()
		{
			_status.WriteStatusLine(EventType.Status, "Validating batch after export.", true);
		}

		public void RestoringAfterCancel()
		{
			_status.WriteStatusLine(EventType.Status, "Export has been canceled. Restoring state from previous batch.", true);
		}

		public void BatchCompleted()
		{
			_status.WriteStatusLine(EventType.Status, "Batch completed.", true);
		}

		public void DownloadingBatch()
		{
			_downloadProgress.UpdateDownloadedCount();
			_status.WriteStatusLine(EventType.Status, "Downloading files for batch.", true);
		}

		public void FilesDownloadCompleted()
		{
			_downloadProgress.UpdateDownloadedCount();
			_status.WriteStatusLine(EventType.Progress, "Documents for batch downloaded.", true);
		}

		public void StateRestored()
		{
			_status.WriteStatusLine(EventType.Status, "State from previous batch restored.", true);
		}
	}
}