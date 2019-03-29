﻿using System;
using System.Linq;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity;
using ExportConstants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextHelper
	{
		private const int _MISSING_EXTRACTED_TEXT_FIELD_ID = -1;

		private readonly ExportFile _exportSettings;
		private readonly IFieldService _fieldService;
		private readonly LongTextRepository _longTextRepository;

		public const string EXTRACTED_TEXT_COLUMN_NAME = "ExtractedText";

		public LongTextHelper(ExportFile exportSettings, IFieldService fieldService, LongTextRepository longTextRepository)
		{
			_fieldService = fieldService;
			_longTextRepository = longTextRepository;
			_exportSettings = exportSettings;
		}

		public bool IsLongTextField(ViewFieldInfo fieldInfo)
		{
			return IsLongTextField(fieldInfo.FieldType);
		}

		public bool IsLongTextField(FieldTypeHelper.FieldType fieldType)
		{
			return fieldType == FieldTypeHelper.FieldType.Text || fieldType == FieldTypeHelper.FieldType.OffTableText;
		}

		public string GetTextFromField(ObjectExportInfo artifact, string fieldName)
		{
			object rawText = artifact.Metadata[_fieldService.GetOrdinalIndex(fieldName)];

			if (rawText is byte[])
			{
				rawText = Encoding.Unicode.GetString((byte[]) rawText);
			}

			if (rawText != null)
			{
				return rawText.ToString();
			}

			return string.Empty;
		}

		public bool IsTextTooLong(string text)
		{
			return text == Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;
		}

		public bool IsTextTooLong(ObjectExportInfo artifact, string fieldName)
		{
			string text = GetTextFromField(artifact, fieldName);
			return IsTextTooLong(text);
		}

		public bool IsExtractedTextMissing()
		{
			int fieldIndex = _fieldService.GetOrdinalIndex(EXTRACTED_TEXT_COLUMN_NAME);
			return fieldIndex >= _fieldService.GetColumns().Length;
		}

		public int GetFieldArtifactId(string fieldName)
		{
			if (fieldName == EXTRACTED_TEXT_COLUMN_NAME && IsExtractedTextMissing())
			{
				if (_exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
				{
					return _MISSING_EXTRACTED_TEXT_FIELD_ID;
				}

				throw new ArgumentException($"Field {fieldName} not found.");
			}

			int fieldIndex = _fieldService.GetOrdinalIndex(fieldName);
			return _fieldService.GetColumns()[fieldIndex].FieldArtifactId;
		}

		public bool IsTextPrecedenceSet()
		{
			if (_exportSettings.SelectedTextFields == null || _exportSettings.SelectedTextFields.Length == 0)
			{
				return false;
			}

			return _exportSettings.SelectedTextFields.Any(x => x != null);
		}

		public ViewFieldInfo GetTextPrecedenceField(ObjectExportInfo artifact)
		{
			if (_exportSettings.SelectedTextFields != null)
			{
				int fieldArtifactId = (int) artifact.Metadata[_fieldService.GetOrdinalIndex(ExportConstants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)];
				return _exportSettings.SelectedTextFields.FirstOrDefault(x => x.FieldArtifactId == fieldArtifactId);
			}

			return null;
		}

		public ViewFieldInfo GetTextPrecedenceTrueField(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			if (field == null || field.AvfColumnName == ExportConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)
			{
				return GetTextPrecedenceField(artifact);
			}

			return field;
		}

		public string GetLongTextFileLocation(ObjectExportInfo artifact, int fieldArtifactId)
		{
			return _longTextRepository.GetTextFileLocation(artifact.ArtifactID, fieldArtifactId);
		}

		public Encoding GetLongTextFieldFileEncoding(ViewFieldInfo field)
		{
			if (field.IsUnicodeEnabled)
			{
				return Encoding.Unicode;
			}

			return Encoding.Default;
		}
	}
}