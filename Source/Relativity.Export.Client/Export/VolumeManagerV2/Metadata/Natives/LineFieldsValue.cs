using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives
{
	public class LineFieldsValue : ILineFieldsValue
	{
		private readonly IFieldService _fieldLookupService;
		private readonly ILongTextHandler _longTextHandler;
		private readonly LongTextHelper _longTextHelper;
		private readonly NonTextFieldHandler _nonTextFieldHandler;
		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;

		public LineFieldsValue(IFieldService fieldLookupService, ILongTextHandler longTextHandler, LongTextHelper longTextHelper, NonTextFieldHandler nonTextFieldHandler,
			ExportFile exportSettings, ILog logger)
		{
			_fieldLookupService = fieldLookupService;
			_longTextHandler = longTextHandler;
			_longTextHelper = longTextHelper;
			_nonTextFieldHandler = nonTextFieldHandler;
			_exportSettings = exportSettings;
			_logger = logger;
		}

		public void AddFieldsValue(DeferredEntry loadFileEntry, ObjectExportInfo artifact)
		{
			List<ViewFieldInfo> fields = _fieldLookupService.GetColumns().ToList();
			for (int i = 0; i < fields.Count; i++)
			{
				ViewFieldInfo field = fields[i];

				_logger.LogVerbose("Adding field {field} value to load file entry.", field.AvfColumnName);

				if (_longTextHelper.IsLongTextField(field))
				{
					_longTextHandler.HandleLongText(artifact, field, loadFileEntry);
				}
				else
				{
					_nonTextFieldHandler.AddNonTextField(field, loadFileEntry, artifact);
				}

				if (i != fields.Count - 1 && !_exportSettings.LoadFileIsHtml)
				{
					loadFileEntry.AddStringEntry(_exportSettings.RecordDelimiter.ToString());
				}
			}
		}
	}
}