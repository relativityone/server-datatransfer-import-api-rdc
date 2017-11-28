namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	/// <summary>
	///     TODO better name
	/// </summary>
	public class StatisticsWrapper
	{
		private readonly Statistics _statistics;

		public StatisticsWrapper(Statistics statistics)
		{
			_statistics = statistics;
			TotalExtractedTextFileLength = 0;
		}

		public long TotalExtractedTextFileLength { get; set; }

		public long MetadataBytes
		{
			set => _statistics.MetadataBytes = value + TotalExtractedTextFileLength;
		}
	}
}