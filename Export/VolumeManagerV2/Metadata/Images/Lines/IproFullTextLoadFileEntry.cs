using System;
using System.IO;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public abstract class IproFullTextLoadFileEntry : IFullTextLoadFileEntry
	{
		private TextReader _textReader;

		private readonly ILog _logger;

		protected readonly IFieldService FieldService;
		protected readonly LongTextHelper LongTextHelper;

		protected IproFullTextLoadFileEntry(IFieldService fieldService, LongTextHelper longTextHelper, ILog logger)
		{
			FieldService = fieldService;
			LongTextHelper = longTextHelper;
			_logger = logger;
		}

		public void WriteFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, IRetryableStreamWriter writer, CancellationToken token)
		{
			_logger.LogVerbose("Attempting to create Full text entry for image {batesNumber}.", batesNumber);
			if (pageNumber == 0)
			{
				_logger.LogVerbose("Processing first page - disposing old Text Reader and creating new one.");
				_textReader?.Dispose();
				_textReader = GetTextStream(artifact);
			}

			WriteLine(batesNumber, pageOffset, writer, token);

			_logger.LogVerbose("Successfully create Full text entry for image.");
		}

		private void WriteLine(string batesNumber, long pageOffset, IRetryableStreamWriter writer, CancellationToken token)
		{
			writer.WriteChunk("FT,", token);
			writer.WriteChunk(batesNumber, token);
			writer.WriteChunk(",1,1,", token);
			if (pageOffset == long.MinValue)
			{
				int c = _textReader.Read();
				while (c != -1)
				{
					writer.WriteChunk(GetLfpFullTextTransform(c), token);
					c = _textReader.Read();
				}
			}
			else
			{
				int i = 0;
				int c = _textReader.Read();
				while (i < pageOffset && c != -1)
				{
					writer.WriteChunk(GetLfpFullTextTransform(c), token);
					c = _textReader.Read();
					i++;
				}
			}

			writer.WriteChunk(Environment.NewLine, token);
			writer.FlushChunks(token);
		}

		private TextReader GetTextStream(ObjectExportInfo artifact)
		{
			if (LongTextHelper.IsTextTooLong(artifact, GetTextColumnName()))
			{
				string fileLocation = LongTextHelper.GetLongTextFileLocation(artifact, GetTextSourceFieldId(artifact));
				return new StreamReader(fileLocation);
			}

			string text = LongTextHelper.GetTextFromField(artifact, GetTextColumnName());
			return new StringReader(text);
		}

		protected abstract int GetTextSourceFieldId(ObjectExportInfo artifact);

		protected abstract string GetTextColumnName();

		private string GetLfpFullTextTransform(int c)
		{
			const int lineFeed = 10;
			if (c == lineFeed || c == ' ')
			{
				return "|0|0|0|0^";
			}

			if (c == ',')
			{
				return "";
			}

			return Convert.ToChar(c).ToString();
		}

		public void Dispose()
		{
			_textReader?.Dispose();
		}
	}
}