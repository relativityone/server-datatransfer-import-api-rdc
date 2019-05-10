namespace Relativity.Export.VolumeManagerV2.Directories
{
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	public interface ILabelManagerForArtifact
	{
		string GetImageSubdirectoryLabel(int objectExportInfoArtifactId);
		string GetNativeSubdirectoryLabel(int objectExportInfoArtifactId);
		string GetTextSubdirectoryLabel(int objectExportInfoArtifactId);
		string GetVolumeLabel(int objectExportInfoArtifactId);

		void InitializeFor(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken);
	}
}