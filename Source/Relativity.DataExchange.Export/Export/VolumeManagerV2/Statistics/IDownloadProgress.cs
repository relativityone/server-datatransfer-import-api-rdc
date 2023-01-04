namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	public interface IDownloadProgress
	{
		/// <summary>
		/// Perform a final check to ensure the process count accounts for all artifacts contained within the current batch. This should only be called after all native, image, and long path transfers are complete.
		/// </summary>
		/// <remarks>
		/// The process count has never reflected success or failure.
		/// </remarks>
		void FinalizeBatchProcessedCount();
	}
}