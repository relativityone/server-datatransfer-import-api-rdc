using System.Collections.Generic;
using System.Linq;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class FieldServiceFactory
	{
		private readonly IColumnsOrdinalLookupFactory _ordinalLookupFactory;
		private readonly IColumnsFactory _columnsFactory;
		private readonly ILog _logger;

		public FieldServiceFactory(IColumnsOrdinalLookupFactory ordinalLookupFactory, IColumnsFactory columnsFactory, ILog logger)
		{
			_ordinalLookupFactory = ordinalLookupFactory;
			_columnsFactory = columnsFactory;
			_logger = logger;
		}

		public FieldService Create(ExportFile exportSettings, string[] columnNamesInOrder, string columnsHeader)
		{
			DecideIfExportingFullText(exportSettings);

			ViewFieldInfo[] columns = _columnsFactory.CreateColumns(exportSettings);

			Dictionary<string, int> ordinalLookup = _ordinalLookupFactory.CreateOrdinalLookup(exportSettings, columnNamesInOrder);

			_logger.LogVerbose("Creating FieldService with {columnsCount} columns. Load file header {columnsHeader}.", columns.Length, columnsHeader);
			return new FieldService(columns, columnsHeader, ordinalLookup);
		}

		private void DecideIfExportingFullText(ExportFile exportSettings)
		{
			exportSettings.ExportFullText = exportSettings.ExportFullText || exportSettings.SelectedViewFields.Any(x => x.Category == FieldCategory.FullText);
			_logger.LogVerbose("Exporting full text: {value}.", exportSettings.ExportFullText);
		}
	}
}