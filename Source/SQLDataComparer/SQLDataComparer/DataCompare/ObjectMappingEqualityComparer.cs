using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class ObjectMappingEqualityComparer : RowMappingEqualityComparer
	{
		public ObjectMappingEqualityComparer(ILog log, string mapId, Dictionary<string, string> mappingTable, string tableName)
		 : base(log, mapId, mappingTable, tableName)
		{
		}

		protected override List<string> GetDifferences(Row leftRow, Row rightRow)
		{
			var differences = new List<string>();

			if (leftRow.Values.Count != rightRow.Values.Count)
			{
				differences.Add($"{_tableName}.{leftRow.Id} : Number of columns, left: {leftRow.Values.Count}, right: {rightRow.Values.Count}");
			}

			differences.AddRange(GetMappingDifferences(leftRow[2], rightRow[2], leftRow.Id));

			return differences;
		}
	}
}
