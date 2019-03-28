using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public class LabelManagerForArtifact : ILabelManagerForArtifact
	{
		private readonly ILabelManager _labelManager;

		public LabelManagerForArtifact(ILabelManager labelManager)
		{
			_labelManager = labelManager;
		}

		public string GetImageSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return _labelManager.GetCurrentImageSubdirectoryLabel();
		}

		public string GetNativeSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return _labelManager.GetCurrentNativeSubdirectoryLabel();
		}

		public string GetTextSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return _labelManager.GetCurrentTextSubdirectoryLabel();
		}

		public string GetVolumeLabel(int objectExportInfoArtifactId)
		{
			return _labelManager.GetCurrentVolumeLabel();
		}

		public void InitializeFor(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
		}
	}
}