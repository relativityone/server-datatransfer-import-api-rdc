using SQLDataComparer.Log;
using System.Collections.Generic;
using SQLDataComparer.Model;


namespace SQLDataComparer.DataCompare
{
	public abstract class RowMappingEqualityComparer : RowEqualityComparer
	{
		protected RowMappingEqualityComparer(ILog log, string mapId, Dictionary<string, string> mappingTable, string tableName)
		 : base(log, mapId, mappingTable, tableName)
		{
		}

		protected List<string> GetMappingDifferences(string leftMapId, string rightMapId, string rowId)
		{
			var differences = new List<string>();

			if (!string.IsNullOrEmpty(leftMapId) && !string.IsNullOrEmpty(rightMapId))
			{
				if (!_mappingTable.ContainsKey(leftMapId))
				{
					// this means there are artifact ids missing from mapping table and we can't fully be sure whether those rows are equal or not
					_log.LogError($"{_tableName}.{rowId} : Missing {_mapId} in the mapping table: {leftMapId}");
				}
				else if (_mappingTable[leftMapId] != rightMapId)
				{
					differences.Add($"{_tableName}.{rowId} : Rows have different mapping {_mapId} : {_mappingTable[leftMapId]} != {rightMapId}");
				}
			}
			else if ((!string.IsNullOrEmpty(leftMapId) && string.IsNullOrEmpty(rightMapId))
						|| (string.IsNullOrEmpty(leftMapId) && !string.IsNullOrEmpty(rightMapId)))
			{
				differences.Add($"{_tableName}.{rowId} : Node mapped only on one side, left: {leftMapId}, right: {rightMapId}");
			}

			return differences;
		}

		protected override void AddMappingsToMappingTable(List<Row> leftRows, List<Row> rightRows, Dictionary<int, int> matchedRows)
		{
		}
	}
}
