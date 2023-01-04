// <copyright file="CollectExecutionPlansProfilingSession.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.ExecutionPlan
{
	public class CollectExecutionPlansProfilingSession : ProfilingSession
	{
		public CollectExecutionPlansProfilingSession(string connectionString)
			: base(
				connectionString,
				profilerReportBuilderFactory: () => new CollectedExecutionPlansProfilerReportBuilder())
		{
		}

		protected override string[] GetEventsToCollect()
		{
			return new[]
				{
					@"ADD EVENT sqlserver.query_post_execution_showplan(
ACTION(sqlserver.client_app_name,sqlserver.database_id,sqlserver.query_hash,sqlserver.session_id,sqlserver.sql_text)
WHERE ([package0].[greater_than_uint64]([sqlserver].[database_id],(4)) AND[package0].[equal_boolean]([sqlserver].[is_system],(0)) AND [duration]>(1000) AND [sqlserver].[like_i_sql_unicode_string]([sqlserver].[client_app_name],N'RelativityWebAPI%')))",
				};
		}
	}
}
