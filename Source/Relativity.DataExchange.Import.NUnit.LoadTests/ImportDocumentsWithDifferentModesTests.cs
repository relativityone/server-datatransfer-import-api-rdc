// ----------------------------------------------------------------------------
// <copyright file="ImportDocumentsWithDifferentModesTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
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

	[TestFixture]
	public class ImportDocumentsWithDifferentModesTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		private const string SingleChoiceFieldName = "SingleChoice_SqlComparer";
		private const string MultiChoiceFieldName = "MultiChoice_SqlComparer";
		private const string SingleObjectFieldName = "SingleObject_SqlComparer";
		private const string MultiObjectFieldName = "MultiObject_SqlComparer";
		private int singleObjectTypeArtifactId;
		private int multiObjectTypeArtifactId;

		[UseSqlComparer]
		[Category(TestCategories.SqlComparer)]
		[IdentifiedTest("8ddb535f-6a0f-4b2b-b89e-2d16be3f4770")]
		public async Task ShouldImportForSqlComparerAsync(
			[Values(1, 2, 4, 16)] int parallelIApiClientCount,
			[Values(2000)] int numberOfDocumentsPerIApiClient,
			[Values(OverwriteModeEnum.Append, OverwriteModeEnum.Overlay, OverwriteModeEnum.AppendOverlay)] OverwriteModeEnum overwriteMode)
		{
			// ARRANGE
			PerformanceDataCollector.Instance.SetPerformanceTestValues("ShouldImportStuffForSqlComparerAsync", parallelIApiClientCount, numberOfDocumentsPerIApiClient, 0, 0, TapiClient.Web, this.TestParameters);
			ForceClient(TapiClient.Web);

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

			await Task.WhenAll(
				RdoHelper.CreateObjectTypeAsync(this.TestParameters, SingleObjectFieldName).ContinueWith(task => this.singleObjectTypeArtifactId = task.Result),
				RdoHelper.CreateObjectTypeAsync(this.TestParameters, MultiObjectFieldName).ContinueWith(task => this.multiObjectTypeArtifactId = task.Result))
				.ConfigureAwait(false);

			await Task.WhenAll(
				FieldHelper.CreateSingleObjectFieldAsync(
					this.TestParameters,
					SingleObjectFieldName,
					this.singleObjectTypeArtifactId,
					(int)ArtifactType.Document),
				FieldHelper.CreateMultiObjectFieldAsync(
					this.TestParameters,
					MultiObjectFieldName,
					this.multiObjectTypeArtifactId,
					(int)ArtifactType.Document),
				this.CreateChoiceFieldsAsync())
				.ConfigureAwait(false);

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
				new IdentifierValueSource("SMALL-SINGLE"),
				numberOfObjects: 100);

			var multiObjectsSource = ObjectNameValueSource.CreateForMultiObjects(
				new IdentifierValueSource("SMALL-MULTI"),
				numberOfObjects: 100,
				maxNumberOfMultiValues: 3);

			IEnumerable<string> files = Enumerable.Repeat(
				RandomHelper.NextTextFile(1, 1024, this.TempDirectory.Directory, false), numberOfDocumentsPerIApiClient);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource())
				.AddField(SingleChoiceFieldName, singleChoicesSource)
				.AddField(MultiChoiceFieldName, multiChoicesSource)
				.AddField(SingleObjectFieldName, singleObjectsSource)
				.AddField(MultiObjectFieldName, multiObjectsSource)
				.AddField(WellKnownFields.FileName, new FileNameValueSource("txt"))
				.AddField(WellKnownFields.ExtractedText, new TextValueSource(10, false));
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
			ThenTheJobCompletedInCorrectTransferMode(results, TapiClient.Web);
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