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
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db08529387AB")]
		[Test]
		[TestCase(8)]
		public void ShouldImportFoldersParallel(int parallelIApiClientCount)
		{
			const int NumberOfDocumentsToImport = 800000;
			var randomFolderGenerator = new RandomFolderGenerator(
				numOfPaths: NumberOfDocumentsToImport,
				maxDepth: 100,
				numOfDifferentFolders: 1000,
				numOfDifferentPaths: 500,
				maxFolderLength: 255,
				percentOfSpecial: 15);

			// ARRANGE
			ForceClient(TapiClient.Direct);

			this.GivenTheImportJobs(parallelIApiClientCount);
			foreach (var keyVal in this.ImportAPIInstancesDict)
			{
				this.GivenDefaultNativeDocumentImportJob(keyVal.Key, keyVal.Value);
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