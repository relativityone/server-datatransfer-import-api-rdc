using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class FromFieldToLoadFileWriter : ILongTextEntryWriter
	{
		private readonly LongTextStreamFormatterFactory _formatterFactory;
		private readonly ILog _logger;

		public FromFieldToLoadFileWriter(ILog logger, LongTextStreamFormatterFactory formatterFactory)
		{
			_logger = logger;
			_formatterFactory = formatterFactory;
		}

		public void WriteLongTextFileToDatFile(StreamWriter writer, string text, Encoding encoding)
		{
			using (TextReader source = new StringReader(text))
			{
				ILongTextStreamFormatter formatter = _formatterFactory.Create(source);
				WriteLongText(source, writer, formatter);
			}
		}

		private void WriteLongText(TextReader source, TextWriter fileWriter, ILongTextStreamFormatter formatter)
		{
			int c = source.Read();

			while (c != -1)
			{
				formatter.TransformAndWriteCharacter(c, fileWriter);
				c = source.Read();
			}
		}
	}
}