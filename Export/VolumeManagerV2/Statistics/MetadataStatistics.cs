using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class MetadataStatistics : ITransferStatistics, IMetadataProcessingStatistics
	{
		private TapiBridge _tapiBridge;

		private long _savedMetadataBytes;
		private long _savedMetadataTime;

		private readonly WinEDDS.Statistics _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		private readonly object _lock = new object();

		public MetadataStatistics(WinEDDS.Statistics statistics, IFileHelper fileHelper, ILog logger)
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
			if (e.Status)
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
		}

		public void AddStatisticsForFile(string path)
		{
			lock (_lock)
			{
				if (_fileHelper.Exists(path))
				{
					_statistics.MetadataBytes += _fileHelper.GetFileSize(path);
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
				_savedMetadataBytes = _statistics.MetadataBytes;
				_savedMetadataTime = _statistics.MetadataTime;
			}
		}

		public void RestoreLastState()
		{
			lock (_lock)
			{
				_statistics.MetadataBytes = _savedMetadataBytes;
				_statistics.MetadataTime = _savedMetadataTime;
			}
		}
	}
}