using System.IO;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	/// <summary>
	///     TODO factory of factory?
	/// </summary>
	public class LongTextStreamFormatterFactory
	{
		private readonly ExportFile _exportSettings;

		public LongTextStreamFormatterFactory(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public ILongTextStreamFormatter Create(TextReader source)
		{
			if (_exportSettings.LoadFileIsHtml)
			{
				return new HtmlFileLongTextStreamFormatter(_exportSettings, source);
			}
			return new DelimitedFileLongTextStreamFormatter(_exportSettings, source);
		}
	}
}