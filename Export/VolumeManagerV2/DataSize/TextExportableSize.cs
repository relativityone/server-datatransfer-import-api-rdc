using System.Text;
using kCura.WinEDDS.Exporters;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize
{
	public class TextExportableSize
	{
		private readonly ExportFile _exportSettings;
		private readonly IFieldService _fieldService;

		public TextExportableSize(ExportFile exportSettings, IFieldService fieldService)
		{
			_exportSettings = exportSettings;
			_fieldService = fieldService;
		}

		public void CalculateTextSize(VolumePredictions volumeSize, ObjectExportInfo artifact)
		{
			bool isTextBeingExportedToFile = _exportSettings.ExportFullText && _exportSettings.ExportFullTextAsFile;
			if (isTextBeingExportedToFile)
			{
				for (int count = 0; count <= _fieldService.GetColumns().Length - 1; count++)
				{
					ViewFieldInfo field = _fieldService.GetColumns()[count];
					string columnName = field.AvfColumnName;
					int columnIndex = _fieldService.GetOrdinalIndex(columnName);
					object fieldValue = artifact.Metadata[columnIndex];
					if (field.FieldType == FieldTypeHelper.FieldType.Text || field.FieldType == FieldTypeHelper.FieldType.OffTableText)
					{
						if (_exportSettings.SelectedTextFields != null && field is CoalescedTextViewField)
						{
							volumeSize.TextFileCount += 1;
							if (fieldValue is byte[])
							{
								fieldValue = Encoding.Unicode.GetString((byte[]) fieldValue);
							}
							fieldValue = fieldValue ?? string.Empty;
							string textValue = fieldValue.ToString();
							if (textValue == Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN)
							{
								int columnWithSizeIndex = _fieldService.GetOrdinalIndex(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE);
								long sizeInUnicode = (long) artifact.Metadata[columnWithSizeIndex];
								if (_exportSettings.TextFileEncoding.Equals(Encoding.Unicode))
								{
									volumeSize.TextFilesSize += sizeInUnicode;
								}
								else
								{
									long maxBytesForCharacters = CalculateLongTextFileSize(sizeInUnicode);
									volumeSize.TextFilesSize += maxBytesForCharacters;
								}
							}
							else
							{
								volumeSize.TextFilesSize += _exportSettings.TextFileEncoding.GetByteCount(textValue);
							}
						}
					}
				}
			}
			else
			{
				volumeSize.TextFileCount = 0;
				volumeSize.TextFilesSize = 0;
			}
		}

		/// <summary>
		/// This calculation is not precise, but it will always return value bigger than real file size
		/// </summary>
		/// <param name="sizeInUnicode"></param>
		/// <returns></returns>
		private long CalculateLongTextFileSize(long sizeInUnicode)
		{
			long maxCharactersEncoded = CalculateMaxCharactersCountInEncoding(Encoding.Unicode, sizeInUnicode);
			long maxBytesForCharacters = CalculateMaxBytesForCharactersCountInEncoding(_exportSettings.TextFileEncoding, maxCharactersEncoded);
			return maxBytesForCharacters;
		}

		/// <summary>
		/// Encoding doesn't contain GetMaxCharCount method for Int64 type
		/// </summary>
		/// <param name="encoding"></param>
		/// <param name="bytes"></param>
		/// <returns></returns>
		private long CalculateMaxCharactersCountInEncoding(Encoding encoding, long bytes)
		{
			const int bytesSample = 1024;
			int maxCharactersForSample = encoding.GetMaxCharCount(bytesSample);
			long maxCharactersForBytes = (bytes / bytesSample + 1) * maxCharactersForSample;
			return maxCharactersForBytes;
		}


		/// <summary>
		/// Encoding doesn't contain GetMaxByteCount method for Int64 type
		/// </summary>
		/// <param name="encoding"></param>
		/// <param name="bytes"></param>
		/// <returns></returns>
		private long CalculateMaxBytesForCharactersCountInEncoding(Encoding encoding, long characters)
		{
			const int charactersSample = 1024;
			int maxBytesForSample = encoding.GetMaxByteCount(charactersSample);
			long maxBytesForCharacters = (characters / charactersSample + 1) * maxBytesForSample;
			return maxBytesForCharacters;
		}
	}
}