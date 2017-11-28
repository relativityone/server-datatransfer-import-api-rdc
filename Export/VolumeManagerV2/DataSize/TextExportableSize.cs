using System.Collections;
using System.Collections.Generic;
using System.Text;
using kCura.WinEDDS.Exporters;
using Relativity;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.DataSize
{
	public class TextExportableSize
	{
		private readonly ExportFile _exportSettings;
		private readonly ArrayList _columns;
		private readonly Dictionary<string, int> _ordinalLookup;

		public TextExportableSize(ExportFile exportSettings, ArrayList columns, Dictionary<string, int> ordinalLookup)
		{
			_exportSettings = exportSettings;
			_columns = columns;
			_ordinalLookup = ordinalLookup;
		}

		public void CalculateTextSize(VolumePredictions volumeSize, ObjectExportInfo artifact)
		{
			if (_exportSettings.ExportFullText && _exportSettings.ExportFullTextAsFile)
			{
				for (int count = 0; count <= _columns.Count - 1; count++)
				{
					ViewFieldInfo field = (ViewFieldInfo) _columns[count];
					string columnName = field.AvfColumnName;
					object fieldValue = artifact.Metadata[_ordinalLookup[columnName]];
					if (field.FieldType == FieldTypeHelper.FieldType.Text || field.FieldType == FieldTypeHelper.FieldType.OffTableText)
					{
						if (_exportSettings.SelectedTextFields != null && field is CoalescedTextViewField)
						{
							volumeSize.TextFileCount += 1;
							if (fieldValue is byte[])
							{
								fieldValue = Encoding.Unicode.GetString((byte[]) fieldValue);
							}
							if (fieldValue == null)
							{
								fieldValue = string.Empty;
							}
							string textValue = fieldValue.ToString();
							if (textValue == Constants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN)
							{
								//TODO
								//This is defect, fix that!!! REL-181870
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
				//We're not exporting files with text
				volumeSize.TextFileCount = 0;
				volumeSize.TextFilesSize = 0;
			}
		}
	}
}