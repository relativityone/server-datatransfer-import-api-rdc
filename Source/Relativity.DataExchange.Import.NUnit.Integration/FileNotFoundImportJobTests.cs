// -----------------------------------------------------------------------------------------------------
// <copyright file="FileNotFoundImportJobTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents file not found related import tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System.Collections.Generic;
	using System.Linq;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class FileNotFoundImportJobTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTestCase("9361b3aa-1333-4606-a05c-9b89ef43a2dd", TapiClient.Direct, false, true)]
		[IdentifiedTestCase("b4f90f5b-cf18-4d8a-bc42-259906bd27fd", TapiClient.Direct, true, true)]
		[IdentifiedTestCase("bbee1077-fc95-42aa-a625-2caad809952a", TapiClient.Web, false, true)]
		[IdentifiedTestCase("d1873516-42ad-476f-820d-b790ee3d6c3d", TapiClient.Web, true, true)]
		[IdentifiedTestCase("2d61508b-2622-4b9b-a824-61387796b09b", TapiClient.Aspera, false, true)]
		[IdentifiedTestCase("6d2220f5-4b59-4989-8a21-cca0559894d3", TapiClient.Aspera, true, true)]
		public void ShouldFailWhenTheFileIsNotFound(
			TapiClient client,
			bool disableNativeLocationValidation,
			bool disableNativeValidation)
		{
			// ARRANGE
			ForceClient(client);
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, NativeImportSettingsProvider.NativeFilePathSourceDocumentImportSettings);

			// Intentionally provide an invalid file before adding valid ones.
			const int NumberOfFilesToImport = 5;
			string missingFileName = $@"C:\abcdefghijklmnop\out{client}{disableNativeLocationValidation}{disableNativeValidation}.txt";
			IEnumerable<DefaultImportDto> importData =
				Enumerable.Repeat(new DefaultImportDto(missingFileName), 1)
				.Concat(DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport)).ToArray();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			this.ThenTheImportJobCompletedWithErrors(results, disableNativeLocationValidation ? 0 : 1, NumberOfFilesToImport + 1);
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(NumberOfFilesToImport + 1));
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
			if (disableNativeLocationValidation)
			{
				Assert.That(results.JobMessages, Has.Some.Contains("does not exist"));
			}
		}
	}
}