using System.Collections.Generic;
using System.Linq;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class ColumnsFactory : IColumnsFactory
	{
		private readonly ILog _logger;

		public ColumnsFactory(ILog logger)
		{
			_logger = logger;
		}

		public ViewFieldInfo[] CreateColumns(ExportFile exportSettings)
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

			if (textFields.Length == 1 && longTextViewFields.Any(x => x.Equals(textFields.First())))
			{
				ViewFieldInfo fieldToRemove = longTextViewFields.FirstOrDefault(x => x.Equals(textFields.First()));
				if (fieldToRemove != null)
				{
					int index = columns.IndexOf(fieldToRemove);
					_logger.LogVerbose("Found field {fieldToReplace} at index {index} to replace with long text field {longTextField}.", fieldToRemove.AvfColumnName, index,
						textFields.First().AvfColumnName);
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