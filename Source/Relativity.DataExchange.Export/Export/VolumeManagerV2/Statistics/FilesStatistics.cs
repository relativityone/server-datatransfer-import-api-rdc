namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Transfer;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	public class FilesStatistics : ITransferStatistics, IFileProcessingStatistics
	{
		private ITapiBridge _tapiBridge;

		private double _savedThroughput;
		private long _savedFileBytes;
		private long _savedFileTime;

		private readonly kCura.WinEDDS.Statistics _statistics;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		private readonly object _lock = new object();

		public FilesStatistics(kCura.WinEDDS.Statistics statistics, IFile fileWrapper, ILog logger)
		{
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
				_statistics.FileThroughput = e.TransferRateBytes;
			}
		}

		private void OnProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Progress event for file {fileName} with status {didTransferSucceed}.", e.FileName, e.DidTransferSucceed);
			if (e.DidTransferSucceed)
			{
				lock (_lock)
				{
					_statistics.FileBytes += e.FileBytes;
					_statistics.FileTime += e.EndTime.Ticks - e.StartTime.Ticks;
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
					_statistics.FileBytes += _fileWrapper.GetFileSize(path);
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