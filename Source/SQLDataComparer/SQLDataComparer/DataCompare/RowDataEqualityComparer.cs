﻿using System;
using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class RowDataEqualityComparer : RowEqualityComparer
	{
		public RowDataEqualityComparer(ILog log, Dictionary<string, string> mappingTable, string tableName)
		: base(log, mappingTable, tableName)
		{
		}

		protected override List<string> GetDifferences(Row leftRow, Row rightRow)
		{
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

					if (leftValue != rightValue)
					{
						differences.Add($"{_tableName} : Rows with {leftRow.Table.RowId} = {leftRow[leftRow.Table.RowId]} have different {column.Key}. Left: {leftValue}, Right: {rightValue}");
					}
				}
			}

			return differences;
		}

		protected override void AddMappingsToMappingTable(List<Row> leftRows, List<Row> rightRows, Dictionary<int, int> matchedRows)
		{
			foreach (var pair in matchedRows)
			{
				AddRowsMappingsToMappingTable(leftRows[pair.Value], rightRows[pair.Key]);
			}
		}

		protected void AddRowsMappingsToMappingTable(Row leftRow, Row rightRow)
		{
			string leftValue = leftRow["ArtifactID"];
			string rightValue = rightRow["ArtifactID"];

			if (string.IsNullOrEmpty(leftValue) || string.IsNullOrEmpty(rightValue))
			{
				throw new Exception($"{_tableName} : ArtifactID missing for table. Left: {leftValue} Right: {rightValue}");
			}

			if (!_mappingTable.ContainsKey(leftValue))
			{
				_mappingTable.Add(leftValue, rightValue);
			}
			else if (_mappingTable[leftValue] != rightValue)
			{
				throw new Exception($"Mapping was already mapped differently for {leftValue} : {_mappingTable[leftValue]} != {rightValue}");
			}
		}
	}
}
