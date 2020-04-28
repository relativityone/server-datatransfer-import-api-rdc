using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class ChoiceMappingEqualityComparer : RowMappingEqualityComparer
	{
		public ChoiceMappingEqualityComparer(ILog log, Dictionary<string, string> mappingTable, string tableName)
		 : base(log, mappingTable, tableName)
		{
		}

		protected override List<string> GetDifferences(Row leftRow, Row rightRow)
		{
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
