// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportDocumentsWithMoveLoadTests.cs" company="Relativity ODA LLC">
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
	using Relativity.Services.LinkManager.Interfaces;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportDocumentsWithMoveLoadTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		private int folderId;

		[SetUp]
		public async Task SetUp()
		{
			var rootFolderId = await FolderHelper.GetWorkspaceRootArtifactIdAsync(this.TestParameters).ConfigureAwait(false);
			List<int> folderIds = await FolderHelper.CreateFolders(this.TestParameters, new List<string> { "TestFolder" }, rootFolderId).ConfigureAwait(false);

			this.folderId = folderIds.First();
		}

		// There are four cases worth testing:
		// One folder -> One folder
		// One folder -> Many folders
		// Many folders -> One folder
		// Many folders -> Many folders
		// We only need to have load test for One Folder -> One Folder case, because this is the worst case in terms of performance
		[CollectDeadlocks]
		[Performance]
		[UseSqlComparer]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.TransferApi)]
		[Category(TestCategories.LoadTest)]
		[IdentifiedTest("C91E748E-7447-496B-8113-D07F3F9BBB2B")]
		public async Task ShouldImportDocumentsWithMoveFromOneFolderToOneFolderParallelAsync(
			[Values(2, 4, 8, 16)] int parallelIApiClientCount,
			[Values(100_000)] int numberOfDocumentsPerIApiClient,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocumentsWithMoveFromOneFolderToOneFolderParallelAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, client, this.TestParameters);
			ForceClient(client);

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings();

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			await this.JobExecutionContext
				.ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
				.ConfigureAwait(false);

			settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings()
				.WithOverwriteMode(OverwriteModeEnum.Overlay)
				.WithDestinationFolderArtifactId(folderId)
				.WithMove(true);

			dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());

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
	}
}