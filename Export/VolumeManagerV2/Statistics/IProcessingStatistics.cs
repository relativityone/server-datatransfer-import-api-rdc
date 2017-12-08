using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public interface IProcessingStatistics : IStateful
	{
		void AddStatisticsForFile(string filePath);
	}
}