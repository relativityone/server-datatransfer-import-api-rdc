using System.Collections.Generic;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class ColumnsOrdinalLookupFactory : IColumnsOrdinalLookupFactory
	{
		private readonly ILog _logger;

		public ColumnsOrdinalLookupFactory(ILog logger)
		{
			_logger = logger;
		}

		public Dictionary<string, int> CreateOrdinalLookup(ExportFile exportSettings, string[] columnNamesInOrder)
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
	}
}