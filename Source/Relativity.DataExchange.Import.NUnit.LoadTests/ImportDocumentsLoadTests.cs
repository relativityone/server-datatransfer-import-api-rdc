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

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Explicit]
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportDocumentsLoadTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		[CollectDeadlocks]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("b9b6897f-ea3f-4694-80d2-db08529387AB")]
		public async Task ShouldImportFoldersParallelAsync(
			[Values(16, 8)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);
			ForceClient(client);

			var settingsBuilder = new NativeImportSettingsBuilder();
			settingsBuilder = settingsBuilder
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			FoldersValueSource foldersValueSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 10,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var dataSourceBuilder = new ImportDataSourceBuilder();
			dataSourceBuilder.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());
			dataSourceBuilder.AddField(WellKnownFields.FolderName, foldersValueSource);

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
		}

		[CollectDeadlocks]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("bbbc5de9-f6a4-4a4c-97a7-6f1f88d96c93")]
		public async Task ShouldImportChoicesInParallelAsync(
			[Values(16, 8)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);
			ForceClient(client);
			var settingsBuilder = new NativeImportSettingsBuilder();
			settingsBuilder = settingsBuilder.WithDefaultSettings();

			var confidentialDesignationSource = new ChoicesValueSource(
				numberOfDifferentPaths: 4,
				maximumPathDepth: 1,
				numberOfDifferentElements: 4,
				maximumElementLength: 200);

			var privilegeDesignationSource = new ChoicesValueSource(
				numberOfDifferentPaths: 100,
				maximumPathDepth: 4,
				numberOfDifferentElements: 250,
				maximumElementLength: 50);

			var dataSourceBuilder = new ImportDataSourceBuilder();
			dataSourceBuilder.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());
			dataSourceBuilder.AddField(WellKnownFields.ConfidentialDesignation, confidentialDesignationSource);
			dataSourceBuilder.AddField(WellKnownFields.PrivilegeDesignation, privilegeDesignationSource);

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
		}

		[CollectDeadlocks]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("9f2f532e-7a16-4199-87ec-1308dbff27a7")]
		public async Task ShouldImportChoicesAndFoldersInParallelAsync(
			[Values(16, 8)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);
			ForceClient(client);

			var settingsBuilder = new NativeImportSettingsBuilder();
			settingsBuilder = settingsBuilder
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			var foldersSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 5,
				numberOfDifferentElements: 100,
				maximumElementLength: 50);

			var confidentialDesignationSource = new ChoicesValueSource(
				numberOfDifferentPaths: 4,
				maximumPathDepth: 1,
				numberOfDifferentElements: 4,
				maximumElementLength: 200);

			var privilegeDesignationSource = new ChoicesValueSource(
				numberOfDifferentPaths: 100,
				maximumPathDepth: 4,
				numberOfDifferentElements: 250,
				maximumElementLength: 50);

			var dataSourceBuilder = new ImportDataSourceBuilder();
			dataSourceBuilder.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());
			dataSourceBuilder.AddField(WellKnownFields.FolderName, foldersSource);
			dataSourceBuilder.AddField(WellKnownFields.ConfidentialDesignation, confidentialDesignationSource);
			dataSourceBuilder.AddField(WellKnownFields.PrivilegeDesignation, privilegeDesignationSource);

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
		}
	}
}