// ----------------------------------------------------------------------------
// <copyright file="ImportDocumentsWithDifferentModesTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

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

	[Ignore("REL-514167")]
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.Load]
	[TestType.Performance]
	public class ImportDocumentsWithDifferentModesTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		private const string SingleChoiceFieldName = "SingleChoice_SqlComparer";
		private const string MultiChoiceFieldName = "MultiChoice_SqlComparer";

		[CollectDeadlocks]
		[Performance]
		[UseSqlComparer]
		[Category(TestCategories.SqlComparer)]
		[IdentifiedTest("8ddb535f-6a0f-4b2b-b89e-2d16be3f4770")]
		public async Task ShouldImportForSqlComparerAsync(
			[Values(1, 4, 8)] int parallelIApiClientCount,
			[Values(2_000)] int numberOfDocumentsPerIApiClient,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode,
			[Values(TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportStuffForSqlComparerAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, TapiClient.Web, this.TestParameters);

			if (overwriteMode == OverwriteModeEnum.Overlay)
			{
				var settingsBuilderForOverlay = NativeImportSettingsBuilder.New()
					.WithDefaultSettings();

				var dataSourceBuilderForOverlay = ImportDataSourceBuilder.New()
					.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource());

				this.InitializeImportApiWithUserAndPwd(settingsBuilderForOverlay, parallelIApiClientCount);

				await this.JobExecutionContext
												   .ExecuteAsync(dataSourceBuilderForOverlay, numberOfDocumentsPerIApiClient)
												   .ConfigureAwait(false);
			}

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithIdentifierField(WellKnownFields.ControlNumber)
				.WithOverwriteMode(overwriteMode);

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

			var smallSingleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
				new IdentifierValueSource("SMALL-SINGLE"),
				numberOfObjects: 100);
			var mediumSingleObjectsSource = ObjectNameValueSource.CreateForSingleObject(
				new IdentifierValueSource("MEDIUM-SINGLE"),
				numberOfObjects: 100);

			var singleObjectFieldsToCreate = new (int numberOfFields, ObjectNameValueSource valueSource)[]
												 {
													 (5, smallSingleObjectsSource),
													 (2, mediumSingleObjectsSource),
												 };

			IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)> singleObjectFieldsToImport
				= await this.CreateSingleObjectsFieldsAsync(singleObjectFieldsToCreate).ConfigureAwait(false);

			var smallMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
				new IdentifierValueSource("SMALL-MULTI"),
				numberOfObjects: 100,
				maxNumberOfMultiValues: 3);

			var mediumMultiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
				new IdentifierValueSource("MEDIUM-MULTI"),
				numberOfObjects: 5_000,
				maxNumberOfMultiValues: 3);

			var multiObjectFieldsToCreate = new (int numberOfFields, ObjectNameValueSource valueSource)[]
												{
													(5, smallMultiObjectsSource),
													(2, mediumMultiObjectsSource),
												};

			IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)> multiObjectFieldsToImport
				= await this.CreateMultiObjectsFieldsAsync(multiObjectFieldsToCreate).ConfigureAwait(false);

			IEnumerable<string> files = Enumerable.Repeat(
				RandomHelper.NextTextFile(1, 1024, this.TempDirectory.Directory, false), numberOfDocumentsPerIApiClient);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(SingleChoiceFieldName, singleChoicesSource)
				.AddField(MultiChoiceFieldName, multiChoicesSource)
				.AddField(WellKnownFields.FileName, new FileNameValueSource("txt"))
				.AddField(WellKnownFields.ExtractedText, new TextValueSource(10, false));
			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			foreach ((string fieldName, ObjectNameValueSource valuesSource) in singleObjectFieldsToImport.Concat(multiObjectFieldsToImport))
			{
				dataSourceBuilder.AddField(fieldName, valuesSource);
			}

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

		private Task<IEnumerable<(string fieldName, ObjectNameValueSource valuesSource)>> CreateSingleObjectsFieldsAsync(
			(int numberOfFields, ObjectNameValueSource valueSource)[] input)
		{
			return this.CreateObjectsFieldsAsync("Single", input, CreateFieldAsync);

			Task CreateFieldAsync(string fieldName, int objectTypeArtifactId)
			{
				return FieldHelper.CreateSingleObjectFieldAsync(
					this.TestParameters,
					fieldName,
					objectArtifactTypeId: (int)ArtifactType.Document,
					associativeObjectArtifactTypeId: objectTypeArtifactId);
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
					objectArtifactTypeId: objectTypeArtifactId,
					associativeObjectArtifactTypeId: (int)ArtifactType.Document);
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
					string fieldName = $"{fieldNamePrefix}_{i}_{j}";
					int objectTypeArtifactId = await RdoHelper.CreateObjectTypeAsync(this.TestParameters, fieldName).ConfigureAwait(false);
					await createFieldFunctionAsync(fieldName, objectTypeArtifactId).ConfigureAwait(false);

					output.Add((fieldName, valueSource));
				}
			}

			return output;
		}
	}
}