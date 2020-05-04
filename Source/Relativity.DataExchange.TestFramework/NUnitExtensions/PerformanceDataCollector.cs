// <copyright file="PerformanceDataCollector.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.NUnitExtensions
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.IO;
	using NUnit.Framework;
	using NUnit.Framework.Interfaces;

	using Relativity.DataExchange.Transfer;
	using File = System.IO.File;

	public sealed class PerformanceDataCollector
	{
		private static PerformanceDataCollector _instance;
		private readonly Stopwatch stopwatch;
		private readonly bool massImportImprovementsToggle;
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
			this.massImportImprovementsToggle = false; // TODO: this value will be set on pipeline script, all tests should be executed for both values true and false
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
			TapiClient tapiClientName)
		{
			this.testCaseName = testName;
			this.numberOfClients = clientsCount;
			this.numberOfDocumentsToImport = documentsCount;
			this.numberOfImagesPerDocument = imagesPerDocument;
			this.maxNumberOfMultiValues = maximumNumberOfMultiValues;
			this.tapiClient = tapiClientName;
		}

		public void StorePerformanceResults()
		{
			// Do not add statistics from tests that were not executed
			if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Skipped)
			{
				return;
			}

			// TODO: Temporary solution with save test performance statistics to file
			// TODO: add to output statistics: WebApiVersion, RelativityVersion, dateOfTestExecution
			string parentFolder = IntegrationTestHelper.IntegrationTestParameters.SqlProfilingReportsOutputPath;
			string outputFile = System.IO.Path.Combine(parentFolder, "Summary.csv");
			Directory.CreateDirectory(parentFolder);

			if (!File.Exists(outputFile))
			{
				IEnumerable<string> fileHeader = new string[] { $"TestCaseName|TestResultStatus|MassImportImprovementsToggle|NumberOfParallelApiClients|NumberOfDocumentsToImportByClient|NumberOfImagesPerDocument|MaxNumberOfMultiValues|FileTransferMode|ImportTime|DeadlocksCount|FolderWithDeadlocksDetails" };
				File.AppendAllLines(outputFile, fileHeader);
			}

			string testResultStatus = TestContext.CurrentContext.Result.Outcome.Status.ToString();

			if (!this.storeDeadlocksInfo)
			{
				this.deadlocksFolderDetails = "CollectDeadlocks turned off";
			}

			IEnumerable<string> fileContent = new string[] { $"{this.testCaseName}|{testResultStatus}|{this.massImportImprovementsToggle}|{this.numberOfClients}|{this.numberOfDocumentsToImport}|{this.numberOfImagesPerDocument}|{this.maxNumberOfMultiValues}|{this.tapiClient.ToString()}|{this.jobExecutionTime.ToString()}|{this.deadlocksCount}|{this.deadlocksFolderDetails}" };
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
	}
}
