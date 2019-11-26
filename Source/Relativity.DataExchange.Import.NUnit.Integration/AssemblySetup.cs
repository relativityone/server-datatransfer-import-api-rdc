// ----------------------------------------------------------------------------
// <copyright file="AssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.Text;
	using System.Xml.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.TestFramework;

	[SetUpFixture]
	public class AssemblySetup
	{
		public static IntegrationTestParameters TestParameters
		{
			get;
			private set;
		}

		[OneTimeSetUp]
		public void Setup()
		{
			TestParameters = IntegrationTestHelper.Create();

			CaptureEvents(IntegrationTestHelper.GetConnectionString(TestParameters));
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			IntegrationTestHelper.Destroy(TestParameters);

			string data = GetEventsData(IntegrationTestHelper.GetConnectionString(TestParameters));
			CalculateReport(data, false);
		}

		private static void CaptureEvents(SqlConnectionStringBuilder sqlConnectionStringBuilder)
		{
			using (SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
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

		private static string GetEventsData(SqlConnectionStringBuilder sqlConnectionStringBuilder)
		{
			string data = null;
			using (SqlConnection connection = new SqlConnection(sqlConnectionStringBuilder.ConnectionString))
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

		private static void CalculateReport(string data, bool printSql)
		{
			ulong cpuTime = 0;
			ulong duration = 0;
			ulong physicalReads = 0;
			ulong logicalReads = 0;
			ulong writes = 0;
			ulong rowCount = 0;

			var sql = new StringBuilder();

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
					switch (element.Name.LocalName)
					{
						case "action":
						case "data":
							switch (element.Attribute("name").Value)
							{
								case "cpu_time":
									cpuTime += ulong.Parse(element.Element("value").Value);
									break;
								case "duration":
									duration += ulong.Parse(element.Element("value").Value);
									break;
								case "physical_reads":
									physicalReads += ulong.Parse(element.Element("value").Value);
									break;
								case "logical_reads":
									logicalReads += ulong.Parse(element.Element("value").Value);
									break;
								case "writes":
									writes += ulong.Parse(element.Element("value").Value);
									break;
								case "row_count":
									rowCount += ulong.Parse(element.Element("value").Value);
									break;
								case "batch_text":
								case "statement":
									if (printSql)
									{
										sql.AppendLine(element.Element("value").Value);
										sql.AppendLine();
										sql.AppendLine("----------------------------------------------------------");
										sql.AppendLine();
									}

									break;
							}

							break;
						default:
							throw new ArgumentException(nameof(element));
					}
				}
			}

			if (printSql)
			{
				Console.WriteLine(sql);
			}

			Console.WriteLine($"{nameof(cpuTime)}: {cpuTime}");
			Console.WriteLine($"{nameof(duration)}: {duration}");
			Console.WriteLine($"{nameof(physicalReads)}: {physicalReads}");
			Console.WriteLine($"{nameof(logicalReads)}: {logicalReads}");
			Console.WriteLine($"{nameof(writes)}: {writes}");
			Console.WriteLine($"{nameof(rowCount)}: {rowCount}");
		}
	}
}