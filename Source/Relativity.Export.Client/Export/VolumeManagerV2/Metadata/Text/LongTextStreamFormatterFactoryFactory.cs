namespace Relativity.Export.VolumeManagerV2.Metadata.Text
{
	using kCura.WinEDDS;

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