using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class FromFieldToLoadFileWriter : ToLoadFileWriter
	{
		private readonly ILongTextStreamFormatterFactory _formatterFactory;
		private readonly ILog _logger;

		public FromFieldToLoadFileWriter(ILog logger, ILongTextStreamFormatterFactory formatterFactory)
		{
			_logger = logger;
			_formatterFactory = formatterFactory;
		}

		public override void WriteLongTextFileToDatFile(StreamWriter writer, string text, Encoding encoding)
		{
			_logger.LogVerbose("Writing value to load file.");
			using (TextReader source = new StringReader(text))
			{
				ILongTextStreamFormatter formatter = _formatterFactory.Create(source);
				WriteLongText(source, writer, formatter);
			}
		}
	}
}