// <copyright file="CollectDeadlockProfilerReportSession.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.DeadlockReport
{
	public class CollectDeadlockProfilerReportSession : ProfilingSession
	{
		public CollectDeadlockProfilerReportSession(string connectionString)
			: base(
				connectionString,
				profilerReportBuilderFactory: () => new CollectedDeadlockProfilerReportBuilder())
		{
		}

		protected override string[] GetEventsToCollect()
		{
			return new[]
				{
					@"ADD EVENT sqlserver.xml_deadlock_report 
					(ACTION(sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.database_name, sqlserver.sql_text)
					WHERE ([package0].[greater_than_uint64]([sqlserver].[database_id],(4)) AND[package0].[equal_boolean]([sqlserver].[is_system],(0)) AND [duration]>(1000) AND[sqlserver].[client_app_name] = N'RelativityWebAPI'))",
				};
		}
	}
}
