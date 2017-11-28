using System.Collections;
using System.Collections.Generic;
using Relativity.Export;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class FieldServiceFactory
	{
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
				int newIndex = ordinalLookup.Count;
				ordinalLookup.Add(Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME, newIndex);
				ordinalLookup.Add(Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME, newIndex + 1);
			}

			return new FieldService(columns, columnsHeader, ordinalLookup);
		}
	}
}