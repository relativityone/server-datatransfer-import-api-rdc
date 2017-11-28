using System.Collections;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class ExportColumns
	{
		public ArrayList Columns { get; }
		public IFieldLookupService FieldLookupService { get; }

		public ExportColumns(ArrayList columns, IFieldLookupService fieldLookupService)
		{
			Columns = columns;
			FieldLookupService = fieldLookupService;
		}
	}
}