using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineFieldsValue
	{
		private readonly IFieldService _fieldLookupService;
		private readonly ILongTextHandler _longTextHandler;
		private readonly LongTextHelper _longTextHelper;
		private readonly NonTextFieldHandler _nonTextFieldHandler;
		private readonly ExportFile _exportSettings;

		public LineFieldsValue(IFieldService fieldLookupService, ILongTextHandler longTextHandler, LongTextHelper longTextHelper, NonTextFieldHandler nonTextFieldHandler,
			ExportFile exportSettings)
		{
			_fieldLookupService = fieldLookupService;
			_longTextHandler = longTextHandler;
			_longTextHelper = longTextHelper;
			_nonTextFieldHandler = nonTextFieldHandler;
			_exportSettings = exportSettings;
		}

		public void AddFieldsValue(DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			List<ViewFieldInfo> fields = _fieldLookupService.GetColumns().ToList();
			for (int i = 0; i < fields.Count; i++)
			{
				ViewFieldInfo field = fields[i];

				if (_longTextHelper.IsLongTextField(field))
				{
					_longTextHandler.HandleLongText(artifact, field, loadFileEntry);
				}
				else
				{
					_nonTextFieldHandler.AddNonTextField(field, loadFileEntry, artifact);
				}

				//TODO maybe we can do something with this
				if (i != fields.Count - 1 && !_exportSettings.LoadFileIsHtml)
				{
					loadFileEntry.AddStringEntry(_exportSettings.RecordDelimiter.ToString());
				}
			}
		}
	}
}