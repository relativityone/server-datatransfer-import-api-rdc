namespace Relativity.Export.VolumeManagerV2.Metadata.Text
{
	using System.IO;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	public class HtmlFileLongTextStreamFormatterFactory : ILongTextStreamFormatterFactory
	{
		private readonly ExportFile _exportSettings;

		public HtmlFileLongTextStreamFormatterFactory(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public ILongTextStreamFormatter Create(TextReader source)
		{
			return new HtmlFileLongTextStreamFormatter(_exportSettings, source);
		}
	}
}