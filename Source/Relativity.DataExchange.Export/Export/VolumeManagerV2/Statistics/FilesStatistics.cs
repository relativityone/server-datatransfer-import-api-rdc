namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class FilesStatistics : ITransferStatistics, IFileProcessingStatistics
	{
		private double _savedThroughput;
		private long _savedFileBytes;
		private TimeSpan _savedFileTime;

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
				_statistics.FileTransferThroughput = e.TransferRateBytes;
			}
		}

		private void OnProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Progress event for file {fileName} with status {Successful}.", e.FileName, e.Successful);
			if (e.Successful)
			{
				lock (_lock)
				{
					_statistics.FileTransferredBytes += e.FileBytes;
					_statistics.FileTransferDuration += (e.EndTime - e.StartTime);
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
					_statistics.FileTransferredBytes += _fileWrapper.GetFileSize(path);
				}
				else
				{
					_logger.LogWarning("Trying to add statistics for file {path}, but file doesn't exist.", path);
				}
			}
		}

		public void SaveState()
		{
			lock (_lock)
			{
				_savedThroughput = _statistics.FileTransferThroughput;
				_savedFileBytes = _statistics.FileTransferredBytes;
				_savedFileTime = _statistics.FileTransferDuration;
			}
		}

		public void RestoreLastState()
		{
			lock (_lock)
			{
				_statistics.FileTransferThroughput = _savedThroughput;
				_statistics.FileTransferredBytes = _savedFileBytes;
				_statistics.FileTransferDuration = _savedFileTime;
			}
		}
	}
}