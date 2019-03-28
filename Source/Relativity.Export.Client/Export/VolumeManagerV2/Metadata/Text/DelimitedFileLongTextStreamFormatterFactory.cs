using System.IO;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class DelimitedFileLongTextStreamFormatterFactory : ILongTextStreamFormatterFactory
	{
		private readonly ExportFile _exportSettings;

		public DelimitedFileLongTextStreamFormatterFactory(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public ILongTextStreamFormatter Create(TextReader source)
		{
			return new DelimitedFileLongTextStreamFormatter(_exportSettings, source);
		}
	}
}