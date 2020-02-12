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
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Import.NUnit.Integration.JobExecutionContext;
	using Relativity.DataExchange.Import.NUnit.LoadTests.JobExecutionContext;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.ImportDataSource;
	using Relativity.DataExchange.TestFramework.ImportDataSource.FieldValueSources;
	using Relativity.DataExchange.TestFramework.NUnitExtensions;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[Explicit]
	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportDocumentsLoadTests : ImportJobTestBase<ParallelNativeImportExecutionContext>
	{
		[TearDown]
		public Task TearDownAsync()
		{
			return this.ResetContextAsync();
		}

		[CollectDeadlocks]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("b9b6897f-ea3f-4694-80d2-db08529387AB", 8, 50 * 1000)]
		[IdentifiedTestCase("68322B14-8BFA-49D2-9B00-6501DBAA2452", 16, 100 * 1000)]
		public async Task ShouldImportFoldersParallelAsync(int parallelIApiClientCount, int numberOfDocumentsPerIApiClient)
		{
			// ARRANGE
			ForceClient(TapiClient.Direct);

			var settingsBuilder = new NativeImportSettingsBuilder();
			settingsBuilder = settingsBuilder
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			FoldersSource foldersSource = new FoldersSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 10,
				numberOfDifferentElements: 100,
				maximumElementLength: 100);

			var dataSourceBuilder = new ImportDataSourceBuilder();
			dataSourceBuilder.AddField(WellKnownFields.ControlNumber, new IdentifierSource());
			dataSourceBuilder.AddField(WellKnownFields.FolderName, foldersSource);

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfRows = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ValidateTotalRowsCount(expectedNumberOfRows);
			this.ValidateFatalExceptionsNotExist();
			this.ValidateErrorRowsNotExist(results);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfRows));
		}

		[CollectDeadlocks]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("cb760ab7-2b37-4bf0-afbf-67267744910c", 8, 250 * 1000)]
		[IdentifiedTestCase("cb760ab7-2b37-4bf0-afbf-67267744910c", 16, 100 * 1000)]
		public async Task ShouldImportChoicesInParallelAsync(int parallelIApiClientCount, int numberOfDocumentsPerIApiClient)
		{
			// ARRANGE
			ForceClient(TapiClient.Direct);

			var settingsBuilder = new NativeImportSettingsBuilder();
			settingsBuilder = settingsBuilder.WithDefaultSettings();

			var settings = settingsBuilder.Build();
			char multiValueDelimiter = settings.MultiValueDelimiter;
			char nestedValueDelimiter = settings.NestedValueDelimiter;

			var confidentialDesignationSource = new ChoicesSource(
				numberOfDifferentPaths: 4,
				maximumPathDepth: 1,
				numberOfDifferentElements: 4,
				maximumElementLength: 200,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			var privilegeDesignationSource = new ChoicesSource(
				numberOfDifferentPaths: 100,
				maximumPathDepth: 4,
				numberOfDifferentElements: 250,
				maximumElementLength: 50,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			var dataSourceBuilder = new ImportDataSourceBuilder();
			dataSourceBuilder.AddField(WellKnownFields.ControlNumber, new IdentifierSource());
			dataSourceBuilder.AddField(WellKnownFields.ConfidentialDesignation, confidentialDesignationSource);
			dataSourceBuilder.AddField(WellKnownFields.PrivilegeDesignation, privilegeDesignationSource);

			this.InitializeImportApiWithUserAndPwd(settingsBuilder, parallelIApiClientCount);

			// ACT
			ImportTestJobResult results = await this.JobExecutionContext
											  .ExecuteAsync(dataSourceBuilder, numberOfDocumentsPerIApiClient)
											  .ConfigureAwait(false);

			// ASSERT
			int expectedNumberOfDocuments = numberOfDocumentsPerIApiClient * parallelIApiClientCount;
			this.ValidateTotalRowsCount(expectedNumberOfDocuments);
			this.ValidateFatalExceptionsNotExist();
			this.ValidateErrorRowsNotExist(results);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfDocuments));
		}

		[CollectDeadlocks]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("cb760ab7-2b37-4bf0-afbf-67267744910c", 8, 500 * 1000)]
		public async Task ShouldImportChoicesAndFoldersInParallelAsync(int parallelIApiClientCount, int numberOfDocumentsPerIApiClient)
		{
			// ARRANGE
			ForceClient(TapiClient.Direct);

			var settingsBuilder = new NativeImportSettingsBuilder();
			settingsBuilder = settingsBuilder
				.WithDefaultSettings()
				.WithFolderPath(WellKnownFields.FolderName);

			var settings = settingsBuilder.Build();
			char multiValueDelimiter = settings.MultiValueDelimiter;
			char nestedValueDelimiter = settings.NestedValueDelimiter;

			var foldersSource = new FoldersSource(
				numberOfDifferentPaths: 1000,
				maximumPathDepth: 5,
				numberOfDifferentElements: 100,
				maximumElementLength: 50);

			var confidentialDesignationSource = new ChoicesSource(
				numberOfDifferentPaths: 4,
				maximumPathDepth: 1,
				numberOfDifferentElements: 4,
				maximumElementLength: 200,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			var privilegeDesignationSource = new ChoicesSource(
				numberOfDifferentPaths: 100,
				maximumPathDepth: 4,
				numberOfDifferentElements: 250,
				maximumElementLength: 50,
				multiValueDelimiter: multiValueDelimiter,
				nestedValueDelimiter: nestedValueDelimiter);

			var dataSourceBuilder = new ImportDataSourceBuilder();
			dataSourceBuilder.AddField(WellKnownFields.ControlNumber, new IdentifierSource());
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
			this.ValidateTotalRowsCount(expectedNumberOfDocuments);
			this.ValidateFatalExceptionsNotExist();
			this.ValidateErrorRowsNotExist(results);
			Assert.That(results.NumberOfJobMessages, Is.Positive);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfDocuments));
		}

		private void ValidateTotalRowsCount(int expectedTotalRows)
		{
			int totalDocCount = this.JobExecutionContext.CompletedTotalRowsCountFromReport;

			Assert.That(totalDocCount, Is.EqualTo(expectedTotalRows));
		}

		private void ValidateFatalExceptionsNotExist()
		{
			IEnumerable<Exception> fatalExceptions =
				this.JobExecutionContext.FatalExceptionsFromReport;

			Assert.That(fatalExceptions.All(item => item == null));
			Assert.That(this.JobExecutionContext.TestJobResult.JobFatalExceptions, Has.Count.Zero);
		}

		private void ValidateErrorRowsNotExist(ImportTestJobResult jobResult)
		{
			int numberOfErrorRows = this.JobExecutionContext.ErrorRowsCountFromReport;

			Assert.That(numberOfErrorRows, Is.Zero);
			Assert.That(jobResult.ErrorRows, Has.Count.Zero);
		}

		private void InitializeImportApiWithUserAndPwd(ISettingsBuilder<Settings> settingsBuilder, int instanceCount)
		{
			this.JobExecutionContext
				.ConfigureImportApiInstanceCount(instanceCount)
				.InitializeImportApiWithUserAndPassword(this.TestParameters, settingsBuilder);
		}
	}
}