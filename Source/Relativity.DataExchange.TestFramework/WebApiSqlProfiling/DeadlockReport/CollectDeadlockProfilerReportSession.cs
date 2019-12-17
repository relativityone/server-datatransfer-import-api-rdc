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
					(ACTION(sqlserver.client_app_name, sqlserver.client_hostname, sqlserver.database_name, sqlserver.sql_text))",
				};
		}
	}
}
