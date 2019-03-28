using System.Collections.Generic;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class FieldService : FieldLookupService, IFieldService
	{
		private readonly ViewFieldInfo[] _columns;
		private readonly string _columnsHeader;

		public FieldService(ViewFieldInfo[] columns, string columnsHeader, Dictionary<string, int> ordinalLookup) : base(ordinalLookup)
		{
			_columns = columns;
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