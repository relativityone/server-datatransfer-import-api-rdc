using SQLDataComparer.Log;
using System.Collections.Generic;
using SQLDataComparer.Model;


namespace SQLDataComparer.DataCompare
{
	public abstract class RowMappingEqualityComparer : RowEqualityComparer
	{
		protected RowMappingEqualityComparer(ILog log, Dictionary<string, string> mappingTable, string tableName)
		 : base(log, mappingTable, tableName)
		{
		}

		protected List<string> GetMappingDifferences(string leftArtifactID, string rightArtifactID, string rowId)
		{
			var differences = new List<string>();

			if (!string.IsNullOrEmpty(leftArtifactID) && !string.IsNullOrEmpty(rightArtifactID))
			{
				if (!_mappingTable.ContainsKey(leftArtifactID))
				{
					// this means there are artifact ids missing from mapping table and we can't fully be sure whether those rows are equal or not
					_log.LogError($"{_tableName}.{rowId} : Missing Artifact ID in the mapping table: {leftArtifactID}");
				}
				else if (_mappingTable[leftArtifactID] != rightArtifactID)
				{
					differences.Add($"{_tableName}.{rowId} : Rows with have different mapping {_mappingTable[leftArtifactID]} != {rightArtifactID}");
				}
			}
			else if ((!string.IsNullOrEmpty(leftArtifactID) && string.IsNullOrEmpty(rightArtifactID))
						|| (string.IsNullOrEmpty(leftArtifactID) && !string.IsNullOrEmpty(rightArtifactID)))
			{
				differences.Add($"{_tableName}.{rowId} : Node mapped only on one side, left: {leftArtifactID}, right: {rightArtifactID}");
			}

			return differences;
		}

		protected override void AddMappingsToMappingTable(List<Row> leftRows, List<Row> rightRows, Dictionary<int, int> matchedRows)
		{
		}
	}
}
