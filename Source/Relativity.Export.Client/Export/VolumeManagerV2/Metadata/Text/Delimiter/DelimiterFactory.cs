namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Delimiter
{
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