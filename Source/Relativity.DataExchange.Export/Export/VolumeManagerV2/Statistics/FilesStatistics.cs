namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Logger;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class FilesStatistics : ITransferStatistics, IFileProcessingStatistics
	{
		private double _savedThroughput;
		private long _savedFileBytes;
		private long _savedFileTime;

		private readonly kCura.WinEDDS.Statistics _statistics;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		private readonly object _lock = new object();

		public FilesStatistics(kCura.WinEDDS.Statistics statistics, IFile fileWrapper, ILog logger)
		{
			_statistics = statistics.ThrowIfNull(nameof(statistics));
			_fileWrapper = fileWrapper.ThrowIfNull(nameof(fileWrapper));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public void Subscribe(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Attached tapi bridge {TapiBridgeInstanceId} to the file statistics.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress += this.OnProgress;
			tapiBridge.TapiStatistics += this.TapiBridgeOnTapiStatistics;
		}

		private void TapiBridgeOnTapiStatistics(object sender, TapiStatisticsEventArgs e)
		{
			lock (_lock)
			{
				_statistics.FileThroughput = e.TransferRateBytes;
			}
		}

		private void OnProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Progress event for file {fileName} with status {Successful}.", e.FileName.Secure(), e.Successful);
			if (e.Successful)
			{
				lock (_lock)
				{
					_statistics.FileBytes += e.FileBytes;
					_statistics.FileTime += e.EndTime.Ticks - e.StartTime.Ticks;
					_statistics.NativeFilesTransferredCount++;
				}
			}
		}

		public void Unsubscribe(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Detached tapi bridge {TapiBridgeInstanceId} from the file statistics.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress -= this.OnProgress;
			tapiBridge.TapiStatistics -= this.TapiBridgeOnTapiStatistics;
		}

		public void UpdateStatisticsForFile(string path)
		{
			lock (_lock)
			{
				if (_fileWrapper.Exists(path))
				{
					_statistics.FileBytes += _fileWrapper.GetFileSize(path);
				}
				else
				{
					_logger.LogWarning("Trying to add statistics for file {path}, but file doesn't exist.", path.Secure());
				}
			}
		}

		public void SaveState()
		{
			lock (_lock)
			{
				_savedThroughput = _statistics.FileThroughput;
				_savedFileBytes = _statistics.FileBytes;
				_savedFileTime = _statistics.FileTime;
			}
		}

		public void RestoreLastState()
		{
			lock (_lock)
			{
				_statistics.FileThroughput = _savedThroughput;
				_statistics.FileBytes = _savedFileBytes;
				_statistics.FileTime = _savedFileTime;
			}
		}
	}
}