﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.DataSize
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;

	public class TextExportableSize
	{
		private const long _EXTRACTED_TEXT_SIZE_NAIVE = 2097152;

		private readonly ExportFile _exportSettings;
		private readonly LongTextHelper _longTextHelper;
		private readonly IFieldService _fieldService;

		public TextExportableSize(ExportFile exportSettings, LongTextHelper longTextHelper, IFieldService fieldService)
		{
			_exportSettings = exportSettings;
			_longTextHelper = longTextHelper;
			_fieldService = fieldService;
		}

		public void CalculateTextSize(VolumePredictions volumeSize, ObjectExportInfo artifact)
		{
			bool isTextBeingExportedToFile = _exportSettings.ExportFullText && _exportSettings.ExportFullTextAsFile && _exportSettings.SelectedTextFields != null;
			if (isTextBeingExportedToFile)
			{
				List<kCura.WinEDDS.ViewFieldInfo> fields = _fieldService.GetColumns().Where(IsTextPrecedenceField).ToList();

				foreach (var field in fields)
				{
					volumeSize.TextFileCount += 1;

					string columnName = field.AvfColumnName;
					string textValue = _longTextHelper.GetTextFromField(artifact, columnName);

					if (textValue == ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN)
					{
						if (!_fieldService.ContainsFieldName(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE))
						{
							//This is just for backward compatibility
							volumeSize.TextFilesSize += _EXTRACTED_TEXT_SIZE_NAIVE;
						}
						else
						{
							int columnWithSizeIndex = _fieldService.GetOrdinalIndex(ServiceConstants.TEXT_PRECEDENCE_AWARE_TEXT_SIZE);
							long sizeInUnicode = (long) artifact.Metadata[columnWithSizeIndex];
							if (_exportSettings.TextFileEncoding.Equals(Encoding.Unicode))
							{
								volumeSize.TextFilesSize += sizeInUnicode;
							}
							else
							{
								long maxBytesForCharacters = EncodingFileSize.CalculateLongTextFileSize(sizeInUnicode, _exportSettings.TextFileEncoding);
								volumeSize.TextFilesSize += maxBytesForCharacters;
							}
						}
					}
					else
					{
						volumeSize.TextFilesSize += _exportSettings.TextFileEncoding.GetByteCount(textValue);
					}
				}
			}
			else
			{
				volumeSize.TextFileCount = 0;
				volumeSize.TextFilesSize = 0;
			}
		}

		private bool IsTextPrecedenceField(kCura.WinEDDS.ViewFieldInfo field)
		{
			return (field.FieldType == FieldType.Text || field.FieldType == FieldType.OffTableText) && field is CoalescedTextViewField;
		}
	}
}