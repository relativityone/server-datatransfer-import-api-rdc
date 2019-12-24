namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	public interface IMetadataProcessingStatistics : IProcessingStatistics
	{
		long MetadataTime
		{
			get;
		}

		void UpdateStatistics(string fileName, bool transferResult, long transferredBytes, long totalTicks);
	}
}