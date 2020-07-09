// <copyright file="ProfilingSessionTestCommand.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;
	using System.IO;
	using System.Linq;

	using NUnit.Framework.Internal;
	using NUnit.Framework.Internal.Commands;

	using Relativity.DataExchange.TestFramework.PerformanceTests;
	using Relativity.DataExchange.TestFramework.WebApiSqlProfiling;

	internal class ProfilingSessionTestCommand : BeforeAndAfterTestCommand
	{
		private readonly string rootOutputPath;
		private readonly ProfilingSession profilingSession;

		public ProfilingSessionTestCommand(TestCommand innerCommand, ProfilingSession profilingSession, string outputPath)
			: base(innerCommand)
		{
			this.profilingSession = profilingSession;
			this.rootOutputPath = outputPath;

			this.BeforeTest = this.ExecuteBeforeTest;
			this.AfterTest = this.ExecuteAfterTest;
		}

		private void ExecuteBeforeTest(TestExecutionContext context)
		{
			this.profilingSession.StartProfilingForRelativityWebApi();
		}

		private void ExecuteAfterTest(TestExecutionContext context)
		{
			ProfilerReport report = this.profilingSession.ReadCapturedEventsAndStopProfiling();
			this.WriteProfilerReport(context, report);
		}

		private void WriteProfilerReport(TestExecutionContext context, ProfilerReport profilingReport)
		{
			context.OutWriter.WriteLine(profilingReport.Description);
			if (profilingReport.Files.Any())
			{
				this.WriteFiles(context, profilingReport);
			}
		}

		private void WriteFiles(TestExecutionContext context, ProfilerReport profilingReport)
		{
			string outputPath = this.GetDirectoryPath(context);
			foreach (TextFileDto fileDto in profilingReport.Files)
			{
				string filePath = Path.Combine(outputPath, $"{fileDto.Name}.{fileDto.Extension}");
				File.WriteAllText(filePath, fileDto.Content);
			}

			int deadlocksCount = profilingReport.Files.Count() - 1;
			PerformanceDataCollector.Instance.StoreDeadlocksCount(deadlocksCount, deadlocksCount == 0 ? string.Empty : outputPath);
		}

		private string GetDirectoryPath(TestExecutionContext context)
		{
			const string PropertyKey = "ProfilerReportDirectory";

			if (!context.CurrentTest.Properties.ContainsKey(PropertyKey))
			{
				string testName = context.CurrentTest.Name;
				string currentUtcTime = DateTime.UtcNow.ToString("yyyyMMddTHHmmss");
				string subDirectoryName = $"{testName.Replace(",", "_")}-{currentUtcTime}";
				string outputPath = Path.Combine(this.rootOutputPath, subDirectoryName);
				Directory.CreateDirectory(outputPath);

				context.CurrentTest.Properties.Set(PropertyKey, outputPath);
			}

			return context.CurrentTest.Properties.Get(PropertyKey) as string;
		}
	}
}
