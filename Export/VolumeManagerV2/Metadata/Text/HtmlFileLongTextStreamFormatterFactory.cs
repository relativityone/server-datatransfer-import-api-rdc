using System.IO;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
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