namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	public interface ILongTextDownloader
	{
		Task DownloadAsync(List<LongTextExportRequest> longTextExportRequests, CancellationToken cancellationToken);

		void RegisterSubscriber(IFileDownloadSubscriber fileDownloadSubscriber);
	}
}