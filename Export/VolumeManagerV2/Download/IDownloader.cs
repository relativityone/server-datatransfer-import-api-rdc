using System.Threading;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface IDownloader
	{
		void DownloadFilesForArtifacts(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken);
	}
}