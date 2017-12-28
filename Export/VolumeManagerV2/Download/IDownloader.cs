using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface IDownloader
	{
		void DownloadFilesForArtifacts(CancellationToken cancellationToken);
	}
}