using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class FromFileToLoadFileWriter : ToLoadFileWriter
	{
		private readonly LongTextStreamFormatterFactory _formatterFactory;
		private readonly ILog _logger;

		public FromFileToLoadFileWriter(ILog logger, LongTextStreamFormatterFactory formatterFactory)
		{
			_logger = logger;
			_formatterFactory = formatterFactory;
		}

		public override void WriteLongTextFileToDatFile(StreamWriter fileWriter, string longTextPath, Encoding encoding)
		{
			_logger.LogVerbose("Writing entry from file {path} to load file.", longTextPath);
			using (TextReader source = new StreamReader(longTextPath, encoding))
			{
				ILongTextStreamFormatter formatter = _formatterFactory.Create(source);
				WriteLongText(source, fileWriter, formatter);
			}
		}
	}
}