﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System.IO;
	using System.Text;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Logging;

	public class FromFileToLoadFileWriter : ToLoadFileWriter
	{
		private readonly ILongTextStreamFormatterFactory _formatterFactory;
		private readonly ILog _logger;

		public FromFileToLoadFileWriter(ILog logger, ILongTextStreamFormatterFactory formatterFactory)
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