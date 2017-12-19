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
								//TODO REL-181870 This is defect, fix that!!! 
								volumeSize.TextFilesSize += 2 * 1048576; //This is the naive approach - assume the final text will be twice as long as the max length limit
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
	}
}