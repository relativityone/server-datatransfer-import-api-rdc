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
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class ImportDocumentsLoadTests : NativeImportJobTestBase
	{
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("b9b6897f-ea3f-4694-80d2-db08529387AB", 16, 800000)]
		[IdentifiedTestCase("68322B14-8BFA-49D2-9B00-6501DBAA2452", 8, 400000)]
		public void ShouldImportFoldersParallel(int parallelIApiClientCount, int numberOfDocumentsToImport)
		{
			var randomFolderGenerator = new RandomFolderGenerator(
				numOfPaths: numberOfDocumentsToImport,
				maxDepth: 10,
				numOfDifferentFolders: 100,
				numOfDifferentPaths: 1000,
				maxFolderLength: 100,
				percentOfSpecial: 15);

			// ARRANGE
			ForceClient(TapiClient.Direct);

			this.GivenTheImportJobs(parallelIApiClientCount);
			foreach (var keyVal in this.ImportAPIInstancesDict)
			{
				this.GetDefaultNativeDocumentImportSettings(keyVal.Key, keyVal.Value);
			}

			IEnumerable<FolderImportDto> importData = randomFolderGenerator.ToEnumerable();

			// ACT
			int index = 0;
			List<Task> tasks = new List<Task>();
			foreach (var iapiJobs in this.ImportJobsDict.Values)
			{
				iapiJobs.Settings.FolderPathSourceFieldName = WellKnownFields.FolderName;

				var dataReader = importData.Skip(index * (NumberOfDocumentsToImport / this.ImportJobsDict.Count))
					.Take(NumberOfDocumentsToImport / this.ImportJobsDict.Count);

				tasks.Add(Task.Run(() => this.WhenExecutingTheJob(dataReader, iapiJobs)));

				++index;
			}

			Task.WaitAll(tasks.ToArray());

			// ASSERT
			Assert.That(this.TestJobResult.ErrorRows, Has.Count.Zero);
			Assert.That(this.TestJobResult.JobFatalExceptions, Has.Count.Zero);

			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(this.TestJobResult.JobMessages, Has.Count.Positive);
			Assert.That(this.TestJobResult.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
		}
	}
}