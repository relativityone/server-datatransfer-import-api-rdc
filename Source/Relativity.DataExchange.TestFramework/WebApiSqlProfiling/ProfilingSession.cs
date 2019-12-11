// ----------------------------------------------------------------------------
// <copyright file="ProfilingSession.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Xml.Linq;

	public abstract class ProfilingSession
	{
		private readonly string connectionString;
		private readonly string profilingSessionName;

		private readonly Func<IProfilerReportBuilder> profilerReportBuilderFactory;

		protected ProfilingSession(string connectionString, Func<IProfilerReportBuilder> profilerReportBuilderFactory)
		{
			this.connectionString = connectionString;
			this.profilingSessionName = $"DataExchange_{Guid.NewGuid().ToString("N")}";
			this.profilerReportBuilderFactory = profilerReportBuilderFactory;
		}

		public void StartProfilingForRelativityWebApi()
		{
			using (var connection = new SqlConnection(this.connectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = this.BuildStartProfilingQuery();
					command.CommandType = CommandType.Text;
					command.ExecuteNonQuery();
				}
			}
		}

		public ProfilerReport ReadCapturedEventsAndStopProfiling()
		{
			string data = null;
			using (var connection = new SqlConnection(this.connectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = $@"
SELECT xet.target_data
FROM sys.dm_xe_session_targets AS xet
JOIN sys.dm_xe_sessions AS xe
   ON (xe.address = xet.event_session_address)
WHERE xe.name = '{this.profilingSessionName}';
DROP EVENT SESSION [{this.profilingSessionName}] ON SERVER;";
					command.CommandType = CommandType.Text;
					data = (string)command.ExecuteScalar();
				}
			}

			return this.BuildProfilingReport(data);
		}

		protected abstract string[] GetEventsToCollect();

		private ProfilerReport BuildProfilingReport(string data)
		{
			IProfilerReportBuilder profilingReportBuilder = this.profilerReportBuilderFactory();

			DateTime lastTimestamp = DateTime.MinValue;

			XElement xml = XElement.Parse(data);
			foreach (XElement eventElement in xml.Elements())
			{
				DateTime currentTimestamp = DateTime.Parse(eventElement.Attribute("timestamp").Value);
				if (currentTimestamp < lastTimestamp)
				{
					throw new OverflowException("The ring buffer wrapped around.");
				}

				lastTimestamp = currentTimestamp;

				profilingReportBuilder.CreateNewRow();
				foreach (XElement element in eventElement.Elements())
				{
					profilingReportBuilder.AddDetailsToCurrentRow(element);
				}

				profilingReportBuilder.CommitRow();
			}

			return profilingReportBuilder.Build();
		}

		private string BuildStartProfilingQuery()
		{
			return $@"
IF NOT EXISTS(SELECT 1 FROM sys.server_event_sessions WHERE [name] = '{this.profilingSessionName}')
BEGIN
  CREATE EVENT SESSION [{this.profilingSessionName}] ON SERVER
  {string.Join(",", this.GetEventsToCollect())}
  ADD TARGET package0.ring_buffer
  WITH (MAX_MEMORY=8192 KB,EVENT_RETENTION_MODE=ALLOW_SINGLE_EVENT_LOSS,MAX_DISPATCH_LATENCY=30 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=ON,STARTUP_STATE=OFF)
END
IF NOT EXISTS(SELECT * FROM sys.dm_xe_sessions WHERE [name] = '{this.profilingSessionName}')
BEGIN
  ALTER EVENT SESSION {this.profilingSessionName} ON SERVER
  STATE = start;
END";
		}
	}
}
