namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System.Collections.Generic;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Logger;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	public class MetadataStatistics : ITransferStatistics, IMetadataProcessingStatistics
	{
		private double _savedThroughput;
		private long _savedMetadataBytes;
		private long _savedMetadataTime;
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

		public long MetadataTime => this._statistics.MetadataTime;

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
				_statistics.MetadataThroughput = e.TransferRateBytes;
			}
		}

		private void OnProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Progress event for file {FileName} with status {Successful}.", e.FileName.Secure(), e.Successful);
			if (e.Successful)
			{
				lock (_lock)
				{
					_statistics.MetadataBytes += e.FileBytes;
					_statistics.MetadataTime += e.EndTime.Ticks - e.StartTime.Ticks;
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
					_statistics.MetadataBytes += newSize - oldSize;
					_filesSize[path] = newSize;
				}
				else
				{
					_logger.LogWarning("Trying to add statistics for file {path}, but file doesn't exist.", path.Secure());
				}
			}
		}

		public void UpdateStatistics(string fileName, bool transferResult, long transferredBytes, long totalTicks)
		{
			_logger.LogVerbose(
				"Progress event for file {FileName} with status {Successful}.",
				fileName.Secure(),
				transferResult);
			if (transferResult)
			{
				lock (_lock)
				{
					// Note: the ticks continually replaces the metadata time instead of adding to the existing value.
					_statistics.MetadataFilesTransferredCount++;
					_statistics.MetadataBytes += transferredBytes;
					_statistics.MetadataTime = totalTicks;
					_statistics.MetadataThroughput = kCura.WinEDDS.Statistics.CalculateThroughput(
						_statistics.MetadataBytes,
						_statistics.MetadataTime);
				}
			}
		}

		public void SaveState()
		{
			lock (_lock)
			{
				_savedThroughput = _statistics.MetadataThroughput;
				_savedMetadataBytes = _statistics.MetadataBytes;
				_savedMetadataTime = _statistics.MetadataTime;
				_savedFilesSize = new Dictionary<string, long>(_filesSize);
			}
		}

		public void RestoreLastState()
		{
			lock (_lock)
			{
				_statistics.MetadataThroughput = _savedThroughput;
				_statistics.MetadataBytes = _savedMetadataBytes;
				_statistics.MetadataTime = _savedMetadataTime;
				_filesSize.Clear();
				foreach (KeyValuePair<string, long> keyValuePair in _savedFilesSize)
				{
					_filesSize.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}
	}
}