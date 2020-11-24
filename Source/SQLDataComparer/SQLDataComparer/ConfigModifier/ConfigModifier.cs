using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using SQLDataComparer.Config;
using SQLDataComparer.Log;

namespace SQLDataComparer.ConfigModifier
{
	public class ConfigModifier
	{
		private readonly string _leftConnectionString;
		private readonly ILog _log;
		private List<string> _tableNames;

		public ConfigModifier(ILog log, string leftConnectionString)
		{
			this._log = log;
			this._leftConnectionString = leftConnectionString;
			this._tableNames = new List<string>();
		}

		/// <summary>
		///  Multi-objects and File tables are appended to the config here since their names are created dynamically.
		/// </summary>
		public void AppendAdditionalTables(ref CompareConfig compareConfig)
		{
			if (!GetAllTables())
			{
				return;
			}

			PopulateWithFileTables(ref compareConfig);
			PopulateWithMultiObjectTables(ref compareConfig);
		}

		private void PopulateWithFileTables(ref CompareConfig compareConfig)
		{
			var fileTables = _tableNames.FindAll(CheckIfFileTable);

			AddToConfig(fileTables, ref compareConfig, GetTableConfigForMultiObject);
		}

		private void PopulateWithMultiObjectTables(ref CompareConfig compareConfig)
		{
			var multiObjectTables = _tableNames.FindAll(CheckIfMultiObjectTable);

			AddToConfig(multiObjectTables, ref compareConfig, GetTableConfigForFiles);
		}

		private static void AddToConfig(List<string> tableNames, ref CompareConfig compareConfig, Func<string, TableConfig> getTable)
		{
			TableConfig[] modifiedTable = compareConfig.TablesConfig;

			Array.Resize(ref modifiedTable, modifiedTable.Length + tableNames.Count);

			int tableIndex = compareConfig.TablesConfig.Length;

			foreach (var name in tableNames)
			{
				modifiedTable[tableIndex++] = getTable(name);
			}

			compareConfig.TablesConfig = modifiedTable;
		}

		static TableConfig GetTableConfigForMultiObject(string name)
		{
			return new TableConfig()
			{
				Name = "EDDSDBO." + name,
				RowId = name.Substring(0, name.Length/2) + "ArtifactID"
			};
		}

		private static TableConfig GetTableConfigForFiles(string name)
		{
			return new TableConfig()
			{
				Name = "EDDSDBO." + name,
				RowId = "Filename",
				IgnoreConfig = new IgnoreConfig[] {
					new IgnoreConfig
					{
						Name = "Location"
					},

					new IgnoreConfig
					{
						Name = "FileID"
					}
				}
			};
		}

		private bool GetAllTables()
		{
			_tableNames.Clear();

			using (var connection = new SqlConnection(_leftConnectionString))
			{
				connection.Open();

				DataTable dataTable = connection.GetSchema("Tables");

				foreach (DataRow row in dataTable.Rows)
				{
					string tableName = (string)row[2];
					_tableNames.Add(tableName);
				}

				connection.Close();
			}

			if (!_tableNames.Any())
			{
				_log.LogError($"No tables were found in the database.");
				return false;
			}

			return true;
		}

		private static bool CheckIfFileTable(string tableName)
		{
			// File table name pattern is "File###", the number of digits may vary.
			const string beginningOfTheTableName = "File";

			if (tableName.StartsWith(beginningOfTheTableName))
			{
				return tableName.Length > beginningOfTheTableName.Length &&
				       tableName.Substring(beginningOfTheTableName.Length + 1).All(char.IsDigit);
			}

			return false;
		}

		private static bool CheckIfMultiObjectTable(string tableName)
		{
			// Multi object table name pattern is "f###f###" the number of digits may vary.
			const char delimiter = 'f';

			if (tableName[0] == delimiter)
			{
				return tableName[tableName.Length / 2] == delimiter &&
				       tableName.Substring(1, tableName.Length / 2 - 1).All(char.IsDigit) &&
				       tableName.Substring(tableName.Length / 2 + 1).All(char.IsDigit);
			}

			return false;
		}
	}
}