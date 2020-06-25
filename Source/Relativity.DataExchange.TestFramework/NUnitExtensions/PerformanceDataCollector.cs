// <copyright file="PerformanceDataCollector.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;

	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataExchange.Transfer;

	using File = System.IO.File;

	public sealed class PerformanceDataCollector
	{
		private static PerformanceDataCollector _instance;
		private readonly Stopwatch stopwatch;
		private bool massImportImprovementsToggle;
		private TimeSpan jobExecutionTime;
		private int numberOfClients;
		private int numberOfDocumentsToImport;
		private int numberOfImagesPerDocument;
		private int maxNumberOfMultiValues;
		private string testCaseName;
		private TapiClient tapiClient;
		private int deadlocksCount;
		private string deadlocksFolderDetails;
		private bool storeDeadlocksInfo;

		private PerformanceDataCollector()
		{
			_instance = this;
			this.stopwatch = new Stopwatch();
			this.jobExecutionTime = stopwatch.Elapsed;
			this.tapiClient = TapiClient.None;
			this.storeDeadlocksInfo = false;
		}

		public static PerformanceDataCollector Instance => _instance ?? new PerformanceDataCollector();

		public static void SetUpPerformanceLogger()
		{
			_instance = new PerformanceDataCollector();
		}

		public void SetPerformanceTestValues(
			string testName,
			int clientsCount,
			int documentsCount,
			int imagesPerDocument,
			int maximumNumberOfMultiValues,
			TapiClient tapiClientName,
			IntegrationTestParameters parameters)
		{
			this.testCaseName = testName;
			this.numberOfClients = clientsCount;
			this.numberOfDocumentsToImport = documentsCount;
			this.numberOfImagesPerDocument = imagesPerDocument;
			this.maxNumberOfMultiValues = maximumNumberOfMultiValues;
			this.tapiClient = tapiClientName;
			this.massImportImprovementsToggle = MassImportImprovementsToggleChecker.GetMassImportToggleValueFromDatabase(parameters);
		}

		public void StorePerformanceResults()
		{
			// Do not add statistics from tests that were not executed
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Skipped)
			{
				return;
			}

			// TODO: Temporary solution with save test performance statistics to file
			string parentFolder = IntegrationTestHelper.IntegrationTestParameters.SqlProfilingReportsOutputPath;
			string outputFile = Path.Combine(parentFolder, "Summary.csv");
			Directory.CreateDirectory(parentFolder);

			string relativityVersion = RelativityVersionChecker.GetCurrentRelativityVersion(IntegrationTestHelper.IntegrationTestParameters).ToString();
			string testResultStatus = TestContext.CurrentContext.Result.Outcome.Status.ToString();

			if (!File.Exists(outputFile))
			{
				IEnumerable<string> fileHeader = new string[] { $"GiBranch|GitCommit|RelativityVersion|Date|TestCaseName|TestResultStatus|MassImportImprovementsToggle|NumberOfParallelApiClients|NumberOfDocumentsToImportByClient|NumberOfImagesPerDocument|MaxNumberOfMultiValues|FileTransferMode|ImportTime|DeadlocksCount|FolderWithDeadlocksDetails" };
				File.AppendAllLines(outputFile, fileHeader);
			}

			if (!this.storeDeadlocksInfo)
			{
				this.deadlocksFolderDetails = "CollectDeadlocks turned off";
			}

			IEnumerable<string> fileContent = new string[] { $"{GetGitBranchName()}|{GetCommitHash()}|{relativityVersion}|{DateTime.UtcNow.ToShortDateString()}|{this.testCaseName}|{testResultStatus}|{this.massImportImprovementsToggle}|{this.numberOfClients}|{this.numberOfDocumentsToImport}|{this.numberOfImagesPerDocument}|{this.maxNumberOfMultiValues}|{this.tapiClient.ToString()}|{this.jobExecutionTime.ToString()}|{this.deadlocksCount}|{this.deadlocksFolderDetails}" };
			File.AppendAllLines(outputFile, fileContent);
		}

		public void StoreDeadlocksCount(int numberOfDeadlocks, string folderWithDeadlocksDetails)
		{
			this.storeDeadlocksInfo = true;
			this.deadlocksCount = numberOfDeadlocks;
			this.deadlocksFolderDetails = folderWithDeadlocksDetails;
		}

		public void StartMeasureTime()
		{
			this.stopwatch.Start();
		}

		public void StopMeasureTime()
		{
			this.stopwatch.Stop();
			this.jobExecutionTime = this.stopwatch.Elapsed;
		}

		private static string GetCommitHash()
		{
			return ExecuteCommandAndReturnOutput($"git -C {GetRootFolder()} rev-parse --short HEAD");
		}

		private static string GetGitBranchName()
		{
			return ExecuteCommandAndReturnOutput($"git -C {GetRootFolder()} describe --all");
		}

		private static string GetRootFolder()
		{
			string executionAssembly = Assembly.GetExecutingAssembly().Location;
			return executionAssembly.Substring(0, executionAssembly.IndexOf("Source", StringComparison.CurrentCulture));
		}

		private static string ExecuteCommandAndReturnOutput(string command)
		{
			ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", $"/c {command}");
			processInfo.CreateNoWindow = true;
			processInfo.UseShellExecute = false;
			processInfo.RedirectStandardOutput = true;
			var process = Process.Start(processInfo);

			StreamReader reader = process.StandardOutput;
			string output = reader.ReadToEnd();

			process.WaitForExit(10000);
			process.Close();

			return output.Trim();
		}
	}
}
