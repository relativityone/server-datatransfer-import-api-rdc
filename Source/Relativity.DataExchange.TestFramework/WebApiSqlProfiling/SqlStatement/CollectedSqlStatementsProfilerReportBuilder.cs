// ----------------------------------------------------------------------------
// <copyright file="CollectedSqlStatementsProfilerReportBuilder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------;

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.SqlStatement
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;

	internal class CollectedSqlStatementsProfilerReportBuilder : ProfilerReportBuilderBase<SqlStatementRowDto>
	{
		public override ProfilerReport Build()
		{
			IList<SqlStatementRowDto> statementsByDuration = this.GetStatementsSortedByDuration();

			string description = this.BuildDescription();
			IEnumerable<TextFileDto> files = BuildFiles(description, statementsByDuration);
			return new ProfilerReport(description, files);
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "It is a simple switch statement.")]
		protected override void AddDetailsToRow(XElement element, SqlStatementRowDto row)
		{
			element = element ?? throw new ArgumentNullException(nameof(element));
			row = row ?? throw new ArgumentNullException(nameof(row));

			switch (element.Attribute("name").Value)
			{
				case "cpu_time":
					row.CpuTime = this.GetNumericValue(element);
					break;
				case "duration":
					row.Duration = this.GetNumericValue(element);
					break;
				case "physical_reads":
					row.PhysicalReads = this.GetNumericValue(element);
					break;
				case "logical_reads":
					row.LogicalReads = this.GetNumericValue(element);
					break;
				case "writes":
					row.Writes = this.GetNumericValue(element);
					break;
				case "row_count":
					row.RowCount = this.GetNumericValue(element);
					break;
				case "query_hash":
					row.QueryHash = this.GetNumericValue(element);
					break;
				case "session_id":
					row.SessionID = this.GetNumericValue(element);
					break;
				case "request_id":
					row.RequestID = this.GetNumericValue(element);
					break;
				case "batch_text":
					row.BatchText = element.Element("value").Value;
					break;
				case "sql_text":
					row.Sql = element.Element("value").Value;
					break;
				case "statement":
					row.Statement = element.Element("value").Value;
					break;
			}
		}

		private static IEnumerable<TextFileDto> BuildFiles(string description, IList<SqlStatementRowDto> sortedStatements)
		{
			const string fileName = "SQLs";
			const string extension = "txt";

			var sb = new StringBuilder();
			sb.AppendLine(description);
			sb.AppendLine("All statements:");
			foreach (SqlStatementRowDto row in sortedStatements)
			{
				sb.AppendLine($"--- Query Hash: {row.QueryHash}; Duration: {row.Duration}; CpuTime: {row.CpuTime};");
				sb.AppendLine($"--- Logical Reads: {row.LogicalReads}; Physical Reads: {row.PhysicalReads}; Writes: {row.Writes};");
				sb.AppendLine($"--- Row Count: {row.RowCount}");
				sb.AppendLine($"--- Session: {row.SessionID}; Request: {row.RequestID}");
				sb.AppendLine("Sql Text:");
				sb.AppendLine(row.Sql);
				sb.AppendLine("Batch Text");
				string batchText = row.BatchText == row.Sql ? "Same as Sql text" : row.BatchText;
				sb.AppendLine(batchText);
				sb.AppendLine("Statement");
				string statementText = row.Statement == row.Sql ? "Same as Sql text" :
				                       row.Statement == row.BatchText ? "Same as Batch text" : row.Statement;
				sb.AppendLine(statementText);
				sb.AppendLine();
			}

			var content = sb.ToString();
			yield return new TextFileDto(fileName, extension, content);
		}

		private string BuildDescription()
		{
			const string ReportHeader = "SQL Profiling - Collect SQL";

			var sb = new StringBuilder();
			sb.AppendLine($"{ReportHeader} - Report:");
			this.WriteAggregatedStatistics(sb);
			sb.AppendLine();
			this.WriteQueryHashes(sb);
			sb.AppendLine($"{ReportHeader} - End of the report");
			return sb.ToString();
		}

		private void WriteQueryHashes(StringBuilder sb)
		{
			IEnumerable<string> statementsGroupedByQueryHashAndSortedByCount = this.Rows
							.GroupBy(x => x.QueryHash)
							.Select(x => new
							{
								QueryHash = x.Key,
								NumberOfQueries = x.Count(),
								TotalDuration = x.Select(y => y.Duration).Aggregate((sum, current) => sum + current),
							})
							.OrderByDescending(x => x.NumberOfQueries)
							.Select(x => $"Query hash: {x.QueryHash}; Count: {x.NumberOfQueries}; Duration: {x.TotalDuration}");

			sb.AppendLine("Statements grouped by query hash and sorted by count:");
			foreach (string reportLine in statementsGroupedByQueryHashAndSortedByCount)
			{
				sb.AppendLine(reportLine);
			}
		}

		private void WriteAggregatedStatistics(StringBuilder sb)
		{
			ulong cpuTime = this.Rows.Select(x => x.CpuTime).Aggregate((sum, current) => sum + current);
			ulong duration = this.Rows.Select(x => x.Duration).Aggregate((sum, current) => sum + current);
			ulong physicalReads = this.Rows.Select(x => x.PhysicalReads).Aggregate((sum, current) => sum + current);
			ulong logicalReads = this.Rows.Select(x => x.LogicalReads).Aggregate((sum, current) => sum + current);
			ulong writes = this.Rows.Select(x => x.Writes).Aggregate((sum, current) => sum + current);
			ulong rowCount = this.Rows.Select(x => x.RowCount).Aggregate((sum, current) => sum + current);

			sb.AppendLine("Aggregated statistics:");
			sb.AppendLine($"{nameof(cpuTime)}: {cpuTime}");
			sb.AppendLine($"{nameof(duration)}: {duration}");
			sb.AppendLine($"{nameof(physicalReads)}: {physicalReads}");
			sb.AppendLine($"{nameof(logicalReads)}: {logicalReads}");
			sb.AppendLine($"{nameof(writes)}: {writes}");
			sb.AppendLine($"{nameof(rowCount)}: {rowCount}");
		}

		private IList<SqlStatementRowDto> GetStatementsSortedByDuration()
		{
			return this.Rows.OrderByDescending(x => x.Duration).ToList();
		}
	}
}
