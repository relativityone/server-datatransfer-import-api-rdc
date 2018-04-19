using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class MetadataStatistics : ITransferStatistics, IMetadataProcessingStatistics
	{
		private ITapiBridge _tapiBridge;

		private long _savedMetadataBytes;
		private long _savedMetadataTime;
		private Dictionary<string, long> _savedFilesSize;

		private readonly WinEDDS.Statistics _statistics;
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		private readonly object _lock = new object();

		private readonly Dictionary<string, long> _filesSize;

		public MetadataStatistics(WinEDDS.Statistics statistics, IFileHelper fileHelper, ILog logger)
		{
			_filesSize = new Dictionary<string, long>();
			_savedFilesSize = new Dictionary<string, long>();

			_statistics = statistics;
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public void Attach(ITapiBridge tapiBridge)
		{
			_tapiBridge = tapiBridge;
			_tapiBridge.TapiProgress += OnProgress;
		}

		private void OnProgress(object sender, TapiProgressEventArgs e)
		{
			_logger.LogVerbose("Progress event for file {fileName} with status {didTransferSucceed}.", e.FileName, e.DidTransferSucceed);
			if (e.DidTransferSucceed)
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

		public void UpdateStatisticsForFile(string path)
		{
			lock (_lock)
			{
				if (_fileHelper.Exists(path))
				{
					long oldSize = 0;
					if (_filesSize.ContainsKey(path))
					{
						oldSize = _filesSize[path];
					}

					long newSize = _fileHelper.GetFileSize(path);
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
				_savedMetadataBytes = _statistics.MetadataBytes;
				_savedMetadataTime = _statistics.MetadataTime;
				_savedFilesSize = new Dictionary<string, long>(_filesSize);
			}
		}

		public void RestoreLastState()
		{
			lock (_lock)
			{
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