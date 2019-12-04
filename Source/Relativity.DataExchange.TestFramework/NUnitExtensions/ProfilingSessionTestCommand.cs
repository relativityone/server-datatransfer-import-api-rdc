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

	using Relativity.DataExchange.TestFramework.WebApiSqlProfiling;

	internal class ProfilingSessionTestCommand : BeforeAndAfterTestCommand
	{
		private const string RootOutputPath = @"C:\SqlProfiling";

		private readonly ProfilingSession profilingSession;

		public ProfilingSessionTestCommand(TestCommand innerCommand, ProfilingSession profilingSession)
			: base(innerCommand)
		{
			this.profilingSession = profilingSession;

			this.BeforeTest = this.ExecuteBeforeTest;
			this.AfterTest = this.ExecuteAfterTest;
		}

		private static void WriteProfilerReport(TestExecutionContext context, ProfilerReport profilingReport)
		{
			context.OutWriter.WriteLine(profilingReport.Description);

			if (profilingReport.Files.Any())
			{
				WriteFiles(context, profilingReport);
			}
		}

		private static void WriteFiles(TestExecutionContext context, ProfilerReport profilingReport)
		{
			string outputPath = GetDirectoryPath(context);
			foreach (TextFileDto fileDto in profilingReport.Files)
			{
				string filePath = Path.Combine(outputPath, $"{fileDto.Name}.{fileDto.Extension}");
				File.WriteAllText(filePath, fileDto.Content);
			}
		}

		private static string GetDirectoryPath(TestExecutionContext context)
		{
			const string PropertyKey = "ProfilerReportDirectory";

			if (!context.CurrentTest.Properties.ContainsKey(PropertyKey))
			{
				string testName = context.CurrentTest.Name;
				string currentUtcTime = DateTime.UtcNow.ToString("yyyyMMddTHHmmss");
				string subDirectoryName = $"{testName}-{currentUtcTime}";
				string outputPath = Path.Combine(RootOutputPath, subDirectoryName);
				Directory.CreateDirectory(outputPath);

				context.CurrentTest.Properties.Set(PropertyKey, outputPath);
			}

			return context.CurrentTest.Properties.Get(PropertyKey) as string;
		}

		private void ExecuteBeforeTest(TestExecutionContext context)
		{
			this.profilingSession.StartProfilingForRelativityWebApi();
		}

		private void ExecuteAfterTest(TestExecutionContext context)
		{
			ProfilerReport report = this.profilingSession.ReadCapturedEventsAndStopProfiling();
			WriteProfilerReport(context, report);
		}
	}
}
