﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Settings
{
	using System.Collections.Generic;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Service;
	using Relativity.Logging;

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
					ServiceConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME, ServiceConstants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME);
				int newIndex = ordinalLookup.Count;
				ordinalLookup.Add(ServiceConstants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME, newIndex);
				ordinalLookup.Add(ServiceConstants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME, newIndex + 1);
			}

			return ordinalLookup;
		}
	}
}