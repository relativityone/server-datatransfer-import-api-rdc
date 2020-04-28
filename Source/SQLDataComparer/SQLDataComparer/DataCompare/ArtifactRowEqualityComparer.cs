using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class ArtifactRowEqualityComparer : RowDataEqualityComparer
	{
		public ArtifactRowEqualityComparer(ILog log, Dictionary<string, string> mappingTable, string tableName)
			: base(log, mappingTable, tableName)
		{
		}

		protected override void AddMappingsToMappingTable(List<Row> leftRows, List<Row> rightRows, Dictionary<int, int> matchedRows)
		{
		}
	}
}
