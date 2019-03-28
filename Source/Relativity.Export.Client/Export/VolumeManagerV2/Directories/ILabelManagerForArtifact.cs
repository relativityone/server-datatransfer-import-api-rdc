using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public interface ILabelManagerForArtifact
	{
		string GetImageSubdirectoryLabel(int objectExportInfoArtifactId);
		string GetNativeSubdirectoryLabel(int objectExportInfoArtifactId);
		string GetTextSubdirectoryLabel(int objectExportInfoArtifactId);
		string GetVolumeLabel(int objectExportInfoArtifactId);

		void InitializeFor(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken);
	}
}