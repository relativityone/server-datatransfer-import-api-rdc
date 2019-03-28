using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface IPhysicalFilesDownloader
	{
		Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken batchCancellationToken);
	}
}