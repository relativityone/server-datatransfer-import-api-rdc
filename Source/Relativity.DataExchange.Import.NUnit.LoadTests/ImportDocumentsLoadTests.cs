// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportDocumentsLoadTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
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
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[Feature.DataTransfer.TransferApi]
	[TestType.Load]
	[TestType.Performance]
	public class ImportDocumentsLoadTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		[CollectDeadlocks]
		[Performance]
		[UseSqlComparer]
		[Category(TestCategories.SqlComparer)]
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db08529387AB")]
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		[TestType.Load]
		[TestType.Performance]
		public async Task ShouldImportFoldersParallelAsync(
				[Values(2, 4, 6)] int parallelIApiClientCount,
				[Values(20_000)] int numberOfDocumentsPerIApiClient,
				[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportFoldersParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			FoldersValueSource foldersValueSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 10,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(WellKnownFields.FolderName, foldersValueSource);

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfRows = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfRows);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfRows));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		[CollectDeadlocks]
		[Performance]
		[UseSqlComparer]
		[Category(TestCategories.SqlComparer)]
		[IdentifiedTest("bbbc5de9-f6a4-4a4c-97a7-6f1f88d96c93")]
		public async Task ShouldImportChoicesInParallelAsync(
			[Values(2, 4, 6)] int parallelIApiClientCount,
			[Values(20_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportChoicesInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			var settingsBuilder = NativeImportSettingsBuilder.New().WithDefaultSettings();

			var fieldsToImport = await ComplexFieldBuilder.Create(this.TestParameters)
									 .WithMediumSingleChoiceField(1)
									 .WithMediumMultiChoiceField(1)
									 .Build()
									 .ConfigureAwait(false);

			var dataSourceBuilder = ImportDataSourceBuilder.New().AddField(
				WellKnownFields.ControlNumber,
				new IdentifierValueSource());

			foreach ((string fieldName, IFieldValueSource valuesSource) in fieldsToImport)
			{
				dataSourceBuilder.AddField(fieldName, valuesSource);
			}

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfDocuments = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfDocuments);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfDocuments));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		[CollectDeadlocks]
		[Performance]
		[IdentifiedTest("9f2f532e-7a16-4199-87ec-1308dbff27a7")]
		public async Task ShouldImportChoicesAndFoldersInParallelAsync(
			[Values(2, 4, 6)] int parallelIApiClientCount,
			[Values(20_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportChoicesAndFoldersInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 5,
				numberOfDifferentElements: 100,
				maximumElementLength: 50);

			var fieldsToImport = await ComplexFieldBuilder.Create(this.TestParameters)
									 .WithMediumSingleChoiceField(1)
									 .WithMediumMultiChoiceField(1)
									 .Build()
									 .ConfigureAwait(false);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(WellKnownFields.FolderName, foldersSource);

			foreach ((string fieldName, IFieldValueSource valuesSource) in fieldsToImport)
			{
				dataSourceBuilder.AddField(fieldName, valuesSource);
			}

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfDocuments = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfDocuments);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfDocuments));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		[CollectDeadlocks]
		[Performance]
		[IdentifiedTestCase("40c70223-0c66-4ccc-b234-1dd1726a260c", 1, 320_000, TapiClient.Direct)]
		[IdentifiedTestCase("a44e2969-e31b-44b4-aa16-94a9f2a0a5a7", 8, 40_000, TapiClient.Direct)]
		[IdentifiedTestCase("04cc904e-a951-4cfa-ad70-195b977ce873", 16, 20_000, TapiClient.Direct)]
		[IdentifiedTestCase("b730c67f-a132-4e76-8af4-2efaae9197b9", 1, 320_000, TapiClient.Aspera)]
		[IdentifiedTestCase("ea9606d1-dcfa-471f-a7b4-5adf9391133e", 8, 40_000, TapiClient.Aspera)]
		[IdentifiedTestCase("067c4400-176e-45bd-9b03-fd9b61301714", 16, 20_000, TapiClient.Aspera)]
		[IdentifiedTestCase("ca91fd30-9aeb-4252-b448-de153eda6a9a", 1, 320_000, TapiClient.Web)]
		[IdentifiedTestCase("7b561f15-2529-4a1b-9b10-8bedb0d99ecf", 8, 40_000, TapiClient.Web)]
		[IdentifiedTestCase("b312e44b-9a76-48b2-97bf-dd5770e53f30", 16, 20_000, TapiClient.Web)]
		public async Task ShouldImportDocumentsWithSingleAndMultiObjectsInParallelAsync(
			int parallelIApiClientCount,
			int numberOfDocumentsPerIApiClient,
			TapiClient client)
		{
			// ARRANGE
			if (parallelIApiClientCount == 16)
			{
				MassImportImprovementsToggleHelper.SkipTestIfMassImportImprovementsToggleHasValue(this.TestParameters, isEnabled: false);
			}

			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocumentsWithSingleAndMultiObjectsInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);

			// create single objects fields in workspace
			var fieldsToImport = await ComplexFieldBuilder.Create(this.TestParameters)

						 // Single Object fields
						 .WithSmallSingleObjectField(1)
						 .WithMediumSingleObjectField(1)
						 .WithLargeSingleObjectField(1)

						 // Multiple Object fields
						 .WithSmallMultiObjectsField(1)
						 .WithMediumMultiObjectsField(1)
						 .WithLargeMultiObjectsField(1)

						 .Build()
						 .ConfigureAwait(false);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());

			foreach ((string fieldName, IFieldValueSource valuesSource) in fieldsToImport)
			{
				dataSourceBuilder.AddField(fieldName, valuesSource);
			}

			var settingsBuilder = NativeImportSettingsBuilder.New().WithDefaultSettings();
			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfDocuments = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfDocuments);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfDocuments));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		[CollectDeadlocks]
		[Performance]
		[IdentifiedTestCase("e7a09288-ccf5-42ff-ae33-2e2dc8a603c2", 1, 160_000, 20, TapiClient.Direct)]
		[IdentifiedTestCase("f9b7c932-fb43-4089-88fe-3548c7f2f533", 1, 160_000, 100, TapiClient.Direct)]
		[IdentifiedTestCase("a5eb21ad-5631-4f36-a3d9-249aa4e2a9ff", 8, 20_000, 20, TapiClient.Direct)]
		[IdentifiedTestCase("84100c56-6368-4416-9b25-b5874d49a800", 16, 10_000, 20, TapiClient.Direct)]
		[IdentifiedTestCase("31ced63f-8722-42c7-8f66-4b05d1c1abf0", 1, 160_000, 20, TapiClient.Aspera)]
		[IdentifiedTestCase("abf0d121-b40f-4559-9c72-e73d85eb6ca3", 1, 160_000, 100, TapiClient.Aspera)]
		[IdentifiedTestCase("83b24cd0-b35f-49d8-b559-c532b81c6d07", 8, 20_000, 20, TapiClient.Aspera)]
		[IdentifiedTestCase("99efb70a-8d6f-4338-9cf0-b25d3b791ba9", 16, 10_000, 20, TapiClient.Aspera)]
		[IdentifiedTestCase("75a720ef-c736-4d50-95ad-b59e5467949b", 1, 160_000, 20, TapiClient.Web)]
		[IdentifiedTestCase("11e1027a-b4fd-44fc-aff4-9d724390bb13", 1, 160_000, 100, TapiClient.Web)]
		[IdentifiedTestCase("00a56efa-32ad-4eff-9cf3-163863a47807", 8, 20_000, 20, TapiClient.Web)]
		[IdentifiedTestCase("93eecd7b-b4cd-4bce-98ad-cf106ee3d780", 16, 10_000, 20, TapiClient.Web)]
		[IgnoreIfMassImportImprovementsToggleHasValue(isEnabled: false)]
		public async Task ShouldImportDocumentsWithBigNumberOfMultiObjectsPerDocumentParallelAsync(
			int parallelIApiClientCount,
			int numberOfDocumentsPerIApiClient,
			int maxNumberOfMultiValues,
			TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocumentsWithBigNumberOfMultiObjectsPerDocumentParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, maxNumberOfMultiValues, client, this.TestParameters);

			var fieldsToImport = await ComplexFieldBuilder.Create(this.TestParameters)
									 .WithLargeMultiObjectsField()
									 .Build()
									 .ConfigureAwait(false);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());

			foreach ((string fieldName, IFieldValueSource valuesSource) in fieldsToImport)
			{
				dataSourceBuilder.AddField(fieldName, valuesSource);
			}

			var settingsBuilder = NativeImportSettingsBuilder.New().WithDefaultSettings();
			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfDocuments = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfDocuments);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfDocuments));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}

		[CollectDeadlocks]
		[Performance]
		[IdentifiedTestCase("495ac24c-bf3e-4245-8179-9532b6f80291", 1, 640_000, TapiClient.Direct)]
		[IdentifiedTestCase("5091dd5d-d9ee-4e45-8444-47fec6caeb4d", 8, 80_000, TapiClient.Direct)]
		[IdentifiedTestCase("4b714377-b170-4884-b567-ac0c399f6fef", 16, 40_000, TapiClient.Direct)]
		[IdentifiedTestCase("b4548647-d753-496e-95e0-cd8c1d2efdb6", 1, 640_000, TapiClient.Aspera)]
		[IdentifiedTestCase("bf0f4afc-08ba-4f30-87b6-587c89dca889", 8, 80_000, TapiClient.Aspera)]
		[IdentifiedTestCase("16e64195-fe94-44b1-8420-8e386db3ee7f", 16, 40_000, TapiClient.Aspera)]
		[IdentifiedTestCase("7d7c1220-ff34-4516-959e-efc25643e6e3", 1, 640_000, TapiClient.Aspera)]
		[IdentifiedTestCase("49d141a2-1307-48d2-b3a3-f68e61e9fe45", 8, 80_000, TapiClient.Aspera)]
		[IdentifiedTestCase("041c8b6a-f538-4ac0-9b97-9a78bb74072a", 16, 40_000, TapiClient.Aspera)]
		public async Task ShouldImportDocumentsWithFolderChoicesAndObjectsInParallelAsync(
			int parallelIApiClientCount,
			int numberOfDocumentsPerIApiClient,
			TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocumentsWithFolderChoicesAndObjectsInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 5,
				numberOfDifferentElements: 100,
				maximumElementLength: 50);

			var fieldsToImport = await ComplexFieldBuilder.Create(this.TestParameters)
									 .WithMediumSingleChoiceField(1)
									 .WithMediumMultiChoiceField(1)
									 .WithSmallSingleObjectField(1)
									 .WithMediumMultiObjectsField(1)
									 .Build()
									 .ConfigureAwait(false);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(WellKnownFields.FolderName, foldersSource);

			foreach ((string fieldName, IFieldValueSource valuesSource) in fieldsToImport)
			{
				dataSourceBuilder.AddField(fieldName, valuesSource);
			}

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfDocuments = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfDocuments);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfDocuments));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
		}
	}
}