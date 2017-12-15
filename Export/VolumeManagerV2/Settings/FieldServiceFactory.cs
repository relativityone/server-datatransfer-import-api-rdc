using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;
using Relativity;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class FieldServiceFactory
	{
		private readonly ILoadFileHeaderFormatter _formatter;
		private readonly ILog _logger;

		public FieldServiceFactory(ILoadFileHeaderFormatter formatter, ILog logger)
		{
			_formatter = formatter;
			_logger = logger;
		}

		public FieldService Create(ExportFile exportSettings, string[] columnNamesInOrder)
		{
			DecideIfExportingFullText(exportSettings);

			ViewFieldInfo[] columns = CreateColumns(exportSettings);

			string columnsHeader = _formatter.GetHeader(columns.ToList());

			Dictionary<string, int> ordinalLookup = CreateOrdinalLookup(exportSettings, columnNamesInOrder);

			_logger.LogVerbose("Creating FieldService with {columnsCount} columns. Load file header {columnsHeader}.", columns.Length, columnsHeader);
			return new FieldService(columns, columnsHeader, ordinalLookup);
		}

		private void DecideIfExportingFullText(ExportFile exportSettings)
		{
			exportSettings.ExportFullText = exportSettings.ExportFullText || exportSettings.SelectedViewFields.Any(x => x.Category == FieldCategory.FullText);
			_logger.LogVerbose("Exporting full text: {value}.", exportSettings.ExportFullText);
		}

		private Dictionary<string, int> CreateOrdinalLookup(ExportFile exportSettings, string[] columnNamesInOrder)
		{
			var ordinalLookup = new Dictionary<string, int>();
			for (int i = 0; i < columnNamesInOrder.Length; i++)
			{
				ordinalLookup.Add(columnNamesInOrder[i], i);
			}
			if (exportSettings.SelectedTextFields != null && exportSettings.SelectedTextFields.Length > 0)
			{
				_logger.LogVerbose("Text Precedence is set. Adding TextPrecedence column {textPrecedenceColumn} and TextPrecedence source column {textPrecedenceSourceColumn}.",
					Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME, Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME);
				int newIndex = ordinalLookup.Count;
				ordinalLookup.Add(Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME, newIndex);
				ordinalLookup.Add(Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME, newIndex + 1);
			}
			return ordinalLookup;
		}

		private ViewFieldInfo[] CreateColumns(ExportFile exportSettings)
		{
			_logger.LogVerbose("Creating column list for export.");
			ViewFieldInfo[] viewFields = exportSettings.SelectedViewFields;
			ViewFieldInfo[] textFields = exportSettings.SelectedTextFields;
			
			if (textFields == null || textFields.Length == 0)
			{
				_logger.LogVerbose("No long text fields selected. Continuing with provided column list.");
				return viewFields.ToArray();
			}

			List<ViewFieldInfo> columns = viewFields.ToList();

			ViewFieldInfo[] longTextViewFields = viewFields.Where(x => x.FieldType == FieldTypeHelper.FieldType.Text || x.FieldType == FieldTypeHelper.FieldType.OffTableText).ToArray();

			if (longTextViewFields.Length == 1 && longTextViewFields.Any(x => x == textFields.First()))
			{
				ViewFieldInfo fieldToRemove = longTextViewFields.FirstOrDefault(x => x == textFields.First());
				if (fieldToRemove != null)
				{
					int index = columns.IndexOf(fieldToRemove);
					_logger.LogVerbose("Found field {fieldToReplace} at index {index} to replace with long text field {longTextField}.", fieldToRemove.AvfColumnName, index, textFields.First().AvfColumnName);
					columns[index] = new CoalescedTextViewField(textFields.First(), true);
				}
				else
				{
					_logger.LogVerbose("Adding missing CoalescedTextViewField for selected long text field {field}.", textFields.First().AvfColumnName);
					columns.Add(new CoalescedTextViewField(textFields.First(), false));
				}
			}
			else
			{
				_logger.LogVerbose("Adding missing CoalescedTextViewField for selected long text field {field}.", textFields.First().AvfColumnName);
				columns.Add(new CoalescedTextViewField(textFields.First(), false));
			}

			return columns.ToArray();
		}
	}
}