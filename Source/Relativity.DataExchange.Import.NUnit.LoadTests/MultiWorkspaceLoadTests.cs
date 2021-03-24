// <copyright file="MultiWorkspaceLoadTests.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>
namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport.FieldValueSources;
	using Relativity.DataExchange.TestFramework.PerformanceTests;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	public class MultiWorkspaceLoadTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		private const string SmallSingleObjectFieldName = "Small-SingleObject_MulitWorkspace";
		private const string MediumSingleObjectFieldName = "Medium-SingleObject_MulitWorkspace";
		private const string SmallMultiObjectFieldName = "Small-MultiObject_MulitWorkspace";
		private const string MediumMultiObjectFieldName = "Medium-MultiObject_MulitWorkspace";

		private const string SingleChoiceFieldName = "SingleChoice_MulitWorkspace";
		private const string MultiChoiceFieldName = "MultiChoice_MulitWorkspace";

		[Category(TestCategories.LoadTest)]
		[IdentifiedTest("8ddb535f-6a0f-4b2b-b89e-2d16be3f4770")]
		public async Task ShouldImportDocumentToDifferentWorkspaces(
			[Values(2, 4, 8)] int parallelIApiClientCount,
			[Values(50_000)] int numberOfDocumentsPerIApiClient,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.AppendOverlay, OverwriteModeEnum.Overlay)] OverwriteModeEnum overwriteMode,
			[Values(TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportDocsForMultiWorkspaceConfiguration", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, TapiClient.Web, this.TestParameters);

			using (var testScope = new TestScope(TestParameters, parallelIApiClientCount))
			{
				var dataSourceBuilder = await testScope.ConfigureAsync().ConfigureAwait(false);

				await this.PerformOverlayModeInitialization(parallelIApiClientCount, numberOfDocumentsPerIApiClient, overwriteMode, testScope).ConfigureAwait(false);

				var settingsBuilder = NativeImportSettingsBuilder.New()
					.WithIdentifierField(WellKnownFields.ControlNumber).WithOverwriteMode(overwriteMode);

				this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount, testScope.WorkspaceIds);

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

		private async Task PerformOverlayModeInitialization(
			int parallelIApiClientCount,
			int numberOfDocumentsPerIApiClient,
			OverwriteModeEnum overwriteMode,
			TestScope testScope)
		{
			if (overwriteMode == OverwriteModeEnum.Overlay)
			{
				var settingsBuilderForOverlay = NativeImportSettingsBuilder.New().WithDefaultSettings();

				var dataSourceBuilderForOverlay = ImportDataSourceBuilder.New().AddField(
					WellKnownFields.ControlNumber,
					new IdentifierValueSource());

				this.InitializeImportApiWithUserAndPwd(settingsBuilderForOverlay, parallelIApiClientCount, testScope.WorkspaceIds);

				await this.JobExecutionContext
					.ExecuteAsync(dataSourceBuilderForOverlay, numberOfDocumentsPerIApiClient)
					.ConfigureAwait(false);
			}
		}

		private class TestScope : IDisposable
		{
			private readonly IntegrationTestParameters testParameters;

			private readonly int workspaceCount;

			public TestScope(IntegrationTestParameters testParameters, int workspaceCount)
			{
				if (workspaceCount <= 0)
				{
					throw new ArgumentException($"{nameof(workspaceCount)} testParameters should be positive number");
				}

				this.testParameters = testParameters;
				this.workspaceCount = workspaceCount;
				this.WorkspaceIds = new List<int>();
			}

			public List<int> WorkspaceIds { get; }

			public async Task<ImportDataSourceBuilder> ConfigureAsync()
			{
				ImportDataSourceBuilder dataSourceBuilder = null;

				for (int index = 0; index < this.workspaceCount; ++index)
				{
					await this.PrepareFieldsAsync(index).ConfigureAwait(false);

					dataSourceBuilder = CreateDataSourceBuilderAsync();
				}

				return dataSourceBuilder;
			}

			public void Dispose()
			{
				this.WorkspaceIds.ForEach(wkspId => IntegrationTestHelper.DeleteTestWorkspace(testParameters, wkspId));
			}

			private static ImportDataSourceBuilder CreateDataSourceBuilderAsync()
			{
				var foldersSource = new FoldersValueSource(
					numberOfDifferentPaths: 1000,
					maximumPathDepth: 5,
					numberOfDifferentElements: 100,
					maximumElementLength: 50);

				var smallSingleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
					new IdentifierValueSource("SMALL-SINGLE-OBJ"),
					numberOfObjects: 100);
				var mediumSingleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
					new IdentifierValueSource("MEDIUM-SINGLE-OBJ"),
					numberOfObjects: 1000);

				var smallMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
					new IdentifierValueSource("SMALL-MULTI-OBJ"),
					numberOfObjects: 1000,
					maxNumberOfMultiValues: 10);

				var mediumMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
					new IdentifierValueSource("MEDIUM-MULTI-OBJ"),
					numberOfObjects: 5_000,
					maxNumberOfMultiValues: 3);

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

				return ImportDataSourceBuilder.New()
					.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
					.AddField(WellKnownFields.FolderName, foldersSource)
					.AddField(SmallSingleObjectFieldName, smallSingleObjectsSource)
					.AddField(MediumSingleObjectFieldName, mediumSingleObjectsSource)
					.AddField(SmallMultiObjectFieldName, smallMultiObjectsSource)
					.AddField(MediumMultiObjectFieldName, mediumMultiObjectsSource)
					.AddField(SingleChoiceFieldName, singleChoicesSource).AddField(MultiChoiceFieldName, multiChoicesSource)
					.AddField(WellKnownFields.FileName, new FileNameValueSource("txt")).AddField(
						WellKnownFields.ExtractedText,
						new TextValueSource(10, false));
			}

			private async Task PrepareFieldsAsync(int index)
			{
				this.WorkspaceIds.Add(WorkspaceHelper.CreateTestWorkspace(this.testParameters, IntegrationTestHelper.Logger).workspaceId);

				int smallSingleObjectTypeArtifactId = await RdoHelper
														  .CreateObjectTypeAsync(
															  this.testParameters,
															  SmallSingleObjectFieldName,
															  this.WorkspaceIds[index]).ConfigureAwait(false);
				int mediumSingleObjectTypeArtifactId = await RdoHelper
														   .CreateObjectTypeAsync(
															   this.testParameters,
															   MediumSingleObjectFieldName,
															   this.WorkspaceIds[index]).ConfigureAwait(false);
				int smallMultiObjectTypeArtifactId = await RdoHelper
														 .CreateObjectTypeAsync(
															 this.testParameters,
															 SmallMultiObjectFieldName,
															 this.WorkspaceIds[index]).ConfigureAwait(false);

				int mediumMultiObjectTypeArtifactId = await RdoHelper
														  .CreateObjectTypeAsync(
															  this.testParameters,
															  MediumMultiObjectFieldName,
															  this.WorkspaceIds[index]).ConfigureAwait(false);

				await Task.WhenAll(
					FieldHelper.CreateSingleObjectFieldAsync(
						this.testParameters,
						SmallSingleObjectFieldName,
						smallSingleObjectTypeArtifactId,
						(int)ArtifactType.Document,
						this.WorkspaceIds[index]),
					FieldHelper.CreateSingleObjectFieldAsync(
						this.testParameters,
						MediumSingleObjectFieldName,
						mediumSingleObjectTypeArtifactId,
						(int)ArtifactType.Document,
						this.WorkspaceIds[index]),
					FieldHelper.CreateMultiObjectFieldAsync(
						this.testParameters,
						SmallMultiObjectFieldName,
						smallMultiObjectTypeArtifactId,
						(int)ArtifactType.Document,
						this.WorkspaceIds[index]),
					FieldHelper.CreateMultiObjectFieldAsync(
						this.testParameters,
						MediumMultiObjectFieldName,
						mediumMultiObjectTypeArtifactId,
						(int)ArtifactType.Document,
						this.WorkspaceIds[index]),
					FieldHelper.CreateSingleChoiceFieldAsync(
						this.testParameters,
						SingleChoiceFieldName,
						(int)ArtifactType.Document,
						false,
						this.WorkspaceIds[index]),
					FieldHelper.CreateMultiChoiceFieldAsync(
						this.testParameters,
						MultiChoiceFieldName,
						(int)ArtifactType.Document,
						false,
						this.WorkspaceIds[index])).ConfigureAwait(false);
			}
		}
	}
}
