using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class CachedLabelManagerForArtifact : ILabelManagerForArtifact
	{
		private readonly ILabelManager _labelManager;
		private readonly IDirectoryManager _directoryManager;

		private readonly Dictionary<int, string> _imageSubdirectoryLabel = new Dictionary<int, string>();
		private readonly Dictionary<int, string> _nativeSubdirectoryLabel = new Dictionary<int, string>();
		private readonly Dictionary<int, string> _textSubdirectoryLabel = new Dictionary<int, string>();
		private readonly Dictionary<int, string> _volumeLabel = new Dictionary<int, string>();

		public CachedLabelManagerForArtifact(ILabelManager labelManager, IDirectoryManager directoryManager)
		{
			_labelManager = labelManager;
			_directoryManager = directoryManager;
		}

		public string GetImageSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return _imageSubdirectoryLabel[objectExportInfoArtifactId];
		}

		public string GetNativeSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return _nativeSubdirectoryLabel[objectExportInfoArtifactId];
		}

		public string GetTextSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return _textSubdirectoryLabel[objectExportInfoArtifactId];
		}

		public string GetVolumeLabel(int objectExportInfoArtifactId)
		{
			return _volumeLabel[objectExportInfoArtifactId];
		}

		public void InitializeFor(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			ClearCache();

			for (int i = 0; i < artifacts.Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				ObjectExportInfo artifact = artifacts[i];
				VolumePredictions volumePrediction = volumePredictions[i];

				_directoryManager.MoveNext(volumePrediction);

				StoreResultsForArtifact(artifact.ArtifactID);
			}
		}

		private void ClearCache()
		{
			_imageSubdirectoryLabel.Clear();
			_nativeSubdirectoryLabel.Clear();
			_textSubdirectoryLabel.Clear();
			_volumeLabel.Clear();
		}

		private void StoreResultsForArtifact(int objectExportInfoArtifactId)
		{
			_imageSubdirectoryLabel[objectExportInfoArtifactId] = _labelManager.GetCurrentImageSubdirectoryLabel();
			_nativeSubdirectoryLabel[objectExportInfoArtifactId] = _labelManager.GetCurrentNativeSubdirectoryLabel();
			_textSubdirectoryLabel[objectExportInfoArtifactId] = _labelManager.GetCurrentTextSubdirectoryLabel();
			_volumeLabel[objectExportInfoArtifactId] = _labelManager.GetCurrentVolumeLabel();
		}
	}
}