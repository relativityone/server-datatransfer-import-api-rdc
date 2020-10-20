using System.Collections.Generic;
using SQLDataComparer.Config;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public abstract class RowEqualityComparer
	{
		protected readonly ILog _log;
		protected readonly string _mapId;
		protected readonly Dictionary<string, string> _mappingTable;
		protected readonly string _tableName;

		protected RowEqualityComparer(ILog log, string mapId, Dictionary<string, string> mappingTable, string tableName)
		{
			this._log = log;
			this._mapId = mapId;
			this._mappingTable = mappingTable;
			this._tableName = tableName;
		}

		protected abstract List<string> GetDifferences(Row row1, Row row2);

		protected abstract void AddMappingsToMappingTable(IDictionary<Row, Row> matchedRows);

		public virtual List<string> MatchAndCompareRowsValues(List<Row> leftRows, List<Row> rightRows)
		{
			var differences = new List<string>();
			var matchedRows = new Dictionary<Row,Row>();

			var rowsToOuterIteration = rightRows.Count >= leftRows.Count ? leftRows : rightRows;
			var rowsToInnerIteration = rightRows.Count >= leftRows.Count ?  rightRows : leftRows;

			foreach (Row i in rowsToOuterIteration)
			{
				Row matchedTo = null;
				List<string> differencesOneRow = null;

				foreach (Row j in rowsToInnerIteration)
				{
					if (!matchedRows.ContainsKey(j))
					{
						List<string> differencesWithCurrentRow = GetDifferences(i, j);

						if (differencesOneRow == null || differencesWithCurrentRow.Count < differencesOneRow.Count)
						{
							matchedTo = j;
							differencesOneRow = differencesWithCurrentRow;
						}
					}
				}

				if (matchedTo != null)
				{
					matchedRows.Add(matchedTo, i);

					if (differencesOneRow?.Count > 0)
					{
						differences.AddRange(differencesOneRow);
					}
				}
			}

			AddMappingsToMappingTable(matchedRows);

			return differences;
		}

		protected class ComparableRows
		{
			public ComparableRows(Row row1, Row row2)
			{
				if (row1.Table.Side == SideEnum.Left)
				{
					this.LeftRow = row1;
					this.RightRow = row2;
				}
				else
				{
					this.RightRow = row1;
					this.LeftRow = row2;
				}
			}

			public Row LeftRow { get; }
			public Row RightRow { get; }
		}
	}
}
