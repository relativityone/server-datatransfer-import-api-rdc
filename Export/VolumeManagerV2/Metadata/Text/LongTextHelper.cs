using System;
using System.Linq;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Exporters;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text
{
	public class LongTextHelper
	{
		private const int _MISSING_EXTRACTED_TEXT_FIELD_ID = -1;

		private readonly ExportFile _exportSettings;
		private readonly IFieldService _fieldService;
		private readonly DownloadedTextFilesRepository _downloadedTextFilesRepository;

		public const string EXTRACTED_TEXT_COLUMN_NAME = "ExtractedText";

		public LongTextHelper(ExportFile exportSettings, IFieldService fieldService, DownloadedTextFilesRepository downloadedTextFilesRepository)
		{
			_fieldService = fieldService;
			_downloadedTextFilesRepository = downloadedTextFilesRepository;
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
			int fieldIndex = _fieldService.GetOrdinalIndex(fieldName);
			if (fieldName == EXTRACTED_TEXT_COLUMN_NAME && IsExtractedTextMissing())
			{
				if (_exportSettings.LogFileFormat == LoadFileType.FileFormat.IPRO_FullText)
				{
					return _MISSING_EXTRACTED_TEXT_FIELD_ID;
				}
				throw new ArgumentException($"Field {fieldName} not found.");
			}
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
				int fieldArtifactId = (int) artifact.Metadata[_fieldService.GetOrdinalIndex(Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME)];
				return _exportSettings.SelectedTextFields.FirstOrDefault(x => x.FieldArtifactId == fieldArtifactId);
			}
			return null;
		}

		public ViewFieldInfo GetTextPrecedenceTrueField(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			if (field == null || field.AvfColumnName == Relativity.Export.Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME)
			{
				return GetTextPrecedenceField(artifact);
			}
			return field;
		}

		public string GetLongTextFileLocation(ObjectExportInfo artifact, ViewFieldInfo field)
		{
			return GetLongTextFileLocation(artifact, field.FieldArtifactId);
		}

		public string GetLongTextFileLocation(ObjectExportInfo artifact, int fieldArtifactId)
		{
			return _downloadedTextFilesRepository.GetTextFileLocation(artifact.ArtifactID, fieldArtifactId);
		}
	}
}