using System.Linq;
using System.Text;
using kCura.WinEDDS.Exporters;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata
{
	public static class FieldUtils
	{
		public const string EXTRACTED_TEXT_COLUMN_NAME = "ExtractedText";
		public const int EXTRACTED_TEXT_FIELD_ID = -1;

		public static bool IsTextField(this FieldTypeHelper.FieldType fieldType)
		{
			return fieldType == FieldTypeHelper.FieldType.Text || fieldType == FieldTypeHelper.FieldType.OffTableText;
		}

		public static bool IsTextPrecedenceSet(this ExportFile exportSettings)
		{
			if (exportSettings.SelectedTextFields == null || exportSettings.SelectedTextFields.Length == 0)
			{
				return false;
			}
			return exportSettings.SelectedTextFields.Any(x => x != null);
		}

		/// <summary>
		///     TODO move to LongTextRepository or something
		/// </summary>
		/// <param name="artifact"></param>
		/// <param name="fieldLookupService"></param>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public static bool IsTextTooLong(this ObjectExportInfo artifact, IFieldLookupService fieldLookupService, string columnName)
		{
			string text = artifact.GetText(fieldLookupService, columnName);

			if (text == Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN)
			{
				return true;
			}

			return false;
		}

		public static string GetText(this ObjectExportInfo artifact, IFieldLookupService fieldLookupService, string columnName)
		{
			object rawText = artifact.Metadata[fieldLookupService.GetOrdinalIndex(columnName)];

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
	}
}