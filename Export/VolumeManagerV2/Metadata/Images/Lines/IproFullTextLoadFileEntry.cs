using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines
{
	public abstract class IproFullTextLoadFileEntry : IFullTextLoadFileEntry
	{
		private TextReader _textReader;

		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;

		protected readonly IFieldService FieldService;
		protected readonly LongTextHelper LongTextHelper;

		protected IproFullTextLoadFileEntry(ExportFile exportSettings, IFieldService fieldService, LongTextHelper longTextHelper, ILog logger)
		{
			_exportSettings = exportSettings;
			FieldService = fieldService;
			LongTextHelper = longTextHelper;
			_logger = logger;
		}

		public bool TryCreateFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, out KeyValuePair<string, string> fullTextEntry)
		{
			_logger.LogVerbose("Attempting to create Full text entry for image {batesNumber}.", batesNumber);
			if (pageNumber == 0)
			{
				_logger.LogVerbose("Processing first page - disposing old Text Reader and creating new one.");
				_textReader?.Dispose();
				_textReader = GetTextStream(artifact);
			}

			string lineToWrite = BuildLineToWrite(batesNumber, pageOffset);

			fullTextEntry = new KeyValuePair<string, string>($"FT{batesNumber}", lineToWrite);

			_logger.LogVerbose("Successfully create Full text entry for image.");

			return true;
		}

		private string BuildLineToWrite(string batesNumber, long pageOffset)
		{
			//TODO REL-185532 This is an issue. We're putting whole ExtractedText into memory
			//we should be able to write to file directly instead of storing metadata in memory
			StringBuilder lineToWrite = new StringBuilder();

			lineToWrite.Append("FT,");
			lineToWrite.Append(batesNumber);
			lineToWrite.Append(",1,1,");
			if (pageOffset == long.MinValue)
			{
				int c = _textReader.Read();
				while (c != -1)
				{
					lineToWrite.Append(GetLfpFullTextTransform(c));
					c = _textReader.Read();
				}
			}
			else
			{
				int i = 0;
				int c = _textReader.Read();
				while (i < pageOffset && c != -1)
				{
					lineToWrite.Append(GetLfpFullTextTransform(c));
					c = _textReader.Read();
					i++;
				}
			}
			lineToWrite.Append(Environment.NewLine);

			return lineToWrite.ToString();
		}

		private TextReader GetTextStream(ObjectExportInfo artifact)
		{
			if (LongTextHelper.IsTextTooLong(artifact, GetTextColumnName()))
			{
				string fileLocation = LongTextHelper.GetLongTextFileLocation(artifact, GetTextSourceFieldId(artifact));
				return new StreamReader(fileLocation, _exportSettings.LoadFileEncoding);
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