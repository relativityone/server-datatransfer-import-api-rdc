using System.Collections;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Exporters;
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


		/// <summary>
		///     TODO creating columns is in LoadColumns methods in Exporter - extract
		/// </summary>
		/// <param name="exportSettings"></param>
		/// <param name="columns"></param>
		/// <param name="columnsHeader"></param>
		/// <param name="columnNamesInOrder"></param>
		/// <returns></returns>
		public FieldService Create(ExportFile exportSettings, ArrayList columns, string[] columnNamesInOrder)
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

			string columnsHeader = _formatter.GetHeader(columns.Cast<ViewFieldInfo>().ToList());

			_logger.LogVerbose("Creating FieldService with {columnsCount} columns. Load file header {columnsHeader}.", columns.Count, columnsHeader);
			return new FieldService(columns, columnsHeader, ordinalLookup);
		}
	}
}