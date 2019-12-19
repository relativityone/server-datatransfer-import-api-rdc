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

			if (string.Equals(element.Attribute("name").Value, "xml_report"))
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

			row.DeadlockReport = deadLockElement.ToString();

			var processes = deadLockElement.Descendants("process");

			var ridToSqls = new List<(string RID, string SqlText)>();

			foreach (var process in processes)
			{
				string sql = process.Descendants("inputbuf").First().Value;
				string rid = process.Attribute("waitresource").Value;
				ridToSqls.Add((RID: rid, SqlText: sql));
			}

			var ridLockElements = deadLockElement.Descendants("ridlock");

			foreach (var ridLockElem in ridLockElements)
			{
				string objectName = ridLockElem.Attribute("objectname")?.Value;
				string ridToCompare =
					$"{ridLockElem.Attribute("dbid")?.Value}:{ridLockElem.Attribute("fileid")?.Value}:{ridLockElem.Attribute("pageid")?.Value}";

				string foundSQL = ridToSqls.Find(item => item.RID.Contains(ridToCompare)).SqlText;

				row.LockedObjectInfo.Add((Name: objectName, Sql: foundSQL));
			}
		}

		private static string BuildSummary(List<DeadlockReportRowDto> deadlockReports)
		{
			const string ReportHeader = "SQL Profiling - Collect Deadlocks";

			var lockedObjectNames = deadlockReports.SelectMany(item => item.LockedObjectInfo)
				.GroupBy(objectName => objectName)
				.ToDictionary(objectName => objectName.Key, objectName => objectName.Count());

			var sb = new StringBuilder();
			sb.AppendLine($"{ReportHeader} - report");
			sb.AppendLine($"Number of deadlock occurence: {deadlockReports.Count}").AppendLine();

			foreach (var lockedObjectName in lockedObjectNames)
			{
				sb.AppendLine($"Deadlocks count on object name '{lockedObjectName.Key.Name}' : {lockedObjectName.Value}");
			}

			sb.AppendLine();
			sb.AppendLine("Details:");
			foreach (var lockObject in deadlockReports.SelectMany(item => item.LockedObjectInfo))
			{
				sb.AppendLine($"Deadlocks object name '{lockObject.Name}' - Sql: {lockObject.Sql}").AppendLine();
			}

			return sb.AppendLine($"{ReportHeader} - End of the report").ToString();
		}
	}
}
