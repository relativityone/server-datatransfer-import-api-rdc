using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class ChoiceMappingEqualityComparer : RowMappingEqualityComparer
	{
		public ChoiceMappingEqualityComparer(ILog log, string mapId, Dictionary<string, string> mappingTable, string tableName)
		 : base(log, mapId, mappingTable, tableName)
		{
		}

		protected override List<string> GetDifferences(Row row1, Row row2)
		{
			ComparableRows rows = new ComparableRows(row1, row2);
			var leftRow = rows.LeftRow;
			var rightRow = rows.RightRow;

			var differences = new List<string>();

			if (leftRow.Values.Count != rightRow.Values.Count)
			{
				differences.Add($"{_tableName}.{leftRow.Id} : Number of columns, left: {leftRow.Values.Count}, right: {rightRow.Values.Count}");
			}

			differences.AddRange(GetMappingDifferences(leftRow[1], rightRow[1], leftRow.Id));

			return differences;
		}
	}
}
