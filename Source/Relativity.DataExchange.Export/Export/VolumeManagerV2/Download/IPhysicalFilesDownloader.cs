namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface IPhysicalFilesDownloader
	{
		Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken batchCancellationToken);
	}
}