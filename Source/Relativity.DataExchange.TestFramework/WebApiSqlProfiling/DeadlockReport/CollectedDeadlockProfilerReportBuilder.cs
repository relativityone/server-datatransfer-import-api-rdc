// ----------------------------------------------------------------------------
// <copyright file="CollectedDeadlockProfilerReportBuilder.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------;

namespace Relativity.DataExchange.TestFramework.WebApiSqlProfiling.DeadlockReport
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Xml.Linq;

	using Relativity.DataExchange.TestFramework.WebApiSqlProfiling.ExecutionPlan;

	internal class CollectedDeadlockProfilerReportBuilder : ProfilerReportBuilderBase<DeadlockReportRowDto>
	{
		public override ProfilerReport Build()
		{
			string description = BuildSummary(this.Rows);

			List<TextFileDto> files = this.Rows.SelectMany(GetSqlStatementFiles).Concat(GetSummaryFile(description)).ToList();

			return new ProfilerReport(description, files);
		}

		protected override void AddDetailsToRow(XElement element, DeadlockReportRowDto row)
		{
			element = element ?? throw new ArgumentNullException(nameof(element));
			row = row ?? throw new ArgumentNullException(nameof(row));

			if (string.Compare(element.Attribute("name").Value, "xml_report", StringComparison.OrdinalIgnoreCase) == 0)
			{
				ParseDeadlockReport(element, row);
			}
		}

		private static IEnumerable<TextFileDto> GetSqlStatementFiles(DeadlockReportRowDto dto)
		{
			return new[] { new TextFileDto(Guid.NewGuid().ToString(), "xdl", dto.DeadlockReport) };
		}

		private static IEnumerable<TextFileDto> GetSummaryFile(string description)
		{
			yield return new TextFileDto("DeadlockReportSummary", "txt", description);
		}

		private static void ParseDeadlockReport(XElement element, DeadlockReportRowDto row)
		{
			var deadLockElement = element.Descendants("deadlock").First();

			row.DeadlockReport = deadLockElement?.ToString();

			var ridLockElements = deadLockElement.Descendants("ridlock");

			foreach (var ridLockElem in ridLockElements)
			{
				row.LockObjectNames.Add(ridLockElem.Attribute("objectname")?.Value);
			}
		}

		private static string BuildSummary(List<DeadlockReportRowDto> deadlockReports)
		{
			const string ReportHeader = "SQL Profiling - Collect Deadlocks";

			var lockedObjectNames = deadlockReports.SelectMany(item => item.LockObjectNames)
				.GroupBy(objectName => objectName)
				.ToDictionary(objectName => objectName.Key, objectName => objectName.Count());

			var sb = new StringBuilder();
			sb.AppendLine($"{ReportHeader} - report");
			sb.AppendLine($"Number of deadlock occurence: {deadlockReports.Count}");

			foreach (var lockedObjectName in lockedObjectNames)
			{
				sb.AppendLine($"Deadlocks count on object name '{lockedObjectName.Key}' : {lockedObjectName.Value}");
			}

			sb.AppendLine($"{ReportHeader} - End of the report");
			return sb.ToString();
		}
	}
}
