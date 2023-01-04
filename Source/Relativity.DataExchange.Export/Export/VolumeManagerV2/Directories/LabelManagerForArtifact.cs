namespace Relativity.DataExchange.Export.VolumeManagerV2.Directories
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

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

		public string GetPdfSubdirectoryLabel(int objectExportInfoArtifactId)
		{
			return _labelManager.GetCurrentPdfSubdirectoryLabel();
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