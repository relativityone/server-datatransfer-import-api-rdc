namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System;
	using System.Collections.Generic;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class MetadataStatistics : ITransferStatistics, IMetadataProcessingStatistics
	{
		private double _savedThroughput;
		private long _savedMetadataBytes;
		private TimeSpan _savedMetadataTransferDuration;
		private Dictionary<string, long> _savedFilesSize;

		private readonly kCura.WinEDDS.Statistics _statistics;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		private readonly object _lock = new object();

		private readonly Dictionary<string, long> _filesSize;

		public MetadataStatistics(kCura.WinEDDS.Statistics statistics, IFile fileWrapper, ILog logger)
		{
			_filesSize = new Dictionary<string, long>();
			_savedFilesSize = new Dictionary<string, long>();

			_statistics = statistics.ThrowIfNull(nameof(statistics));
			_fileWrapper = fileWrapper.ThrowIfNull(nameof(fileWrapper));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public long MetadataTime => this._statistics.MetadataTransferDuration.Ticks;

		public void Subscribe(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Attached tapi bridge {TapiBridgeInstanceId} to metadata statistics.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress += this.OnProgress;
			tapiBridge.TapiStatistics += this.TapiBridgeOnTapiStatistics;
		}

		private void TapiBridgeOnTapiStatistics(object sender, TapiStatisticsEventArgs e)
		{
			lock (_lock)
			{
				_statistics.MetadataTransferThroughput = e.TransferRateBytes;
			}
		}

		private void OnProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Progress event for file {FileName} with status {Successful}.", e.FileName, e.Successful);
			if (e.Successful)
			{
				lock (_lock)
				{
					_statistics.MetadataTransferredBytes += e.FileBytes;
					_statistics.MetadataTransferDuration += (e.EndTime - e.StartTime);
					_statistics.MetadataFilesTransferredCount++;
				}
			}
		}

		public void Unsubscribe(ITapiBridge tapiBridge)
		{
			tapiBridge.ThrowIfNull(nameof(tapiBridge));
			_logger.LogVerbose("Detached tapi bridge {TapiBridgeInstanceId} from metadata statistics.", tapiBridge.InstanceId);
			tapiBridge.TapiProgress -= this.OnProgress;
			tapiBridge.TapiStatistics -= this.TapiBridgeOnTapiStatistics;
		}

		public void UpdateStatisticsForFile(string path)
		{
			lock (_lock)
			{
				if (_fileWrapper.Exists(path))
				{
					long oldSize = 0;
					if (_filesSize.ContainsKey(path))
					{
						oldSize = _filesSize[path];
					}

					long newSize = _fileWrapper.GetFileSize(path);
					_statistics.MetadataTransferredBytes += newSize - oldSize;
					_filesSize[path] = newSize;
				}
				else
				{
					_logger.LogWarning("Trying to add statistics for file {path}, but file doesn't exist.", path);
				}
			}
		}

		public void UpdateStatistics(string fileName, bool transferResult, long transferredBytes, long totalTicks)
		{
			_logger.LogVerbose(
				"Progress event for file {FileName} with status {Successful}.",
				fileName,
				transferResult);
			if (transferResult)
			{
				lock (_lock)
				{
					// Note: the ticks continually replaces the metadata time instead of adding to the existing value.
					_statistics.MetadataFilesTransferredCount++;
					_statistics.MetadataTransferredBytes += transferredBytes;
					_statistics.MetadataTransferDuration = new TimeSpan(totalTicks);
					_statistics.MetadataTransferThroughput = kCura.WinEDDS.Statistics.CalculateThroughput(
						_statistics.MetadataTransferredBytes,
						_statistics.MetadataTransferDuration.TotalSeconds);
				}
			}
		}

		public void SaveState()
		{
			lock (_lock)
			{
				_savedThroughput = _statistics.MetadataTransferThroughput;
				_savedMetadataBytes = _statistics.MetadataTransferredBytes;
				_savedMetadataTransferDuration = _statistics.MetadataTransferDuration;
				_savedFilesSize = new Dictionary<string, long>(_filesSize);
			}
		}

		public void RestoreLastState()
		{
			lock (_lock)
			{
				_statistics.MetadataTransferThroughput = _savedThroughput;
				_statistics.MetadataTransferredBytes = _savedMetadataBytes;
				_statistics.MetadataTransferDuration = _savedMetadataTransferDuration;
				_filesSize.Clear();
				foreach (KeyValuePair<string, long> keyValuePair in _savedFilesSize)
				{
					_filesSize.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
	}
}