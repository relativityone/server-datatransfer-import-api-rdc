using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public interface ILongTextDownloader
	{
		Task DownloadAsync(List<LongTextExportRequest> longTextExportRequests, CancellationToken cancellationToken);
	}
}