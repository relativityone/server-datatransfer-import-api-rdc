namespace Relativity.Export.VolumeManagerV2.Metadata.Text.Delimiter
{
	using kCura.WinEDDS;

	public class DelimiterFactory
	{
		public IDelimiter Create(ExportFile exportSettings)
		{
			if (exportSettings.LoadFileIsHtml)
			{
				return new HtmlDelimiter();
			}

			return new ConfigurableDelimiter(exportSettings.QuoteDelimiter.ToString(), exportSettings.QuoteDelimiter.ToString());
		}
	}
}