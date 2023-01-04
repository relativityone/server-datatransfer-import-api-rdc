// ----------------------------------------------------------------------------
// <copyright file="CollectedExecutionPlansProfilerReportBuilder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------;

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.ExecutionPlan
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;

	internal class CollectedExecutionPlansProfilerReportBuilder : ProfilerReportBuilderBase<ExecutionPlanReportRowDto>
	{
		public override ProfilerReport Build()
		{
			List<ExecutionPlanReportRowDto> sortedExecutionPlans = this.GetExecutionPlansSortedByDuration().ToList();
			string description = BuildDescription(sortedExecutionPlans);

			List<TextFileDto> files = sortedExecutionPlans
				.SelectMany(
					dto => GetSqlStatementFiles(dto).Concat(GetSqlPlanFiles(dto)))
				.Concat(GetSummaryFile(description))
				.ToList();

			return new ProfilerReport(description, files);
		}

		protected override void AddDetailsToRow(XElement element, ExecutionPlanReportRowDto row)
		{
			element = element ?? throw new ArgumentNullException(nameof(element));
			row = row ?? throw new ArgumentNullException(nameof(row));

			switch (element.Attribute("name").Value)
			{
				case "duration":
					row.Duration = this.GetNumericValue(element);
					break;
				case "query_hash":
					row.QueryHash = this.GetNumericValue(element);
					break;
				case "showplan_xml":
					SetExecutionPlans(element, row);
					break;
				case "sql_text":
					row.Sql = element.Element("value").Value;
					break;
			}
		}

		private static void SetExecutionPlans(XElement element, ExecutionPlanReportRowDto row)
		{
			if (row.ExecutionPlans != null)
			{
				throw new NotSupportedException("Multiple execution plans not supported");
			}

			row.ExecutionPlans = element
				.Element("value")
				.Nodes()
				.Select(x => x.ToString())
				.ToList();
		}

		private static IEnumerable<TextFileDto> GetSqlPlanFiles(ExecutionPlanReportRowDto dto)
		{
			return dto.ExecutionPlans.Select(
				(executionPlan, i) => new TextFileDto($"{GetFileName(dto)} - {i}", "sqlplan", executionPlan));
		}

		private static IEnumerable<TextFileDto> GetSqlStatementFiles(ExecutionPlanReportRowDto dto)
		{
			return new[] { new TextFileDto(GetFileName(dto), "sql", dto.Sql) };
		}

		private static string GetFileName(ExecutionPlanReportRowDto dto)
		{
			return $"{dto.Duration} - {dto.QueryHash}";
		}

		private static IEnumerable<TextFileDto> GetSummaryFile(string description)
		{
			yield return new TextFileDto("ExecutionPlansSummary", "txt", description);
		}

		private static string BuildDescription(IEnumerable<ExecutionPlanReportRowDto> sortedExecutionPlans)
		{
			const string ReportHeader = "SQL Profiling - Collect Execution Plans";

			IEnumerable<string> numberOfQueryByHash = sortedExecutionPlans
				.GroupBy(x => x.QueryHash)
				.Select(
					x =>
						new
						{
							QueryHash = x.Key,
							NumberOfQueries = x.Count(),
							TotalDuration = x.Select(y => y.Duration).Aggregate((sum, current) => sum + current),
						})
				.OrderByDescending(x => x.NumberOfQueries)
				.Select(x => $"Query hash: {x.QueryHash}; Count: {x.NumberOfQueries}; Duration: {x.TotalDuration}");

			var sb = new StringBuilder();
			sb.AppendLine($"{ReportHeader} - report:");
			sb.AppendLine("Number of queries by hash:");
			foreach (string x in numberOfQueryByHash)
			{
				sb.AppendLine(x);
			}

			sb.AppendLine($"{ReportHeader} - End of the report");

			return sb.ToString();
		}

		private IEnumerable<ExecutionPlanReportRowDto> GetExecutionPlansSortedByDuration()
		{
			return this.Rows.OrderByDescending(x => x.Duration);
		}
	}
}
