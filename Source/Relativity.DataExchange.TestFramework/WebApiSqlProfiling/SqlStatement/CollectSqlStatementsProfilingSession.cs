// <copyright file="CollectSqlStatementsProfilingSession.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.SqlStatement
{
	public class CollectSqlStatementsProfilingSession : ProfilingSession
	{
		private const string Action =
			@"ACTION(sqlserver.client_app_name,sqlserver.database_id,sqlserver.query_hash,sqlserver.sql_text,sqlserver.request_id,sqlserver.session_id)";

		private const string Where =
			@"WHERE ([package0].[greater_than_uint64]([sqlserver].[database_id],(4)) AND [package0].[equal_boolean]([sqlserver].[is_system],(0)) AND [sqlserver].[like_i_sql_unicode_string]([sqlserver].[client_app_name],N'RelativityWebAPI%'))";

		public CollectSqlStatementsProfilingSession(string connectionString)
			: base(
				connectionString,
				profilerReportBuilderFactory: () => new CollectedSqlStatementsProfilerReportBuilder())
		{
		}

		protected override string[] GetEventsToCollect()
		{
			return new[]
				{
					$@"ADD EVENT sqlserver.rpc_completed({Action} {Where})",
					$@"ADD EVENT sqlserver.sql_batch_completed({Action} {Where})",
					$@"ADD EVENT sqlserver.sql_statement_completed({Action} {Where})",
					$@"ADD EVENT sqlserver.sp_statement_completed({Action} {Where})",
				};
		}
	}
}
