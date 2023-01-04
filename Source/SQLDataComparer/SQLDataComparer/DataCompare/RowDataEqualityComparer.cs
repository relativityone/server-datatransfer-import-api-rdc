using System;
using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class RowDataEqualityComparer : RowEqualityComparer
	{
		public RowDataEqualityComparer(ILog log, string mapId, Dictionary<string, string> mappingTable, string tableName)
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
			foreach (var pair in matchedRows)
			{
				AddRowsMappingsToMappingTable(pair.Value, pair.Key);
			}
		}

		protected void AddRowsMappingsToMappingTable(Row leftRow, Row rightRow)
		{
			string leftValue = leftRow[_mapId];
			string rightValue = rightRow[_mapId];

			if (string.IsNullOrEmpty(leftValue) || string.IsNullOrEmpty(rightValue))
			{
				throw new Exception($"{_tableName} : {_mapId} missing for table. Left: {leftValue} Right: {rightValue}");
			}

			if (!_mappingTable.TryGetValue(leftValue, out string output))
			{
				_mappingTable.Add(leftValue, rightValue);
			}
			else if (output != rightValue)
			{
				throw new Exception($"Mapping was already mapped differently for {leftValue} : {_mappingTable[leftValue]} != {rightValue}");
			}
		}
	}
}
