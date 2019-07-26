namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System.Collections.Generic;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	public class MetadataStatistics : ITransferStatistics, IMetadataProcessingStatistics
	{
		private ITapiBridge _tapiBridge;

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

			_statistics = statistics;
			_fileWrapper = fileWrapper;
			_logger = logger;
		}

		public void Attach(ITapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiProgress += OnProgress;
			_tapiBridge.TapiStatistics += TapiBridgeOnTapiStatistics;
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
			_logger.LogVerbose("Progress event for file {FileName} with status {Successful}.", e.FileName, e.Successful);
			if (e.Successful)
			{
				lock (_lock)
				{
					_statistics.MetadataBytes += e.FileBytes;
					_statistics.MetadataTime += e.EndTime.Ticks - e.StartTime.Ticks;
				}
			}
		}

		public void Detach()
		{
			_tapiBridge.TapiProgress -= OnProgress;
			_tapiBridge.TapiStatistics -= TapiBridgeOnTapiStatistics;
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
					_logger.LogWarning("Trying to add statistics for file {path}, but file doesn't exist.", path);
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