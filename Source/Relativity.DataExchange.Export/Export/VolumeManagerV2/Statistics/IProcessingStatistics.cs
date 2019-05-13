namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;

	public interface IProcessingStatistics : IStateful
	{
		void UpdateStatisticsForFile(string filePath);
	}
}