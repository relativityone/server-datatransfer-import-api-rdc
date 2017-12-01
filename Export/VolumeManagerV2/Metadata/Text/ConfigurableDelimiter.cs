namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class ConfigurableDelimiter : IDelimiter
	{
		public ConfigurableDelimiter(string start, string end)
		{
			Start = start;
			End = end;
		}

		public string Start { get; }
		public string End { get; }
	}
}