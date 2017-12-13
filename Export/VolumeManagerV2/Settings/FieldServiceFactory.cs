using System.Collections;
using System.Collections.Generic;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class FieldServiceFactory
	{
		private readonly ILog _logger;

		public FieldServiceFactory(ILog logger)
		{
			_logger = logger;
		}

		/// <summary>
		///     TODO creating columns and columnsHeader is in LoadColumns methods in Exporter - extract
		/// </summary>
		/// <param name="exportSettings"></param>
		/// <param name="columns"></param>
		/// <param name="columnsHeader"></param>
		/// <param name="columnNamesInOrder"></param>
		/// <returns></returns>
		public FieldService Create(ExportFile exportSettings, ArrayList columns, string columnsHeader, string[] columnNamesInOrder)
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

			_logger.LogVerbose("Creating FieldService with {columnsCount} columns. Load file header {columnsHeader}.", columns.Count, columnsHeader);
			return new FieldService(columns, columnsHeader, ordinalLookup);
		}
	}
}