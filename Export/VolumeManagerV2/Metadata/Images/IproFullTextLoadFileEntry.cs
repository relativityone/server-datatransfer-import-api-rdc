using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public abstract class IproFullTextLoadFileEntry : IFullTextLoadFileEntry
	{
		private TextReader _textReader;

		private readonly ExportFile _exportSettings;

		protected readonly IFieldService FieldService;
		protected readonly LongTextHelper LongTextHelper;

		protected IproFullTextLoadFileEntry(ExportFile exportSettings, IFieldService fieldService, LongTextHelper longTextHelper)
		{
			_exportSettings = exportSettings;
			FieldService = fieldService;
			LongTextHelper = longTextHelper;
		}

		public bool TryCreateFullTextLine(ObjectExportInfo artifact, string batesNumber, int pageNumber, long pageOffset, out KeyValuePair<string, string> fullTextEntry)
		{
			StringBuilder lineToWrite = new StringBuilder();

			if (pageNumber == 0)
			{
				_textReader?.Dispose();
				_textReader = GetTextStream(artifact);
			}

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

			fullTextEntry = new KeyValuePair<string, string>($"FT{batesNumber}", lineToWrite.ToString());

			return true;
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