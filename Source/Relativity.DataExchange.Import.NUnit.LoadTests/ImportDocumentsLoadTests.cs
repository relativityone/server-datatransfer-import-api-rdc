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

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.Import.NUnit.LoadTests.SetUp;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportDocumentsLoadTests : ImportJobTestBase<ImportBulkArtifactJob, Settings>
	{
		public ImportDocumentsLoadTests()
			: base(new ParallelNativeImportApiSetUp())
		{
		}

		[TestFramework.NUnitExtensions.CollectDeadlocks]
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("b9b6897f-ea3f-4694-80d2-db08529387AB", 8, 400000)]
		[IdentifiedTestCase("68322B14-8BFA-49D2-9B00-6501DBAA2452", 16, 800000)]
		public void ShouldImportFoldersParallel(int parallelIApiClientCount, int numberOfDocumentsToImport)
		{
			// ARRANGE
			var randomFolderGenerator = RandomPathGenerator.GetFolderGenerator(
				numOfDifferentElements: 100,
				maxElementLength: 100,
				numOfDifferentPaths: 1000,
				maxPathDepth: 10);

			ForceClient(TapiClient.Direct);

			Settings settings = NativeImportSettingsProvider.GetDefaultNativeDocumentImportSettings();
			settings.FolderPathSourceFieldName = WellKnownFields.FolderName;
			this.InitializeImportApiWithUserAndPwd(settings, parallelIApiClientCount, numberOfDocumentsToImport);

			IEnumerable<FolderImportDto> importData = randomFolderGenerator
				.ToFolders(numberOfDocumentsToImport)
				.Select((p, i) => new FolderImportDto(i.ToString(), p));

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			this.ValidateTotalRowsCount(numberOfDocumentsToImport);
			this.ValidateFatalExceptionsNotExist();
			this.ValidateErrorRowsNotExist(results);
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(numberOfDocumentsToImport));
		}

		protected override void ValidateTotalRowsCount(int expectedTotalRows)
		{
			int totalDocCount = this.ImportApiSetUp<ParallelNativeImportApiSetUp>().GetCompletedTotalDocCountFromReport();

			Assert.That(totalDocCount, Is.EqualTo(expectedTotalRows));
		}

		protected override void ValidateFatalExceptionsNotExist()
		{
			List<Exception> fatalExceptions =
				this.ImportApiSetUp<ParallelNativeImportApiSetUp>().GetFatalExceptionsFromReport();

			Assert.That(fatalExceptions.All(item => item == null));
			Assert.That(this.ImportApiSetUp<ParallelNativeImportApiSetUp>().TestJobResult.JobFatalExceptions, Has.Count.Zero);
		}

		private void ValidateErrorRowsNotExist(ImportTestJobResult jobResult)
		{
			IEnumerable<JobReport.RowError> errorRows = this.ImportApiSetUp<ParallelNativeImportApiSetUp>().GetErrorRowsFromReport();

			Assert.That(!errorRows.Any());
			Assert.That(jobResult.ErrorRows, Has.Count.Zero);
		}

		private void InitializeImportApiWithUserAndPwd(Settings settings, int instanceCount, int documentCount)
		{
			this.ImportApiSetUp<ParallelNativeImportApiSetUp>()
				.ConfigureImportApiInstanceAndDocCounts(instanceCount, documentCount)
				.SetUpImportApi(this.CreateImportApiWithUserAndPwd, settings);
		}
	}
}