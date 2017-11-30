using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	/// <summary>
	/// TODO change name :)
	/// </summary>
	public class TrueVolumeManager : IVolume, IDirectoryManager
	{
		private long _currentVolumeSize;

		private readonly long _volumeMaxSizeInMB;
		private readonly ISubdirectoryManager _subdirectoryManager;

		public TrueVolumeManager(ExportFile exportSettings, ISubdirectoryManager subdirectoryManager)
		{
			_subdirectoryManager = subdirectoryManager;

			const int bytesInKB = 1024;
			_volumeMaxSizeInMB = exportSettings.VolumeInfo.VolumeMaxSize * bytesInKB * bytesInKB;

			CurrentVolumeNumber = exportSettings.VolumeInfo.VolumeStartNumber;

			_currentVolumeSize = 0;
		}

		public int CurrentVolumeNumber { get; private set; }

		public void MoveNext(VolumePredictions volumePredictions)
		{
			if (_currentVolumeSize + volumePredictions.TotalFileSize > _volumeMaxSizeInMB && _currentVolumeSize > 0)
			{
				MoveToNextVolume();
			}
			_currentVolumeSize += volumePredictions.TotalFileSize;
		}

		private void MoveToNextVolume()
		{
			_currentVolumeSize = 0;
			CurrentVolumeNumber++;

			_subdirectoryManager.RestartSubdirectoryCounting();
		}
	}
}