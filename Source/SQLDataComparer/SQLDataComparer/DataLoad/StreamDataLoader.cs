using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using SQLDataComparer.Config;
using SQLDataComparer.Log;
using SQLDataComparer.Model;
using MappingType = SQLDataComparer.Config.MappingType;

namespace SQLDataComparer.DataLoad
{
	public class StreamDataLoader : IDataLoader
	{
		private readonly ILog _log;
		private readonly string _leftConnectionString;
		private readonly string _rightConnectionString;

		public StreamDataLoader(ILog log, string leftConnectionString, string rightConnectionString)
		{
			this._log = log;
			this._leftConnectionString = leftConnectionString;
			this._rightConnectionString = rightConnectionString;
		}

		public IEnumerable<Table> GetDataTable(TableConfig tableConfig, SideEnum side)
		{
			string query = GetQuery(tableConfig);
			string connectionString = GetConnectionString(side);

			return GetTable(tableConfig, connectionString, query, side);
		}

		public IEnumerable<Table> GetMappingTable(TableConfig tableConfig, MappingConfig mappingConfig, Dictionary<string,string> mappingTable, SideEnum side)
		{
			switch (mappingConfig.Type)
			{
				case MappingType.SingleObject:
					return GetSingleObjectTable(tableConfig, mappingConfig, mappingTable, side);
				case MappingType.SingleChoice:
				case MappingType.MultiChoice:
					return GetChoiceTable(tableConfig, mappingConfig, mappingTable, side);
				case MappingType.MultiObject:
					return GetMultiObjectTable(tableConfig, mappingConfig, mappingTable, side);
				default:
					return null;
			}
		}

		public IEnumerable<Table> GetAuditTable(TableConfig auditConfig, Dictionary<string, string> mappingTable, SideEnum side)
		{
			string query = GetAuditQuery(auditConfig, mappingTable, side);
			string connectionString = GetConnectionString(side);

			return GetTable(auditConfig, connectionString, query, side);
		}

		private IEnumerable<Table> GetSingleObjectTable(TableConfig tableConfig, MappingConfig mappingConfig, Dictionary<string, string> mappingTable, SideEnum side)
		{
			string query = GetSingleObjectQuery(tableConfig, mappingConfig, mappingTable, side);
			string connectionString = GetConnectionString(side);

			return GetTable(tableConfig, connectionString, query, side);
		}

		private IEnumerable<Table> GetChoiceTable(TableConfig tableConfig, MappingConfig mappingConfig, Dictionary<string,string> mappingTable, SideEnum side)
		{
			string connectionString = GetConnectionString(side);
			
			string query = GetChoiceQuery(tableConfig, mappingConfig, mappingTable, side);

			return GetTable(tableConfig, connectionString, query, side);
		}

		private IEnumerable<Table> GetMultiObjectTable(TableConfig tableConfig, MappingConfig mappingConfig, Dictionary<string,string> mappingTable, SideEnum side)
		{
			string connectionString = GetConnectionString(side);

			string query = GetMultiObjectQuery(tableConfig, mappingConfig, mappingTable, side);

			return GetTable(tableConfig, connectionString, query, side);
		}

		private IEnumerable<Table> GetTable(TableConfig tableConfig, string connectionString, string query, SideEnum side)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();

				var command = new SqlCommand(query, connection);
				_log.LogQuery(command.CommandText, command.Parameters);

				SqlDataReader reader = command.ExecuteReader();

				Table table = CreateTable(reader, tableConfig, side);
				table.Name = tableConfig.Name;
				table.RowId = tableConfig.RowId;
				table.Ignores = new HashSet<string>(tableConfig.IgnoreConfig.Select(x => x.Name)); 

				if (reader.HasRows)
				{
					reader.Read();
					IDataRecord firstRecord = reader;
					Row row = CreateRow(firstRecord, table, tableConfig, side);
					table.Rows.Add(row);

					string currentRowId = row.Id;

					foreach (IDataRecord record in reader)
					{
						// case 1 row with the same row id
						if (String.Compare(currentRowId, record[tableConfig.RowId].ToString(), StringComparison.CurrentCultureIgnoreCase) == 0)
						{
							row = CreateRow(record, table, tableConfig, side);
							table.Rows.Add(row);
						}
						// case 2 row with different row id
						else
						{
							yield return table;
							table.Rows.Clear();
							row = CreateRow(record, table, tableConfig, side);
							table.Rows.Add(row);
							currentRowId = row.Id;
						}
					}
				}
			}
		}

		private Table CreateTable(IDataReader reader, TableConfig tableConfig, SideEnum side)
		{
			var table = new Table();

			DataTable schemaTable = reader.GetSchemaTable();

			for (int i = 0; i < schemaTable.Rows.Count; i++)
			{
				string colName = schemaTable.Rows[i].Field<string>("ColumnName");
				table.Columns.Add(colName, i);
			}

			return table;
		}

		private Row CreateRow(IDataRecord record, Table table, TableConfig tableConfig, SideEnum side)
		{
			var row = new Row(table);

			foreach (var column in table.Columns)
			{
				row.Values.Add(record[column.Key].ToString());
			}

			row.Id = row[table.RowId];

			return row;
		}

		private string GetQuery(TableConfig tableConfig)
		{
			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append($"SELECT * FROM {tableConfig.Name}");

			if (tableConfig.WhereConfig != null && tableConfig.WhereConfig.Length > 0)
			{
				queryBuilder.Append(GetQuery(tableConfig.WhereConfig));
			}

			queryBuilder.Append($" ORDER BY {tableConfig.RowId} ASC");

			return queryBuilder.ToString();
		}

		private string GetQuery(WhereConfig[] whereConfig)
		{
			StringBuilder queryBuilder = new StringBuilder();

			bool first = true;
			foreach (var singleWhereConfig in whereConfig)
			{
				if (first)
				{
					queryBuilder.Append($" WHERE {singleWhereConfig.Name}");
					first = false;
				}
				else
				{
					queryBuilder.Append($" AND {singleWhereConfig.Name}");
				}

				if (singleWhereConfig.Value.Contains(','))
				{
					if (singleWhereConfig.Type == WhereType.IsNot)
						queryBuilder.Append(" NOT");

					queryBuilder.Append($" IN({singleWhereConfig.Value})");
				}
				else
				{
					if (singleWhereConfig.Type == WhereType.IsNot)
						queryBuilder.Append(" !");

					queryBuilder.Append($"={singleWhereConfig.Value}");
				}
			}

			return queryBuilder.ToString();
		}

		private string GetSingleObjectQuery(TableConfig tableConfig, MappingConfig mappingConfig, Dictionary<string,string> mappingTable, SideEnum side)
		{
			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append($"SELECT {tableConfig.RowId}, ArtifactID, {mappingConfig.Name}");
			queryBuilder.Append($" FROM {tableConfig.Name}");
			queryBuilder.Append($" ORDER BY {tableConfig.RowId}");

			return queryBuilder.ToString();
		}

		private string GetCodeTypeIDQuery(MappingConfig mappingConfig)
		{
			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append("SELECT CodeTypeID");
			queryBuilder.Append(" FROM EDDSDBO.Field");
			queryBuilder.Append($" WHERE DisplayName = '{mappingConfig.Name}'");

			return queryBuilder.ToString();
		}

		private string GetArtifactIDQuery(MappingConfig mappingConfig)
		{
			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append("SELECT ArtifactID");
			queryBuilder.Append(" FROM EDDSDBO.Field");
			queryBuilder.Append($" WHERE DisplayName = '{mappingConfig.Name}'");
			queryBuilder.Append(" AND FieldArtifactTypeID = 10");

			return queryBuilder.ToString();
		}

		private string GetAssociativeArtifactIDQuery(MappingConfig mappingConfig)
		{
			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append("SELECT ArtifactID");
			queryBuilder.Append(" FROM EDDSDBO.Field");
			queryBuilder.Append($" WHERE DisplayName = '{mappingConfig.Name}'");
			queryBuilder.Append(" AND AssociativeArtifactTypeID = 10");

			return queryBuilder.ToString();
		}

		private string GetChoiceQuery(TableConfig tableConfig, MappingConfig mapping, Dictionary<string, string> mappingTable, SideEnum side)
		{
			string connectionString = GetConnectionString(side);
			string query = GetCodeTypeIDQuery(mapping);
			string codeTypeID;

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();

				var command = new SqlCommand(query, connection);
				_log.LogQuery(command.CommandText, command.Parameters);

				codeTypeID = command.ExecuteScalar().ToString();
			}

			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append($"SELECT {tableConfig.Name}.{tableConfig.RowId}, CodeArtifactID, AssociatedArtifactID");
			queryBuilder.Append($" FROM {tableConfig.Name}");
			queryBuilder.Append($" JOIN EDDSDBO.ZCodeArtifact_{codeTypeID}");
			queryBuilder.Append($" ON {tableConfig.Name}.ArtifactID");
			queryBuilder.Append(" = AssociatedArtifactID");
			queryBuilder.Append($" ORDER BY {tableConfig.RowId}");

			return queryBuilder.ToString();
		}

		private string GetMultiObjectQuery(TableConfig tableConfig, MappingConfig mappingConfig, Dictionary<string,string> mappingTable, SideEnum side)
		{
			string connectionString = GetConnectionString(side);
			string query = GetArtifactIDQuery(mappingConfig);

			string docArtifactID;

			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();

				var command = new SqlCommand(query, connection);
				_log.LogQuery(command.CommandText, command.Parameters);

				docArtifactID = command.ExecuteScalar().ToString();
			}

			string objArtifactID;

			query = GetAssociativeArtifactIDQuery(mappingConfig);
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();

				var command = new SqlCommand(query, connection);
				_log.LogQuery(command.CommandText, command.Parameters);

				objArtifactID = command.ExecuteScalar().ToString();
			}

			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append($"SELECT {tableConfig.Name}.{tableConfig.RowId}, f{docArtifactID}ArtifactID, f{objArtifactID}ArtifactID");
			queryBuilder.Append($" FROM {tableConfig.Name}");
			queryBuilder.Append($" JOIN EDDSDBO.f{docArtifactID}f{objArtifactID}");
			queryBuilder.Append($" ON {tableConfig.Name}.ArtifactID");
			queryBuilder.Append($" = f{objArtifactID}ArtifactID");
			queryBuilder.Append($" ORDER BY {tableConfig.RowId}");

			return queryBuilder.ToString();
		}

		private string GetAuditQuery(TableConfig auditConfig, Dictionary<string, string> mappingTable, SideEnum side)
		{
			StringBuilder queryBuilder = new StringBuilder();

			queryBuilder.Append("SELECT artifact.TextIdentifier, artifact.ArtifactID, audit.Details");
			queryBuilder.Append($" FROM {auditConfig.Name} as audit");
			queryBuilder.Append(" JOIN EDDSDBO.Artifact as artifact");
			queryBuilder.Append(" ON artifact.ArtifactID = audit.ArtifactID");
			queryBuilder.Append(" WHERE audit.Action = 47");
			queryBuilder.Append($" ORDER BY artifact.TextIdentifier, audit.ID");

			return queryBuilder.ToString();
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
