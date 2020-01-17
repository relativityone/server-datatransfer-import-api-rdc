// <copyright file="SqlQueryHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System.Data;
	using System.Data.SqlClient;
	using System.Threading.Tasks;

	public class SqlQueryHelper
    {
	    private readonly string connectionString;

	    public SqlQueryHelper(IntegrationTestParameters parameters)
	    {
		    var builder = IntegrationTestHelper.GetSqlConnectionStringBuilder(parameters);
		    connectionString = builder.ConnectionString;
	    }

	    public async Task<DataTable> ExecuteQueryAsync(string queryString, params SqlParameter[] queryParameters)
	    {
		    var dataTable = new DataTable();

		    using (var connection = new SqlConnection(this.connectionString))
		    {
			    connection.Open();

			    using (var command = new SqlCommand(queryString, connection))
			    {
				    command.Parameters.AddRange(queryParameters);

				    using (SqlDataReader sqlDataReader = await command.ExecuteReaderAsync().ConfigureAwait(false))
				    {
					    dataTable.Load(sqlDataReader);
				    }
			    }
		    }

		    return dataTable;
	    }
    }
}
