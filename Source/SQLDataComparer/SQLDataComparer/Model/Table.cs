using System.Collections.Generic;

namespace SQLDataComparer.Model
{
	public class Table
	{
		public string Name { get; set; }
		public string RowId { get; set; }
		public Dictionary<string, int> Columns { get; set; } = new Dictionary<string, int>();
		public List<Row> Rows { get; set; } = new List<Row>();
		public HashSet<string> Ignores { get; set; } = new HashSet<string>();

	}
}
