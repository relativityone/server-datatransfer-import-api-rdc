using System.Collections.Generic;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public abstract class RowEqualityComparer
	{
		protected readonly ILog _log;
		protected readonly Dictionary<string, string> _mappingTable;
		protected readonly string _tableName;

		protected RowEqualityComparer(ILog log, Dictionary<string, string> mappingTable, string tableName)
		{
			this._log = log;
			this._mappingTable = mappingTable;
			this._tableName = tableName;
		}

		protected abstract List<string> GetDifferences(Row leftRow, Row rightRow);

		protected abstract void AddMappingsToMappingTable(List<Row> leftRows, List<Row> rightRows, Dictionary<int, int> matchedRows);

		public virtual List<string> MatchAndCompareRowsValues(List<Row> leftRows, List<Row> rightRows)
		{
			var differences = new List<string>();
			var matchedRows = new Dictionary<int, int>();

			for (int i = 0; i < leftRows.Count; i++)
			{
				int matchedTo = -1;
				List<string> differencesOneRow = null;

				for (int j = 0; j < rightRows.Count; j++)
				{
					if (!matchedRows.ContainsKey(j))
					{
						List<string> differencesWithCurrentRow = GetDifferences(leftRows[i], rightRows[j]);

						if (differencesOneRow == null || differencesWithCurrentRow.Count < differencesOneRow.Count)
						{
							matchedTo = j;
							differencesOneRow = differencesWithCurrentRow;
						}
					}
				}

				if (matchedTo != -1)
				{
					matchedRows.Add(matchedTo, i);

					if (differencesOneRow?.Count > 0)
					{
						differences.AddRange(differencesOneRow);
					}
				}
			}

			AddMappingsToMappingTable(leftRows, rightRows, matchedRows);

			return differences;
		}
	}
}
