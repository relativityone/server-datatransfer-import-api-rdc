namespace Relativity.DataExchange.Export.VolumeManagerV2.Settings
{
	using System.Collections.Generic;
	using System.Linq;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Service;
	using Relativity.Logging;

	public class ColumnsFactory : IColumnsFactory
	{
		private readonly ILog _logger;

		public ColumnsFactory(ILog logger)
		{
			_logger = logger;
		}

		public kCura.WinEDDS.ViewFieldInfo[] CreateColumns(ExportFile exportSettings)
		{
			_logger.LogVerbose("Creating column list for export.");
			kCura.WinEDDS.ViewFieldInfo[] viewFields = exportSettings.SelectedViewFields;
			kCura.WinEDDS.ViewFieldInfo[] textFields = exportSettings.SelectedTextFields;

			if (textFields == null || textFields.Length == 0)
			{
				_logger.LogVerbose("No long text fields selected. Continuing with provided column list.");
				return viewFields.ToArray();
			}

			List<kCura.WinEDDS.ViewFieldInfo> columns = viewFields.ToList();

			kCura.WinEDDS.ViewFieldInfo[] longTextViewFields = viewFields.Where(x => x.FieldType == FieldType.Text || x.FieldType == FieldType.OffTableText).ToArray();

			if (textFields.Length == 1 && longTextViewFields.Any(x => x.Equals(textFields.First())))
			{
				kCura.WinEDDS.ViewFieldInfo fieldToRemove = longTextViewFields.FirstOrDefault(x => x.Equals(textFields.First()));
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