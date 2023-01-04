using System.Collections.Generic;
using SQLDataComparer.Config;

namespace SQLDataComparer.Model
{
	public class Table
	{
		public string Name { get; set; }
		public string RowId { get; set; }
		public Dictionary<string, int> Columns { get; set; } = new Dictionary<string, int>();
		public List<Row> Rows { get; set; } = new List<Row>();
		public HashSet<string> Ignores { get; set; } = new HashSet<string>();
		public SideEnum Side { get; set; }

		public Dictionary<string, KeyValuePair<string, string>> MappedStrings { get; set; } =
			new Dictionary<string, KeyValuePair<string, string>>();

	}
}
