// -----------------------------------------------------------------------------------------------------
// <copyright file="PositiveImportJobTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImportJobTests : NativeImportJobTestBase
	{
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("f808845d-c8c9-454b-9d84-51d84be70bd1")]
		[Test]
		[Pairwise]
		public void ShouldImportTheFiles(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(true, false)] bool disableNativeLocationValidation,
			[Values(true, false)] bool disableNativeValidation)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			ForceClient(client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			this.GivenTheImportJob();
			this.GiveNativeFilePathSourceDocumentImportJob();

			const int NumberOfFilesToImport = 5;
			IEnumerable<DefaultImportDto> importData = DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport);

			// ACT
			this.WhenExecutingTheJob(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfFilesToImport);
			Assert.That(this.TestJobResult.JobMessages, Has.Count.Positive);
			Assert.That(this.TestJobResult.ProgressCompletedRows, Has.Count.EqualTo(NumberOfFilesToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db0852938789")]
		[Test]
		public void ShouldImportFolders()
		{
			// ARRANGE
			ForceClient(TapiClient.Direct);

			this.GivenTheImportJob();
			this.GivenDefaultNativeDocumentImportJob();
			this.ImportJob.Settings.FolderPathSourceFieldName = WellKnownFields.FolderName;

			const int NumberOfDocumentsToImport = 2000;
			var randomFolderGenerator = new RandomFolderGenerator(
				numOfPaths: NumberOfDocumentsToImport,
				maxDepth: 100,
				numOfDifferentFolders: 25,
				numOfDifferentPaths: 100,
				maxFolderLength: 255,
				percentOfSpecial: 15);

			IEnumerable<FolderImportDto> importData = randomFolderGenerator.ToEnumerable();

			// ACT
			this.WhenExecutingTheJob(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(this.TestJobResult.JobMessages, Has.Count.Positive);
			Assert.That(this.TestJobResult.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("700bda86-6e9a-43c1-a69c-2a1972cba4f8")]
		[Test]
		public void ShouldImportDocumentWithChoices()
		{
			// ARRANGE
			this.GivenTheImportJob();
			this.GivenDefaultNativeDocumentImportJob();

			const int NumberOfDocumentsToImport = 2;
			IEnumerable<DocumentWithChoicesImportDto> importData = new[]
			{
				new DocumentWithChoicesImportDto("20", "Highly Confidential", "Attorney Client Communication;Attorney Work Product"),
				new DocumentWithChoicesImportDto("21", "Not Confidential", "Attorney Work Product;Do Whatever You Want"),
			};

			// ACT
			this.WhenExecutingTheJob(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(this.TestJobResult.JobMessages, Has.Count.Positive);
			Assert.That(this.TestJobResult.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[IdentifiedTest("13dc1d17-4a2b-4b48-9015-b61e58bc5168")]
		[Test]
		public void ShouldImportDocumentWithObjects()
		{
			// ARRANGE
			this.GivenTheImportJob();
			this.GivenDefaultNativeDocumentImportJob();

			const int NumberOfDocumentsToImport = 2;
			IEnumerable<DocumentWithObjectsImportDto> importData = new[]
			{
				new DocumentWithObjectsImportDto("1", "Error1", "abc.com;def.org", "ijk.com"),
				new DocumentWithObjectsImportDto("2", "Error2", "def.com", "def.com;lmn.pl"),
			};

			// ACT
			this.WhenExecutingTheJob(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(this.TestJobResult.JobMessages, Has.Count.Positive);
			Assert.That(this.TestJobResult.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
		}
	}
}