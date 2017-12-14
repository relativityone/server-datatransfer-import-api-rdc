using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class FilesStatistics : ITransferStatistics, IFileProcessingStatistics
	{
		private TapiBridge _tapiBridge;

		private long _savedFileBytes;
		private long _savedFileTime;

		private readonly WinEDDS.Statistics _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		private readonly object _lock = new object();

		public FilesStatistics(WinEDDS.Statistics statistics, IFileHelper fileHelper, ILog logger)
		{
			_statistics = statistics;
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public void Attach(TapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiProgress += OnProgress;
		}

		private void OnProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Progress event for file {fileName} with status {status}.", e.FileName, e.Status);
			if (e.Status)
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
		}

		public void AddStatisticsForFile(string path)
		{
			lock (_lock)
			{
				if (_fileHelper.Exists(path))
				{
					_statistics.FileBytes += _fileHelper.GetFileSize(path);
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
				_savedFileBytes = _statistics.FileBytes;
				_savedFileTime = _statistics.FileTime;
			}
		}

		public void RestoreLastState()
		{
			lock (_lock)
			{
				_statistics.FileBytes = _savedFileBytes;
				_statistics.FileTime = _savedFileTime;
			}
		}
	}
}