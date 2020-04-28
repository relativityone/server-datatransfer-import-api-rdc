using System;
using System.Collections.Generic;
using System.Linq;
using SQLDataComparer.Config;
using SQLDataComparer.ConfigCheck;
using SQLDataComparer.DataLoad;
using SQLDataComparer.Log;
using SQLDataComparer.Model;

namespace SQLDataComparer.DataCompare
{
	public class StreamDataComparer
	{
		private readonly ILog _log;
		private readonly IConfigChecker _configChecker;
		private readonly IDataLoader _dataLoader;

		private readonly Dictionary<string, string> _mappingTable = new Dictionary<string, string>();

		private readonly List<ComparisonResult> _results = new List<ComparisonResult>();

		public StreamDataComparer(ILog log, IConfigChecker configChecker, IDataLoader dataLoader)
		{
			this._log = log;
			this._configChecker = configChecker;
			this._dataLoader = dataLoader;
		}

		public IEnumerable<ComparisonResult> CompareData(CompareConfig compareConfig)
		{
			if (!compareConfig.TablesConfig.Any())
				throw new Exception("No tables in the configuration");

			_results.Clear();

			_mappingTable.Clear();

			foreach (var tableConfig in compareConfig.TablesConfig.Where(x=>x.Name != "EDDSDBO.Artifact" && x.Name != "EDDSDBO.AuditRecord_PrimaryPartition"))
			{
				// step 1 - check configuration - parameters have to be filled to check this
				if (_configChecker.CheckTableConfig(tableConfig))
				{
					// step 2 - compare tables loading row by row
					Compare(tableConfig, new RowDataEqualityComparer(_log, _mappingTable, tableConfig.Name), tableConfig.Name);
				}
			}

			foreach (var tableConfig in compareConfig.TablesConfig)
			{
				// step 3 - compare mappings loading row by row
				foreach (var mappingConfig in tableConfig.MappingsConfig)
				{
					string logTableName = tableConfig.Name + "->" + mappingConfig.Name;
					switch (mappingConfig.Type)
					{
						case MappingType.SingleObject:
						case MappingType.MultiObject:
							Compare(tableConfig, mappingConfig, new ObjectMappingEqualityComparer(_log, _mappingTable, logTableName), logTableName);
							break;
						case MappingType.SingleChoice:
						case MappingType.MultiChoice:
							Compare(tableConfig, mappingConfig, new ChoiceMappingEqualityComparer(_log, _mappingTable, logTableName), logTableName);
							break;
					}
				}
			}

			TableConfig artifactConfig = compareConfig.TablesConfig.FirstOrDefault(x => x.Name == "EDDSDBO.Artifact");

			if (artifactConfig != null)
			{
				Compare(artifactConfig, new ArtifactRowEqualityComparer(_log, _mappingTable, artifactConfig.Name), artifactConfig.Name);
			}

			TableConfig auditConfig = compareConfig.TablesConfig.FirstOrDefault(x => x.Name == "EDDSDBO.AuditRecord_PrimaryPartition");

			if (auditConfig != null)
			{
				CompareAudit(auditConfig, new AuditRowEqualityComparer(_log, _mappingTable, auditConfig.Name), auditConfig.Name);
			}

			return _results;
		}

		private void Compare(TableConfig tableConfig, RowEqualityComparer comparer, string tableName)
		{
			using (IEnumerator<Table> leftEnumerator = _dataLoader.GetDataTable(tableConfig, SideEnum.Left).GetEnumerator())
			using (IEnumerator<Table> rightEnumerator = _dataLoader.GetDataTable(tableConfig, SideEnum.Right).GetEnumerator())
			{
				CompareTables(leftEnumerator, rightEnumerator, comparer, tableName);
			}
		}

		private void Compare(TableConfig tableConfig, MappingConfig mappingConfig, RowEqualityComparer comparer, string tableName)
		{
			using (IEnumerator<Table> leftEnumerator = _dataLoader.GetMappingTable(tableConfig, mappingConfig, _mappingTable, SideEnum.Left).GetEnumerator())
			using (IEnumerator<Table> rightEnumerator = _dataLoader.GetMappingTable(tableConfig, mappingConfig, _mappingTable, SideEnum.Right).GetEnumerator())
			{
				CompareTables(leftEnumerator, rightEnumerator, comparer, tableName);
			}
		}

		private void CompareAudit(TableConfig auditConfig, AuditRowEqualityComparer auditComparer, string auditTableName)
		{
			using (IEnumerator<Table> leftEnumerator = _dataLoader.GetAuditTable(auditConfig, _mappingTable, SideEnum.Left).GetEnumerator())
			using (IEnumerator<Table> rightEnumerator = _dataLoader.GetAuditTable(auditConfig, _mappingTable, SideEnum.Right).GetEnumerator())
			{
				CompareTables(leftEnumerator, rightEnumerator, auditComparer, auditTableName);
			}
		}

		private void CompareTables(IEnumerator<Table> leftEnumerator, IEnumerator<Table> rightEnumerator, RowEqualityComparer comparer, string tableName)
		{
			bool leftCanAdvance = leftEnumerator.MoveNext();
			bool rightCanAdvance = rightEnumerator.MoveNext();

			while (leftCanAdvance || rightCanAdvance)
			{
				int result = CompareRows(leftEnumerator.Current, rightEnumerator.Current, comparer, tableName);

				// rows had the same row id, advance both
				if (result == 0)
				{
					leftCanAdvance = leftEnumerator.MoveNext();
					rightCanAdvance = rightEnumerator.MoveNext();
				}
				else if (result < 0)
				{
					//right row id is higher
					//advance left so it can catchup to right
					if (leftCanAdvance)
					{
						leftCanAdvance = leftEnumerator.MoveNext();
					}
					//left is already at the end so now move only right
					else
					{
						rightCanAdvance = rightEnumerator.MoveNext();
					}
				}
				else
				{
					//left row id is higher
					//advance right so it can catchup to left
					if (rightCanAdvance)
					{
						rightCanAdvance = rightEnumerator.MoveNext();
					}
					//right is already at the end so now move only left
					else
					{
						leftCanAdvance = leftEnumerator.MoveNext();
					}
				}
			}
		}

		// Check whether rows should be compared, decide what enumerator should advance
		// if 0  - advance both
		// if -1 - advance left
		// if 1  - advance right
		private int CompareRows(Table leftSubTable, Table rightSubTable, RowEqualityComparer comparer, string tableName)
		{
			if ((leftSubTable == null || leftSubTable.Rows.Count == 0)
				&& (rightSubTable == null || rightSubTable.Rows.Count == 0))
			{
				return 0;
			}

			if (leftSubTable == null || leftSubTable.Rows.Count == 0 && rightSubTable.Rows.Count != 0)
			{
				_results.Add(new ComparisonResult
				{
					Result = ComparisonResultEnum.RightOnly,
					RowIdentifier = rightSubTable.Rows[0][rightSubTable.RowId],
					TableName = tableName
				});
				return 1;
			}

			if (rightSubTable == null || rightSubTable.Rows.Count == 0 && leftSubTable.Rows.Count != 0)
			{
				_results.Add(new ComparisonResult
				{
					Result = ComparisonResultEnum.LeftOnly,
					RowIdentifier = leftSubTable.Rows[0][leftSubTable.RowId],
					TableName = tableName
				});
				return -1;
			}

			string leftRowId = leftSubTable.Rows[0][leftSubTable.RowId];
			string rightRowId = rightSubTable.Rows[0][leftSubTable.RowId];

			int rowIdComparisonResult = String.Compare(leftRowId, rightRowId, StringComparison.CurrentCultureIgnoreCase);

			if (rowIdComparisonResult == 0)
			{
				List<string> differences = CompareRowsWithSameId(leftSubTable.Rows, rightSubTable.Rows, tableName, leftSubTable.RowId, comparer);

				if (differences.Count == 0)
				{
					_results.Add(new ComparisonResult
					{
						Result = ComparisonResultEnum.Identical,
						RowIdentifier = leftSubTable.Rows[0][leftSubTable.RowId],
						TableName = tableName
					});
				}
				else
				{
					_results.Add(new ComparisonResult
					{
						Result = ComparisonResultEnum.Different,
						RowIdentifier = leftSubTable.Rows[0][leftSubTable.RowId],
						TableName = tableName,
						DifferenceReasons = differences
					});
				}
			}
			else if (rowIdComparisonResult < 0)
			{
				_results.Add(new ComparisonResult
				{
					Result = ComparisonResultEnum.LeftOnly,
					RowIdentifier = leftSubTable.Rows[0][leftSubTable.RowId],
					TableName = tableName
				});
			}
			else
			{
				_results.Add(new ComparisonResult
				{
					Result = ComparisonResultEnum.RightOnly,
					RowIdentifier = rightSubTable.Rows[0][leftSubTable.RowId],
					TableName = tableName
				});
			}

			return rowIdComparisonResult;
		}

		// This is called for rows with the same row id - only those rows should be compared
		private List<string> CompareRowsWithSameId(List<Row> leftRows, List<Row> rightRows, string tableName, string rowId, RowEqualityComparer comparer)
		{
			var differences = new List<string>();

			string leftRowId = leftRows.FirstOrDefault().Id;

			int leftRowsCount = leftRows.Count;
			int rightRowsCount = rightRows.Count;

			if (leftRowsCount != rightRowsCount)
			{
				differences.Add($"{tableName} : Number of rows with {rowId} = {leftRowId}, left: {leftRowsCount}, right: {rightRowsCount}");
			}
			
			differences.AddRange(comparer.MatchAndCompareRowsValues(leftRows, rightRows));

			return differences;
		}
	}
}
