using System.Collections;
using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class FieldService : FieldLookupService, IFieldService
	{
		private readonly ArrayList _columns;
		private readonly string _columnsHeader;

		public FieldService(ArrayList columns, string columnsHeader, Dictionary<string, int> ordinalLookup) : base(ordinalLookup)
		{
			_columns = columns;
			_columnsHeader = columnsHeader;
		}

		public ArrayList GetColumns()
		{
			return _columns;
		}

		public string GetColumnHeader()
		{
			return _columnsHeader;
		}
	}
}