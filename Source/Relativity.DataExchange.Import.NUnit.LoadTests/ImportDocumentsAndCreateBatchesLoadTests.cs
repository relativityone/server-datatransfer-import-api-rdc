// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportDocumentsAndCreateBatchesLoadTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.TestFramework.PerformanceTests;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Batching;
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Services.Review.Batching.Models;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[Feature.DataTransfer.TransferApi]
	[TestType.Load]
	[TestType.Performance]
	public class ImportDocumentsAndCreateBatchesLoadTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		private const string IdentifierPrefix = "1-IAPI-test";
		private const string SingleObjectFieldName = "SingleObject";
		private const string AllDocumentsSearch = "All Documents";
		private const string BatchSetName = "Test";
		private const string BatchSetPrefix = "test";
		private const int BatchSetSize = 50;

		[Ignore("This test reproduces https://jira.kcura.com/browse/REL-486057. It has to be ignored until that problem is fixed.")]
		[CollectDeadlocks]
		[Performance]
		[UseSqlComparer]
		[IdentifiedTest("22429B4F-438B-4128-9A44-E7D26C7AE098")]
		public async Task ShouldCreateBatchesDuringParallelImportAsync(
			[Values(2, 4, 8, 16)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsInitialImport,
			[Values(1)] int numberOfSingleObjectFields,
			[Values(25_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldCreateBatchesDuringParallelImportAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			ForceClient(client);

			var settingsBuilder = NativeImportSettingsBuilder.New().WithDefaultSettings();

			// 1. import initial documents to the workspace
			await InitialImport(settingsBuilder, numberOfDocumentsInitialImport, numberOfSingleObjectFields).ConfigureAwait(false);

			// 2. create batch set
			var batchSet = await this.CreateBatchSetForSavedSearchAsync().ConfigureAwait(false);

			// 3. prepare for the parallel import of parallelIApiClientCount * numberOfDocumentsPerIApiClient
			var dataSourceBuilder = this.PrepareParallelImport(settingsBuilder, parallelIApiClientCount, numberOfSingleObjectFields);

			// ACT
			// parallel import of new documents while previously imported ones are being batched
			Task<ImportTestJobResult> parallelImportsTask = this.JobExecutionContext.ExecuteAsync(
				dataSourceBuilder,
				numberOfDocumentsPerIApiClient);

			Task<BatchProcessResult> batchSetCreationTask = BatchSetHelper.CreateBatchesAsync(TestParameters, batchSet, TestParameters.WorkspaceId);

			await Task.WhenAll(parallelImportsTask, batchSetCreationTask).ConfigureAwait(false);

			ImportTestJobResult results = parallelImportsTask.Result;

			// ASSERT
			int expectedNumberOfRows = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfRows);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfRows));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		private async Task InitialImport(NativeImportSettingsBuilder settingsBuilder, int numberOfDocumentsInitialImport, int numberOfSingleObjectFields)
		{
			var dataSourceBuilder = ImportDataSourceBuilder.New().AddField(
				WellKnownFields.ControlNumber, new IdentifierValueSource());

			for (int i = 0; i < numberOfSingleObjectFields; i++)
			{
				await RdoHelper.CreateObjectTypeAsync(this.TestParameters, SingleObjectFieldName + i).ConfigureAwait(false);

				var singleObjectsSource = new OneValueSource(SingleObjectFieldName + i);

				dataSourceBuilder.AddField(SingleObjectFieldName + i, singleObjectsSource);
			}

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, 1);

			await this.JobExecutionContext
				.ExecuteAsync(dataSourceBuilder, numberOfDocumentsInitialImport)
				.ConfigureAwait(false);
		}

		private ImportDataSourceBuilder PrepareParallelImport(NativeImportSettingsBuilder settingsBuilder, int parallelIApiClientCount, int numberOfSingleObjectFields)
		{
			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource(IdentifierPrefix));

			for (int i = 0; i < numberOfSingleObjectFields; i++)
			{
				var singleObjectsSource = new OneValueSource(SingleObjectFieldName + i);

				dataSourceBuilder.AddField(SingleObjectFieldName + i, singleObjectsSource);
			}

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			return dataSourceBuilder;
		}

		private async Task<BatchSet> CreateBatchSetForSavedSearchAsync()
		{
			IList<RelativityObject> relativityObjects = RdoHelper.QueryRelativityObjects(this.TestParameters, (int)ArtifactTypeID.SavedSearch, fields: new[] { WellKnownFields.TextIdentifier });
			RelativityObject allDocumentsSearch = relativityObjects.Single(x => x.FieldValues[0].Value.ToString() == AllDocumentsSearch);

			DataSource dataSource = new DataSource
			{
				ArtifactID = allDocumentsSearch.ArtifactID,
				Name = allDocumentsSearch.FieldValues[0].Value.ToString(),
				IsSecured = false,
			};

			return await BatchSetHelper.CreateBatchSetAsync(
								TestParameters,
								dataSource,
								TestParameters.WorkspaceId,
								BatchSetName,
								BatchSetPrefix,
								BatchSetSize).ConfigureAwait(false);
		}
	}
}