// -----------------------------------------------------------------------------------------------------
// <copyright file="ImportDocumentPerformanceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents document import job performance tests.
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
	using Relativity.DataExchange.TestFramework.PerformanceTests;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[TestType.Performance]
	[TestExecutionCategory.CI]
	[AutomatedPerformanceTest]
	[Ignore("This test Should be a part of performance test pipelines - execution takes around 2 hours")]
	public class ImportDocumentPerformanceTests : ImportLoadTestsBase<NativeImportExecutionContext, Settings>
	{
		[IdentifiedTest("KW4F5F69-AA3F-4BDA-93E8-6F22CAW4534")]
		public async Task GoldenFlowPerformanceTest()
		{
			// Arrange
			const double ExpectedSqlProcRateBaseline = 15.0;

			// For now this test is taking more than 2 hours
			const int ParallelIApiClientCount = 8;
			const int NumberOfDocumentsPerIApiClient = 50_000;

			PerformanceDataCollector.Instance.SetPerformanceTestValues("ImportDocumentPerformanceTests", ParallelIApiClientCount, NumberOfDocumentsPerIApiClient, 0, 0, TapiClient.Web, this.TestParameters);

			var settingsBuilder = NativeImportSettingsBuilder.New()
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			FoldersValueSource foldersValueSource = new FoldersValueSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 10,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var dataSourceBuilder = ImportDataSourceBuilder.New()
				.AddField(WellKnownFields.ControlNumber, new IdentifierValueSource()).AddField(
					WellKnownFields.FolderName,
					foldersValueSource);

			var fieldsToImport = await ComplexFieldBuilder.Create(this.TestParameters)

									 // Single Object fields
									 .WithSmallSingleObjectField()
									 .WithMediumSingleObjectField()
									 .WithLargeSingleObjectField()

									 // Multiple Object fields
									 .WithSmallMultiObjectsField()
									 .WithMediumMultiObjectsField()
									 .WithLargeMultiObjectsField()

									 // Single Choice Fields
									 .WithSmallSingleChoiceField()
									 .WithMediumSingleChoiceField()
									 .WithLargeSingleChoiceField()

									 // Multi Choice Fields
									 .WithSmallMultiChoiceField()
									 .WithMediumMultiChoiceField()
									 .WithLargeMultiChoiceField()
									 .Build()
									 .ConfigureAwait(false);

			foreach ((string fieldName, IFieldValueSource valuesSource) in fieldsToImport)
			{
				dataSourceBuilder.AddField(fieldName, valuesSource);
			}

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, ParallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, NumberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfRows = NumberOfDocumentsPerIApiClient * ParallelIApiClientCount;
			this.ThenTheImportJobIsSuccessful(results, expectedNumberOfRows);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfRows));

			// validate performance
			Assert.That(results.SqlProcessRate, Is.GreaterThan(ExpectedSqlProcRateBaseline), $"Actual SqlProcessRate is {results.SqlProcessRate} and is lower than baseline value {ExpectedSqlProcRateBaseline}");
		}
	}
}
