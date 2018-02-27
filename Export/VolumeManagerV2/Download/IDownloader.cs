using System.Threading;
using System.Threading.Tasks;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface IDownloader
	{
		void DownloadFilesForArtifacts(CancellationToken cancellationToken);
	}
}