using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class DirectoryManager : IDirectoryManager
	{
		private readonly TrueVolumeManager _volumeManager;
		private readonly SubdirectoryManager _subdirectoryManager;

		public DirectoryManager(TrueVolumeManager volumeManager, SubdirectoryManager subdirectoryManager)
		{
			_volumeManager = volumeManager;
			_subdirectoryManager = subdirectoryManager;
		}

		public void MoveNext(VolumePredictions volumePredictions)
		{
			_volumeManager.MoveNext(volumePredictions);
			_subdirectoryManager.MoveNext(volumePredictions);
		}
	}
}