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
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
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
	using Relativity.Testing.Identification;

	[Explicit]
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportDocumentsLoadTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		private const string SingleChoiceFieldName = "SingleChoice";
		private const string MultiChoiceFieldName = "MultiChoice";
		private const string SingleObjectFieldName = "SingleObject";
		private const string MultiObjectFieldName = "MultiObject";

		[UseSqlComparer]
		[CollectDeadlocks]
		[Performance]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.TransferApi)]
		[Category(TestCategories.LoadTest)]
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db08529387AB")]
		public async Task ShouldImportFoldersParallelAsync(
			[Values(2, 4, 8, 16)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportFoldersParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			ForceClient(client);

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

		[UseSqlComparer]
		[CollectDeadlocks]
		[Performance]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.TransferApi)]
		[Category(TestCategories.LoadTest)]
		[IdentifiedTest("bbbc5de9-f6a4-4a4c-97a7-6f1f88d96c93")]
		public async Task ShouldImportChoicesInParallelAsync(
			[Values(2, 4, 8, 16)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportChoicesInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			ForceClient(client);
			var settingsBuilder = NativeImportSettingsBuilder.New().WithDefaultSettings();

			await this.CreateChoiceFieldsAsync().ConfigureAwait(false);
			var singleChoicesSource = new ChoicesValueSource(
				numberOfDifferentPaths: 4,
				maximumPathDepth: 1,
				numberOfDifferentElements: 4,
				maximumElementLength: 200);

			var multiChoicesSource = new ChoicesValueSource(
				numberOfDifferentPaths: 100,
				maximumPathDepth: 4,
				numberOfDifferentElements: 250,
				maximumElementLength: 50);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(SingleChoiceFieldName, singleChoicesSource)
				.AddField(MultiChoiceFieldName, multiChoicesSource);

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
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.TransferApi)]
		[Category(TestCategories.LoadTest)]
		[IdentifiedTest("9f2f532e-7a16-4199-87ec-1308dbff27a7")]
		public async Task ShouldImportChoicesAndFoldersInParallelAsync(
			[Values(2, 4, 8, 16)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportChoicesAndFoldersInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			ForceClient(client);

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 5,
				numberOfDifferentElements: 100,
				maximumElementLength: 50);

			await this.CreateChoiceFieldsAsync().ConfigureAwait(false);
			var singleChoicesSource = new ChoicesValueSource(
				numberOfDifferentPaths: 4,
				maximumPathDepth: 1,
				numberOfDifferentElements: 4,
				maximumElementLength: 200);

			var multiChoicesSource = new ChoicesValueSource(
				numberOfDifferentPaths: 100,
				maximumPathDepth: 4,
				numberOfDifferentElements: 250,
				maximumElementLength: 50);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(WellKnownFields.FolderName, foldersSource)
				.AddField(SingleChoiceFieldName, singleChoicesSource)
				.AddField(MultiChoiceFieldName, multiChoicesSource);

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
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.TransferApi)]
		[Category(TestCategories.LoadTest)]
		[IdentifiedTestCase("40c70223-0c66-4ccc-b234-1dd1726a260c", 1, 320_000, TapiClient.Direct)]
		[IdentifiedTestCase("a44e2969-e31b-44b4-aa16-94a9f2a0a5a7", 8, 40_000, TapiClient.Direct)]
		[IdentifiedTestCase("04cc904e-a951-4cfa-ad70-195b977ce873", 16, 20_000, TapiClient.Direct)]
		[IdentifiedTestCase("b730c67f-a132-4e76-8af4-2efaae9197b9", 1, 320_000, TapiClient.Aspera)]
		[IdentifiedTestCase("ea9606d1-dcfa-471f-a7b4-5adf9391133e", 8, 40_000, TapiClient.Aspera)]
		[IdentifiedTestCase("067c4400-176e-45bd-9b03-fd9b61301714", 16, 20_000, TapiClient.Aspera)]
		[IdentifiedTestCase("ca91fd30-9aeb-4252-b448-de153eda6a9a", 1, 320_000, TapiClient.Web)]
		[IdentifiedTestCase("7b561f15-2529-4a1b-9b10-8bedb0d99ecf", 8, 40_000, TapiClient.Web)]
		[IdentifiedTestCase("b312e44b-9a76-48b2-97bf-dd5770e53f30", 16, 20_000, TapiClient.Web)]
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		public async Task ShouldImportDocumentsWithSingleAndMultiObjectsInParallelAsync(
			int parallelIApiClientCount,
			int numberOfDocumentsPerIApiClient,
			TapiClient client)
		{
			// ARRANGE
			if (parallelIApiClientCount == 16)
			{
				MassImportImprovementsToggleChecker.SkipTestIfMassImportImprovementToggleOff(this.TestParameters);
			}

			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocumentsWithSingleAndMultiObjectsInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			ForceClient(client);

			// create single objects fields in workspace
			var smallSingleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
				new IdentifierValueSource("SMALL-SINGLE"),
				numberOfObjects: 100);
			var mediumSingleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
				new IdentifierValueSource("MEDIUM-SINGLE"),
				numberOfObjects: 100);
			var bigSingleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
				new IdentifierValueSource("BIG-SINGLE"),
				numberOfObjects: 100_000);
			var singleObjectFieldsToCreate = new (int numberOfFields, ObjectNameValueSource valueSource)[]
												{
													(5, smallSingleObjectsSource),
													(2, mediumSingleObjectsSource),
													(1, bigSingleObjectsSource),
												};
			IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)> singleObjectFieldsToImport
				= await this.CreateSingleObjectsFieldsAsync(singleObjectFieldsToCreate).ConfigureAwait(false);

			// create multi objects fields in workspace
			var smallMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
				new IdentifierValueSource("SMALL-MULTI"),
				numberOfObjects: 100,
				maxNumberOfMultiValues: 3);
			var mediumMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
				new IdentifierValueSource("MEDIUM-MULTI"),
				numberOfObjects: 5_000,
				maxNumberOfMultiValues: 3);
			var bigMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
				new IdentifierValueSource("BIG-MULTI"),
				numberOfObjects: 100_000,
				maxNumberOfMultiValues: 10);
			var multiObjectFieldsToCreate = new (int numberOfFields, ObjectNameValueSource valueSource)[]
										{
											(5, smallMultiObjectsSource),
											(2, mediumMultiObjectsSource),
											(1, bigMultiObjectsSource),
										};
			IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)> multiObjectFieldsToImport
				= await this.CreateMultiObjectsFieldsAsync(multiObjectFieldsToCreate).ConfigureAwait(false);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());

			foreach ((string fieldName, ObjectNameValueSource valuesSource) in singleObjectFieldsToImport.Concat(multiObjectFieldsToImport))
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
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.LoadTest)]
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
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		public async Task ShouldImportDocumentsWithBigNumberOfMultiObjectsPerDocumentParallelAsync(
			int parallelIApiClientCount,
			int numberOfDocumentsPerIApiClient,
			int maxNumberOfMultiValues,
			TapiClient client)
		{
			// ARRANGE
			MassImportImprovementsToggleChecker.SkipTestIfMassImportImprovementToggleOff(this.TestParameters);
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocumentsWithBigNumberOfMultiObjectsPerDocumentParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, maxNumberOfMultiValues, client, this.TestParameters);
			ForceClient(client);

			int secondObjectTypeArtifactId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, MultiObjectFieldName).ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(this.TestParameters, MultiObjectFieldName, secondObjectTypeArtifactId, (int)ArtifactType.Document).ConfigureAwait(false);

			var bigMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
				new IdentifierValueSource("BIG-MULTI"),
				numberOfObjects: 100_000,
				maxNumberOfMultiValues: maxNumberOfMultiValues);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(MultiObjectFieldName, bigMultiObjectsSource);

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
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.TransferApi)]
		[Category(TestCategories.LoadTest)]
		[IdentifiedTestCase("495ac24c-bf3e-4245-8179-9532b6f80291", 1, 640_000, TapiClient.Direct)]
		[IdentifiedTestCase("5091dd5d-d9ee-4e45-8444-47fec6caeb4d", 8, 80_000, TapiClient.Direct)]
		[IdentifiedTestCase("4b714377-b170-4884-b567-ac0c399f6fef", 16, 40_000, TapiClient.Direct)]
		[IdentifiedTestCase("b4548647-d753-496e-95e0-cd8c1d2efdb6", 1, 640_000, TapiClient.Aspera)]
		[IdentifiedTestCase("bf0f4afc-08ba-4f30-87b6-587c89dca889", 8, 80_000, TapiClient.Aspera)]
		[IdentifiedTestCase("16e64195-fe94-44b1-8420-8e386db3ee7f", 16, 40_000, TapiClient.Aspera)]
		[IdentifiedTestCase("7d7c1220-ff34-4516-959e-efc25643e6e3", 1, 640_000, TapiClient.Aspera)]
		[IdentifiedTestCase("49d141a2-1307-48d2-b3a3-f68e61e9fe45", 8, 80_000, TapiClient.Aspera)]
		[IdentifiedTestCase("041c8b6a-f538-4ac0-9b97-9a78bb74072a", 16, 40_000, TapiClient.Aspera)]
		[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
		public async Task ShouldImportDocumentsWithFolderChoicesAndObjectsInParallelAsync(
			int parallelIApiClientCount,
			int numberOfDocumentsPerIApiClient,
			TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocumentsWithFolderChoicesAndObjectsInParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			ForceClient(client);

			int firstObjectTypeArtifactId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, SingleObjectFieldName).ConfigureAwait(false);
			int secondObjectTypeArtifactId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, MultiObjectFieldName).ConfigureAwait(false);
			await FieldHelper.CreateSingleObjectFieldAsync(this.TestParameters, SingleObjectFieldName, firstObjectTypeArtifactId, (int)ArtifactType.Document).ConfigureAwait(false);
			await FieldHelper.CreateMultiObjectFieldAsync(this.TestParameters, MultiObjectFieldName, secondObjectTypeArtifactId, (int)ArtifactType.Document).ConfigureAwait(false);
			await this.CreateChoiceFieldsAsync().ConfigureAwait(false);

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 5,
				numberOfDifferentElements: 100,
				maximumElementLength: 50);

			var singleChoicesSource = new ChoicesValueSource(
				numberOfDifferentPaths: 4,
				maximumPathDepth: 1,
				numberOfDifferentElements: 4,
				maximumElementLength: 200);

			var multiChoicesSource = new ChoicesValueSource(
				numberOfDifferentPaths: 100,
				maximumPathDepth: 4,
				numberOfDifferentElements: 250,
				maximumElementLength: 50);

			var singleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
				new IdentifierValueSource("SINGLE"),
				numberOfObjects: 100);

			var multiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
					new IdentifierValueSource("OBJ"),
					numberOfObjects: 5000,
					maxNumberOfMultiValues: 5);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(WellKnownFields.FolderName, foldersSource)
				.AddField(SingleObjectFieldName, singleObjectsSource)
				.AddField(MultiObjectFieldName, multiObjectsSource)
				.AddField(SingleChoiceFieldName, singleChoicesSource)
				.AddField(MultiChoiceFieldName, multiChoicesSource);

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

		private Task<IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)>> CreateSingleObjectsFieldsAsync(
			(int numberOfFields, ObjectNameValueSource valueSource)[] input)
		{
			return this.CreateObjectsFieldsAsync("Single", input, CreateFieldAsync);

			Task CreateFieldAsync(string fieldName, int objectTypeArtifactId)
			{
				return FieldHelper.CreateSingleObjectFieldAsync(
					this.TestParameters,
					fieldName,
					objectTypeArtifactId,
					(int)ArtifactType.Document);
			}
		}

		private Task<IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)>> CreateMultiObjectsFieldsAsync(
			(int numberOfFields, ObjectNameValueSource valueSource)[] input)
		{
			return this.CreateObjectsFieldsAsync("Multi", input, CreateFieldAsync);

			Task CreateFieldAsync(string fieldName, int objectTypeArtifactId)
			{
				return FieldHelper.CreateMultiObjectFieldAsync(
					this.TestParameters,
					fieldName,
					objectTypeArtifactId,
					(int)ArtifactType.Document);
			}
		}

		private async Task<IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)>> CreateObjectsFieldsAsync(
			string fieldNamePrefix,
			(int numberOfFields, ObjectNameValueSource valueSource)[] input,
			Func<string, int, Task> createFieldFunctionAsync)
		{
			var output = new List<(string fieldName, ObjectNameValueSource valuesSource)>();
			for (int i = 0; i < input.Length; i++)
			{
				(int numberOfFields, ObjectNameValueSource valueSource) = input[i];

				for (int j = 0; j < numberOfFields; j++)
				{
					string fieldName = $"{fieldNamePrefix}-{i}-{j}";
					int objectTypeArtifactId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, fieldName).ConfigureAwait(false);
					await createFieldFunctionAsync(fieldName, objectTypeArtifactId).ConfigureAwait(false);

					output.Add((fieldName, valueSource));
				}
			}

			return output;
		}

		[SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ownership is transfered to Task.WhenAll.")]
		private Task CreateChoiceFieldsAsync()
		{
			Task<int> createSingleChoiceFieldTask = FieldHelper.CreateSingleChoiceFieldAsync(
				this.TestParameters,
				SingleChoiceFieldName,
				(int)ArtifactType.Document,
				false);
			Task<int> createMultiChoiceFieldTask = FieldHelper.CreateMultiChoiceFieldAsync(
				this.TestParameters,
				MultiChoiceFieldName,
				(int)ArtifactType.Document,
				false);

			return Task.WhenAll(createSingleChoiceFieldTask, createMultiChoiceFieldTask);
		}
	}
}