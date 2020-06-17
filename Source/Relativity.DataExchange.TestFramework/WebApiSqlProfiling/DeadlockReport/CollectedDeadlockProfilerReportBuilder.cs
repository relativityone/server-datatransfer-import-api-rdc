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
		private const string WebAPIClientAppPrefix = "RelativityWebAPI";

		public override ProfilerReport Build()
		{
			string description = BuildSummary(this.Rows);

			List<TextFileDto> files = this.Rows.SelectMany(GetDeadlockGraphFiles).Concat(GetSummaryFile(description)).ToList();

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

		private static IEnumerable<TextFileDto> GetDeadlockGraphFiles(DeadlockReportRowDto dto)
		{
			if (IsWebApiDeadlock(dto))
			{
				yield return new TextFileDto(Guid.NewGuid().ToString(), "xdl", dto.DeadlockReport);
			}
		}

		private static IEnumerable<TextFileDto> GetSummaryFile(string description)
		{
			yield return new TextFileDto("DeadlockReportSummary", "txt", description);
		}

		private static void ParseDeadlockReport(XElement element, DeadlockReportRowDto row)
		{
			var deadLockElement = element.Descendants("deadlock").First();

			row.DeadlockReport = deadLockElement.ToString();

			row.VictimProcessId = deadLockElement
				.Descendants("victimProcess")
				.Single()
				.Attribute("id")
				.Value;

			foreach (var process in deadLockElement.Descendants("process"))
			{
				string processId = process.Attribute("id").Value;
				string clientApp = process.Attribute("clientapp").Value;
				row.ProcessIdToClientApp[processId] = clientApp;
			}

			var resourceList = deadLockElement.Descendants("resource-list").Single();

			foreach (var lockElement in resourceList.Elements())
			{
				string objectName = lockElement.Attribute("objectname")?.Value;
				row.LockedObjectsNames.Add(objectName);
			}
		}

		private static string BuildSummary(IEnumerable<DeadlockReportRowDto> deadlockReports)
		{
			const string ReportHeader = "SQL Profiling - Collect Deadlocks";

			deadlockReports = deadlockReports.Where(IsWebApiDeadlock).ToList();

			var lockedObjectNames = deadlockReports
				.SelectMany(item => item.LockedObjectsNames)
				.GroupBy(objectName => objectName)
				.OrderByDescending(item => item.Count());

			int numberOfDeadlocks = deadlockReports.Count();
			int numberOfWebApiVictims = deadlockReports
				.Select(deadlockReport => deadlockReport.ProcessIdToClientApp[deadlockReport.VictimProcessId])
				.Count(victimClientApp => victimClientApp.StartsWith(WebAPIClientAppPrefix, StringComparison.OrdinalIgnoreCase));

			var sb = new StringBuilder();
			sb.AppendLine($"{ReportHeader} - report");
			sb.AppendLine($"Number of deadlock occurence: {numberOfDeadlocks}");
			sb.AppendLine($"Number of WebAPI victims: {numberOfWebApiVictims}");
			sb.AppendLine($"Number of other victims: {numberOfDeadlocks - numberOfWebApiVictims}");
			sb.AppendLine();

			foreach (var lockedObjectName in lockedObjectNames)
			{
				sb.AppendLine($"Deadlocks count on object name '{lockedObjectName.Key}' : {lockedObjectName.Count()}");
			}

			return sb.AppendLine().AppendLine($"{ReportHeader} - End of the report").ToString();
		}

		private static bool IsWebApiDeadlock(DeadlockReportRowDto deadlockReport)
		{
			return deadlockReport
				.ProcessIdToClientApp
				.Values
				.Any(clientApp => clientApp.StartsWith(WebAPIClientAppPrefix, StringComparison.OrdinalIgnoreCase));
		}
	}
}
