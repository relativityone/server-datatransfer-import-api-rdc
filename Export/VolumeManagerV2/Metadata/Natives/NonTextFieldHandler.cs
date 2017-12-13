using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.Helpers;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class NonTextFieldHandler
	{
		private readonly IFieldService _fieldLookupService;
		private readonly ILoadFileCellFormatter _loadFileCellFormatter;
		private readonly ExportFile _exportSettings;

		public NonTextFieldHandler(IFieldService fieldLookupService, ILoadFileCellFormatter loadFileCellFormatter, ExportFile exportSettings)
		{
			_fieldLookupService = fieldLookupService;
			_loadFileCellFormatter = loadFileCellFormatter;
			_exportSettings = exportSettings;
		}

		public void AddNonTextField(ViewFieldInfo field, DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			object rawFieldValue = artifact.Metadata[_fieldLookupService.GetOrdinalIndex(field.AvfColumnName)];
			string fieldValue = FieldValueHelper.ConvertToString(rawFieldValue, field, _exportSettings.MultiRecordDelimiter);
			loadFileEntry.AddStringEntry(_loadFileCellFormatter.TransformToCell(fieldValue));
		}
	}
}