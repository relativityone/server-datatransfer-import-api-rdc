using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images
{
	public abstract class IproFullTextLoadFileEntry : IFullTextLoadFileEntry
	{
		private TextReader _textReader;

		private readonly ExportFile _exportSettings;
		private readonly DownloadedTextFilesRepository _downloadedTextFilesRepository;

		protected readonly IFieldService FieldService;

		protected IproFullTextLoadFileEntry(ExportFile exportSettings, IFieldService fieldService, DownloadedTextFilesRepository downloadedTextFilesRepository)
		{
			_exportSettings = exportSettings;
			FieldService = fieldService;
			_downloadedTextFilesRepository = downloadedTextFilesRepository;
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
			string text = artifact.GetText(FieldService, GetTextColumnName());

			if (text == Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN)
			{
				string fileLocation = _downloadedTextFilesRepository.GetTextFileLocation(artifact.ArtifactID, GetTextSourceFieldId(artifact));
				return new StreamReader(fileLocation, _exportSettings.LoadFileEncoding);
			}

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