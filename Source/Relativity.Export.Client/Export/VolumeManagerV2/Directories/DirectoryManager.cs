namespace Relativity.Export.VolumeManagerV2.Directories
{
	using kCura.WinEDDS.Exporters;

	public class DirectoryManager : IDirectoryManager
	{
		private readonly VolumeManager _volumeManager;
		private readonly SubdirectoryManager _subdirectoryManager;

		public DirectoryManager(VolumeManager volumeManager, SubdirectoryManager subdirectoryManager)
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