using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Helpers;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class NonTextFieldHandler
	{
		private readonly IFieldService _fieldLookupService;
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;

		public NonTextFieldHandler(IFieldService fieldLookupService, ILoadFileCellFormatter loadFileCellFormatter, ExportFile exportSettings, ILog logger)
		{
			_fieldLookupService = fieldLookupService;
			_loadFileCellFormatter = loadFileCellFormatter;
			_exportSettings = exportSettings;
			_logger = logger;
		}

		public void AddNonTextField(ViewFieldInfo field, DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			_logger.LogVerbose("Adding field to load files entries.");
			object rawFieldValue = artifact.Metadata[_fieldLookupService.GetOrdinalIndex(field.AvfColumnName)];
			string fieldValue = FieldValueHelper.ConvertToString(rawFieldValue, field, _exportSettings.MultiRecordDelimiter);
			loadFileEntry.AddStringEntry(_loadFileCellFormatter.TransformToCell(fieldValue));
		}
	}
}