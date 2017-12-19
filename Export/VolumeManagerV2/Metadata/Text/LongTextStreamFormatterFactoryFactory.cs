namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextStreamFormatterFactoryFactory
	{
		public ILongTextStreamFormatterFactory Create(ExportFile exportSettings)
		{
			if (exportSettings.LoadFileIsHtml)
			{
				return new HtmlFileLongTextStreamFormatterFactory(exportSettings);
			}
			return new DelimitedFileLongTextStreamFormatterFactory(exportSettings);
		}
	}
}