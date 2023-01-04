namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System.Threading;
	using System.Threading.Tasks;

	public interface IDownloader
	{
		Task DownloadFilesForArtifactsAsync(CancellationToken cancellationToken);
	}
}