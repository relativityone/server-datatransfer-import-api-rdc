using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class ArtifactMappingEqualityComparer : RowEqualityComparer
	{
		public ArtifactMappingEqualityComparer(ILog log, Dictionary<string, string> mappingTable, string tableName)
			: base(log, "ArtifactID", mappingTable, tableName)
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
				differences.Add($"{_tableName} : Number of columns, left: {leftRow.Values.Count}, right: {rightRow.Values.Count}");
			}

			foreach (var column in leftRow.Table.Columns)
			{
				if (column.Key != leftRow.Table.RowId && !leftRow.Table.Ignores.Contains(column.Key))
				{
					string leftValue = leftRow[column.Key];
					string rightValue = rightRow[column.Key];

					bool identical = leftValue == rightValue;

					if (!identical && leftRow.Table.MappedStrings.TryGetValue(column.Key, out var keyValuePair)
					               && keyValuePair.Key == leftValue && keyValuePair.Value == rightValue)
					{
						identical = true;
					}

					if (!identical)
					{
						differences.Add($"{_tableName} : Rows with {leftRow.Table.RowId} = {leftRow[leftRow.Table.RowId]} have different {column.Key}. Left: {leftValue}, Right: {rightValue}");
					}
				}
			}

			return differences;
		}

		protected override void AddMappingsToMappingTable(IDictionary<Row, Row> matchedRows)
		{
		}
	}
}
