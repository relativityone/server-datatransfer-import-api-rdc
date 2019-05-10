namespace Relativity.Export.VolumeManagerV2.Statistics
{
	using Relativity.Export.VolumeManagerV2.Batches;

	public interface IDownloadProgress : IStateful
	{
		void UpdateDownloadedCount();
	}
}