using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class SubdirectoryManager : ISubdirectoryManager, ISubdirectory
	{
		private long _currentNativeSubdirectoryFileCount;
		private long _currentImageSubdirectoryFileCount;
		private long _currentTextSubdirectoryFileCount;

		private VolumePredictions _currentVolumePredictions;

		private readonly long _subdirectoryMaxFileCount;
		private readonly int _subdirectoryStartNumber;

		public SubdirectoryManager(ExportFile exportSettings)
		{
			_subdirectoryMaxFileCount = exportSettings.VolumeInfo.SubdirectoryMaxSize;
			_subdirectoryStartNumber = exportSettings.VolumeInfo.SubdirectoryStartNumber;

			RestartSubdirectoryCounting();
		}

		public int CurrentSubdirectoryNumber { get; private set; }

		public void MoveNext(VolumePredictions volumePredictions)
		{
			_currentVolumePredictions = volumePredictions;
			if (WillNativeFolderBeFull() || WillImageFolderBeFull() || WillTextFolderBeFull())
			{
				MoveToNextSubdirectory();
			}
			_currentNativeSubdirectoryFileCount += _currentVolumePredictions.NativeFileCount;
			_currentImageSubdirectoryFileCount += _currentVolumePredictions.ImageFileCount;
			_currentTextSubdirectoryFileCount += _currentVolumePredictions.TextFileCount;
		}

		private bool WillNativeFolderBeFull()
		{
			return _currentNativeSubdirectoryFileCount + _currentVolumePredictions.NativeFileCount > _subdirectoryMaxFileCount && _currentNativeSubdirectoryFileCount > 0;
		}

		private bool WillImageFolderBeFull()
		{
			return _currentImageSubdirectoryFileCount + _currentVolumePredictions.ImageFileCount > _subdirectoryMaxFileCount && _currentImageSubdirectoryFileCount > 0;
		}

		private bool WillTextFolderBeFull()
		{
			return _currentTextSubdirectoryFileCount + _currentVolumePredictions.TextFileCount > _subdirectoryMaxFileCount && _currentTextSubdirectoryFileCount > 0;
		}

		private void MoveToNextSubdirectory()
		{
			ResetFileCounts();
			CurrentSubdirectoryNumber++;
		}

		public void RestartSubdirectoryCounting()
		{
			ResetFileCounts();
			CurrentSubdirectoryNumber = _subdirectoryStartNumber;
		}

		private void ResetFileCounts()
		{
			_currentNativeSubdirectoryFileCount = 0;
			_currentImageSubdirectoryFileCount = 0;
			_currentTextSubdirectoryFileCount = 0;
		}
	}
}