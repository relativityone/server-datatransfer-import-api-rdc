using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class FieldService : FieldLookupService, IFieldService
	{
		private readonly ViewFieldInfo[] _columns;
		private readonly string _columnsHeader;

		public FieldService(ArrayList columns, string columnsHeader, Dictionary<string, int> ordinalLookup) : base(ordinalLookup)
		{
			_columns = columns.Cast<ViewFieldInfo>().ToArray();
			_columnsHeader = columnsHeader;
		}

		public ViewFieldInfo[] GetColumns()
		{
			return _columns;
		}

		public string GetColumnHeader()
		{
			return _columnsHeader;
		}
	}
}