namespace Relativity.Export.VolumeManagerV2.Metadata.Text
{
	using System.IO;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

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