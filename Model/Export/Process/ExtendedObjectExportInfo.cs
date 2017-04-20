using System.Collections.Generic;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Model.Export.Process
{
	public class ExtendedObjectExportInfo : ObjectExportInfo
	{
		private readonly IFieldLookupService _fieldLookupService;

		public ExtendedObjectExportInfo(IFieldLookupService fieldLookupService)
		{
			_fieldLookupService = fieldLookupService;
		}
		public List<ViewFieldInfo> SelectedNativeFileNameViewFields { get; set; }

		public object GetFieldValue(string fieldName)
		{
			return Metadata[_fieldLookupService.GetOrdinalIndex(fieldName)];
		}

	}
}
