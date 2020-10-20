using System;
using System.Data.SqlClient;
using System.Linq;
using SQLDataComparer.Config;
using SQLDataComparer.Log;

namespace SQLDataComparer.ConfigCheck
{
	public class ConfigChecker : IConfigChecker
	{
		private readonly string _leftConnectionString;
		private readonly string _rightConnectionString;
		private readonly ILog _log;

		public ConfigChecker(ILog log, string leftConnectionString, string rightConnectionString)
		{
			this._log = log;
			this._leftConnectionString = leftConnectionString;
			this._rightConnectionString = rightConnectionString;
		}

		public bool CheckTableConfig(TableConfig table)
		{
			string[] tableNameParts = table.Name.Split('.');

			if (tableNameParts.Length != 2)
				throw new Exception($"Invalid table name {table.Name}");

			string tableSchema = tableNameParts[0];
			string tableName = tableNameParts[1];

			return CheckTables(table, tableSchema, tableName)
			       && CheckColumnsConfig(table, tableSchema, tableName)
			       && CheckIgnoresConfig(table, tableSchema, tableName)
			       && CheckBoundsConfig(table, tableSchema, tableName);
		}

		private bool CheckColumnsConfig(TableConfig table, string tableSchema, string tableName)
		{
			return CheckColumn(tableSchema, tableName, table.RowId, SideEnum.Left)
			       && CheckColumn(tableSchema, tableName, table.RowId, SideEnum.Right);
		}

		private bool CheckIgnoresConfig(TableConfig table, string tableSchema, string tableName)
		{
			return table.IgnoreConfig.All(column =>
				CheckColumn(tableSchema, tableName, column.Name, SideEnum.Left) 
				&& CheckColumn(tableSchema, tableName, column.Name, SideEnum.Right));
		}

		private bool CheckBoundsConfig(TableConfig table, string tableSchema, string tableName)
		{
			return table.WhereConfig.All(column => 
				CheckColumn(tableSchema, tableName, column.Name, SideEnum.Left) 
				&& CheckColumn(tableSchema, tableName, column.Name, SideEnum.Right));
		}

		private bool CheckTables(TableConfig table, string tableSchema, string tableName)
		{
			return CheckTable(table, tableSchema, tableName, SideEnum.Left)
			       && CheckTable(table, tableSchema, tableName, SideEnum.Right);
		}

		private bool CheckTable(TableConfig table, string tableSchema, string tableName, SideEnum side)
		{
			if (table == null)
			{
				_log.LogError($"Table {tableSchema}.{tableName} is missing from the configuration");
			}

			bool tableExists;
			string connectionString = GetConnectionString(side);
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlParameter tableSchemaParameter = new SqlParameter("@TableSchema", tableSchema);
				SqlParameter tableNameParameter = new SqlParameter("@TableName", tableName);

				const string query = "IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @TableSchema AND TABLE_NAME = @TableName) SELECT 1 ELSE SELECT 0";

				using (var dbCommand = new SqlCommand(query, connection))
				{
					dbCommand.Parameters.Add(tableSchemaParameter);
					dbCommand.Parameters.Add(tableNameParameter);

					_log.LogQuery(dbCommand.CommandText, dbCommand.Parameters);
					tableExists = (int)dbCommand.ExecuteScalar() == 1;
				}
			}

			if (!tableExists)
			{
				_log.LogError($"Invalid table name: Table {tableName} does not exist in the {side.ToString()} database");
			}

			return tableExists;
		}

		private bool CheckColumn(string tableSchema, string tableName, string columnName, SideEnum side)
		{
			bool columnExists;
			string connectionString = GetConnectionString(side);
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				SqlParameter tableSchemaParameter = new SqlParameter("@TableSchema", tableSchema);
				SqlParameter tableNameParameter = new SqlParameter("@TableName", tableName);
				SqlParameter columnNameParameter = new SqlParameter("@ColumnName", columnName);

				const string query = "IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = @TableSchema AND TABLE_NAME = @TableName AND COLUMN_NAME = @ColumnName) SELECT 1 ELSE SELECT 0";

				using (var dbCommand = new SqlCommand(query, connection))
				{
					dbCommand.Parameters.Add(tableSchemaParameter);
					dbCommand.Parameters.Add(tableNameParameter);
					dbCommand.Parameters.Add(columnNameParameter);

					_log.LogQuery(dbCommand.CommandText, dbCommand.Parameters);
					columnExists = (int)dbCommand.ExecuteScalar() == 1;
				}
			}

			if (!columnExists)
			{
				_log.LogError($"Invalid rowId: Column {columnName} of Table {tableName} does not exist in the left database");
			}

			return columnExists;
		}

		private string GetConnectionString(SideEnum side)
		{
			switch (side)
			{
				case SideEnum.Left:
					return _leftConnectionString;
				case SideEnum.Right:
					return _rightConnectionString;
				default:
					throw new Exception($"Cannot retrieve connection string for {side}");
			}
		}
	}
}
