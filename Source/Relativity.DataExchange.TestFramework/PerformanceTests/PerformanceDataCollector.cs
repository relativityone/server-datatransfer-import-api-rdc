// <copyright file="PerformanceDataCollector.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.PerformanceTests
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using NUnit.Framework;
	using NUnit.Framework.Interfaces;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.DataExchange.Transfer;

	public sealed class PerformanceDataCollector
	{
		private static PerformanceDataCollector _instance;
		private readonly Stopwatch stopwatch;
		private string massImportImprovementsToggle;
		private TimeSpan jobExecutionTime;
		private int numberOfClients;
		private int numberOfDocumentsToImport;
		private int numberOfImagesPerDocument;
		private int maxNumberOfMultiValues;
		private string testCaseName;
		private TapiClient tapiClient;
		private int deadlocksCount;
		private string deadlocksFolderDetails;

		private PerformanceDataCollector()
		{
			_instance = this;
			this.stopwatch = new Stopwatch();
			this.jobExecutionTime = stopwatch.Elapsed;
			this.tapiClient = TapiClient.None;
			this.deadlocksFolderDetails = "CollectDeadlocks turned off";
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
			this.massImportImprovementsToggle = MassImportImprovementsToggleHelper.GetDisplayableMassImportImprovementsToggle(parameters);
		}

		public void StorePerformanceResults()
		{
			if (TestContext.CurrentContext.Result.Outcome.Status != TestStatus.Skipped)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{ "GiBranch", GitInformation.GetGitBranchName },
					{ "GitCommit", GitInformation.GetCommitHash },
					{ "RelativityVersion", RelativityVersionChecker.GetCurrentRelativityVersion(IntegrationTestHelper.IntegrationTestParameters).ToString() },
					{ "Date", DateTime.UtcNow.ToShortDateString() },
					{ "TestCaseName", this.testCaseName },
					{ "TestResultStatus", TestContext.CurrentContext.Result.Outcome.Status.ToString() },
					{ "MassImportImprovementsToggle", this.massImportImprovementsToggle },
					{ "NumberOfParallelApiClients", this.numberOfClients.ToString() },
					{ "NumberOfDocumentsToImportByClient", this.numberOfDocumentsToImport.ToString() },
					{ "NumberOfImagesPerDocument", this.numberOfImagesPerDocument.ToString() },
					{ "MaxNumberOfMultiValues", this.maxNumberOfMultiValues.ToString() },
					{ "FileTransferMode", this.tapiClient.ToString() },
					{ "ImportTime", this.jobExecutionTime.ToString() },
					{ "DeadlocksCount", this.deadlocksCount.ToString() },
					{ "FolderWithDeadlocksDetails", this.deadlocksFolderDetails },
				};

				PerformanceMetricsSender.SendPerformanceMetrics(dictionary);
			}
		}

		public void StoreDeadlocksCount(int numberOfDeadlocks, string folderWithDeadlocksDetails)
		{
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