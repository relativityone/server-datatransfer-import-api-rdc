// ----------------------------------------------------------------------------
// <copyright file="SqlProfiling.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Xml.Linq;

	public class SqlProfiling
	{
		public static void StartProfilingForRelativityWebApi(string connectionString)
		{
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = @"
IF NOT EXISTS(SELECT 1 FROM sys.server_event_sessions WHERE [name] = 'RelativityWebAPI')
BEGIN
  CREATE EVENT SESSION [RelativityWebAPI] ON SERVER
  ADD EVENT sqlserver.error_reported(
      ACTION(sqlserver.client_app_name,sqlserver.database_id,sqlserver.query_hash,sqlserver.session_id,sqlserver.sql_text)
      WHERE ([package0].[greater_than_uint64]([sqlserver].[database_id],(4)) AND [package0].[equal_boolean]([sqlserver].[is_system],(0)) AND [sqlserver].[client_app_name]=N'RelativityWebAPI')),
  ADD EVENT sqlserver.rpc_completed(
      ACTION(sqlserver.client_app_name,sqlserver.database_id,sqlserver.query_hash,sqlserver.session_id,sqlserver.sql_text)
      WHERE ([package0].[greater_than_uint64]([sqlserver].[database_id],(4)) AND [package0].[equal_boolean]([sqlserver].[is_system],(0)) AND [sqlserver].[client_app_name]=N'RelativityWebAPI')),
  ADD EVENT sqlserver.sql_batch_completed(
      ACTION(sqlserver.client_app_name,sqlserver.database_id,sqlserver.query_hash,sqlserver.session_id,sqlserver.sql_text)
      WHERE ([package0].[greater_than_uint64]([sqlserver].[database_id],(4)) AND [package0].[equal_boolean]([sqlserver].[is_system],(0)) AND [sqlserver].[client_app_name]=N'RelativityWebAPI'))
  -- ADD TARGET package0.event_file(SET filename=N'C:\relativity\RelativityWebAPI.xel'),
  ADD TARGET package0.ring_buffer
  WITH (MAX_MEMORY=4096 KB,EVENT_RETENTION_MODE=ALLOW_SINGLE_EVENT_LOSS,MAX_DISPATCH_LATENCY=30 SECONDS,MAX_EVENT_SIZE=0 KB,MEMORY_PARTITION_MODE=NONE,TRACK_CAUSALITY=ON,STARTUP_STATE=OFF)
END
IF NOT EXISTS(SELECT * FROM sys.dm_xe_sessions WHERE [name] = 'RelativityWebAPI')
BEGIN
  ALTER EVENT SESSION RelativityWebAPI ON SERVER
  STATE = start;
END";
					command.CommandType = CommandType.Text;
					command.ExecuteNonQuery();
				}
			}
		}
		public static string ReadCapturedEventsAndStopProfiling(string connectionString)
		{
			string data = null;
			using (var connection = new SqlConnection(connectionString))
			{
				connection.Open();
				using (SqlCommand command = connection.CreateCommand())
				{
					command.CommandText = @"
SELECT xet.target_data
FROM sys.dm_xe_session_targets AS xet
JOIN sys.dm_xe_sessions AS xe
   ON (xe.address = xet.event_session_address)
WHERE xe.name = 'RelativityWebAPI';
DROP EVENT SESSION [RelativityWebAPI] ON SERVER;";
					command.CommandType = CommandType.Text;
					data = (string)command.ExecuteScalar();
				}
			}

			return data;
		}

		public static ProfilingReport AggregateReport(string data, bool printSql)
		{
			var profilingReport = new ProfilingReport(printSql);

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
				foreach (XElement element in eventElement.Elements())
				{
					profilingReport.Add(element);
				}
			}

			return profilingReport;
		}
	}
}
