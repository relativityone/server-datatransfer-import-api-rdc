﻿// -----------------------------------------------------------------------------------------------------
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
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("b9b6897f-ea3f-4694-80d2-db0852938789")]
		[Test]
		public void ShouldImportFolders()
		{
			ForceClient(TapiClient.Direct);

			this.GivenTheImportJob();
			this.GivenDefaultNativeDocumentImportJob();
			this.ImportJob.Settings.FolderPathSourceFieldName = WellKnownFields.FolderName;

			const int NumberOfDocumentsToImport = 2000;
			IEnumerable<FolderImportDto> importData = FolderImportDto.GetRandomFolders(NumberOfDocumentsToImport, 100, 25, 100, 255, 15);

			// ACT
			this.WhenExecutingTheJob(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(NumberOfDocumentsToImport);
			Assert.That(this.TestJobResult.JobMessages, Has.Count.Positive);
			Assert.That(this.TestJobResult.ProgressCompletedRows, Has.Count.EqualTo(NumberOfDocumentsToImport));
		}
	}
}