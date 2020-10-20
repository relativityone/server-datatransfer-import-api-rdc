using System.Collections.Generic;

namespace SQLDataComparer.Model
{
	public class Row
	{
		public string Id { get; set; }
		public List<string> Values { get; set; } = new List<string>();
		public Table Table { get; }

		public Row(Table table)
		{
			Table = table;
		}

		public string this[int index]
		{
			get => Values[index];
			set => Values[index] = value;
		}

		public string this[string column]
		{
			get => Values[Table.Columns[column]];
			set => Values[Table.Columns[column]] = value;
		}
	}
}
