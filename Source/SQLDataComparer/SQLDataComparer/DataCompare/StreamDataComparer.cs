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

		private readonly Dictionary<string, Dictionary<string, string>> _mappingTable =
			new Dictionary<string, Dictionary<string, string>>();

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
					if (!String.IsNullOrEmpty(tableConfig.MapId))
					{
						if (!_mappingTable.ContainsKey(tableConfig.MapId))
						{
							_mappingTable[tableConfig.MapId] = new Dictionary<string, string>();
						}

						// step 2 - compare tables loading row by row with mapping
						Compare(tableConfig, new RowDataEqualityComparer(_log, tableConfig.MapId, _mappingTable[tableConfig.MapId], tableConfig.Name), tableConfig.Name);
					}
					else
					{
						// step 2 - compare tables loading row by row without mapping
						Compare(tableConfig, new SimpleRowDataEqualityComparer(_log, tableConfig.Name), tableConfig.Name);
					}
					
				}
			}

			foreach (var tableConfig in compareConfig.TablesConfig)
			{
				// step 3 - compare mappings loading row by row except Artifact mappings
				foreach (var mappingConfig in tableConfig.MappingsConfig)
				{
					string logTableName = tableConfig.Name + "->" + mappingConfig.Name;
					switch (mappingConfig.Type)
					{
						case MappingType.SingleObject:
						case MappingType.MultiObject:
							Compare(tableConfig, mappingConfig, new ObjectMappingEqualityComparer(_log, mappingConfig.TargetColumn, _mappingTable[mappingConfig.TargetColumn], logTableName), logTableName);
							break;
						case MappingType.SingleChoice:
						case MappingType.MultiChoice:
							Compare(tableConfig, mappingConfig, new ChoiceMappingEqualityComparer(_log, mappingConfig.TargetColumn, _mappingTable[mappingConfig.TargetColumn], logTableName), logTableName);
							break;
					}
				}
			}

			TableConfig artifactConfig = compareConfig.TablesConfig.FirstOrDefault(x => x.Name == "EDDSDBO.Artifact");

			if (artifactConfig != null)
			{
				Compare(artifactConfig, new ArtifactRowEqualityComparer(_log, _mappingTable["ArtifactID"], artifactConfig.Name), artifactConfig.Name);

				foreach (var artifactMappingConfig in artifactConfig.MappingsConfig)
				{
					if (artifactMappingConfig.Type == MappingType.Artifact)
					{
						Compare(artifactConfig, artifactMappingConfig, new SimpleRowDataEqualityComparer(_log, artifactConfig.Name), artifactConfig.Name);
					}
				}
			}

			TableConfig auditConfig = compareConfig.TablesConfig.FirstOrDefault(x => x.Name == "EDDSDBO.AuditRecord_PrimaryPartition");

			if (auditConfig != null)
			{
				CompareAudit(auditConfig, new AuditRowEqualityComparer(_log, _mappingTable["ArtifactID"], auditConfig.Name), auditConfig.Name);
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
			using (IEnumerator<Table> leftEnumerator = _dataLoader.GetMappingTable(tableConfig, mappingConfig, _mappingTable[mappingConfig.TargetColumn], SideEnum.Left).GetEnumerator())
			using (IEnumerator<Table> rightEnumerator = _dataLoader.GetMappingTable(tableConfig, mappingConfig, _mappingTable[mappingConfig.TargetColumn], SideEnum.Right).GetEnumerator())
			{
				CompareTables(leftEnumerator, rightEnumerator, comparer, tableName);
			}
		}

		private void CompareAudit(TableConfig auditConfig, AuditRowEqualityComparer auditComparer, string auditTableName)
		{
			using (IEnumerator<Table> leftEnumerator = _dataLoader.GetAuditTable(auditConfig, _mappingTable["ArtifactID"], SideEnum.Left).GetEnumerator())
			using (IEnumerator<Table> rightEnumerator = _dataLoader.GetAuditTable(auditConfig, _mappingTable["ArtifactID"], SideEnum.Right).GetEnumerator())
			{
				CompareTables(leftEnumerator, rightEnumerator, auditComparer, auditTableName);
			}
		}

		private void CompareTables(IEnumerator<Table> leftEnumerator, IEnumerator<Table> rightEnumerator, RowEqualityComparer comparer, string tableName)
		{
			bool leftCanAdvance = leftEnumerator.MoveNext();
			bool rightCanAdvance = rightEnumerator.MoveNext();
			bool lastRow = false;

			while (leftCanAdvance || rightCanAdvance || lastRow)
			{
				int result = CompareRows(leftEnumerator.Current, rightEnumerator.Current, comparer, tableName);

				// rows had the same row id, advance both
				if (result == 0)
				{
					// when advancing both check if both arrived at the last element
					// in thi case we have to compare one more time
					if (!lastRow)
					{
						leftCanAdvance = leftEnumerator.MoveNext();
						rightCanAdvance = rightEnumerator.MoveNext();

						lastRow = !leftCanAdvance && !rightCanAdvance;
					}
					// if we already compared last rows then we are done
					else
					{
						lastRow = false;
					}
				}
				else if (result < 0)
				{
					// right row id is higher
					// advance left so it can catchup to right
					if (leftCanAdvance)
					{
						leftCanAdvance = leftEnumerator.MoveNext();
					}
					//left is already at the end so now move only right
					else
					{
						rightCanAdvance = rightEnumerator.MoveNext();
					}

					if (lastRow)
					{
						lastRow = false;
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

					if (lastRow)
					{
						lastRow = false;
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

			int rowIdComparisonResult = string.Compare(leftRowId.TrimEnd(), rightRowId.TrimEnd(), StringComparison.CurrentCultureIgnoreCase);

			if (leftSubTable.MappedStrings.ContainsKey(leftSubTable.RowId) 
			    && rowIdComparisonResult != 0 
			    && leftRowId == leftSubTable.MappedStrings[leftSubTable.RowId].Key
			    && rightRowId == leftSubTable.MappedStrings[leftSubTable.RowId].Value)
			{
				rowIdComparisonResult = 0;
			}

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
