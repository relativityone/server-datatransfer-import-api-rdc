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
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	[Feature.DataTransfer.TransferApi]
	[TestType.Error]
	[TestExecutionCategory.CI]
	public class FileNotFoundImportJobTests : ImportJobTestBase<NativeImportExecutionContext>
	{
		private bool _originalTapiFileNotFoundErrorsRetry;

		[SetUp]
		public void SetUp()
		{
			this._originalTapiFileNotFoundErrorsRetry = AppSettings.Instance.TapiFileNotFoundErrorsRetry;
		}

		[TearDown]
		public async Task TearDownAsync()
		{
			AppSettings.Instance.TapiFileNotFoundErrorsRetry = this._originalTapiFileNotFoundErrorsRetry;
			await RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document).ConfigureAwait(false);
		}

		[IdentifiedTestCase("9361b3aa-1333-4606-a05c-9b89ef43a2dd", TapiClient.Direct, false, true)]
		[IdentifiedTestCase("b4f90f5b-cf18-4d8a-bc42-259906bd27fd", TapiClient.Direct, true, true)]
		[IdentifiedTestCase("bbee1077-fc95-42aa-a625-2caad809952a", TapiClient.Web, false, true)]
		[IdentifiedTestCase(
			"d1873516-42ad-476f-820d-b790ee3d6c3d",
			TapiClient.Web,
			true,
			true,
			IgnoreReason = "REL-910396 - Behavior has changed for the web mode, but neither RIP nor Processing uses it. IAPI is deprecated for external clients.")]
		[IdentifiedTestCase("2d61508b-2622-4b9b-a824-61387796b09b", TapiClient.Aspera, false, true, IgnoreReason = "Aspera is deprecated.")]
		[IdentifiedTestCase("6d2220f5-4b59-4989-8a21-cca0559894d3", TapiClient.Aspera, true, true, IgnoreReason = "Aspera is deprecated.")]
		public void ShouldFailWhenTheFirstFileIsNotFound(
			TapiClient client,
			bool disableNativeLocationValidation,
			bool disableNativeValidation)
		{
			// ARRANGE
			if (disableNativeLocationValidation)
			{
				AppSettings.Instance.TapiFileNotFoundErrorsRetry = false; // it does not make sense to retry because file does not exist
			}

			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, NativeImportSettingsProvider.GetFileCopySettings((int)ArtifactType.Document));

			// Intentionally provide an invalid file before adding valid ones.
			const int NumberOfFilesToImport = 5;
			string missingFileName = $@"out{client}{disableNativeLocationValidation}{disableNativeValidation}.txt";
			string missingFilePath = $@"C:\abcdefghijklmnop\{missingFileName}";
			IEnumerable<DefaultImportDto> importData =
				Enumerable.Repeat(new DefaultImportDto(controlNumber: missingFileName, filePath: missingFilePath), 1)
				.Concat(DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport)).ToArray();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			// REL-910396 - item error is reported even if validation is disable
			this.ThenTheImportJobCompletedWithErrors(results, 1, NumberOfFilesToImport + 1);

			Assert.That(results.ErrorRows, Has.Count.EqualTo(1), "It should report one error, because one file was missing");
			var error = results.ErrorRows.Single();
			Assert.That(error["Line Number"], Is.EqualTo(1), "Error was in the first line");
			Assert.That(error["Identifier"], Is.EqualTo(missingFileName), "Identifier should be set correctly");

			int expectedNumberOfCompletedRows = disableNativeValidation && disableNativeLocationValidation ? NumberOfFilesToImport : NumberOfFilesToImport + 1;
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfCompletedRows));
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
			if (disableNativeLocationValidation)
			{
				Assert.That(results.JobMessages, Has.Some.Contains("Unable to find the file"));
			}
		}

		[TestCase(TapiClient.Direct, false, true)]
		[TestCase(TapiClient.Direct, true, true)]
		public void ShouldFailWhenTheLastFileIsNotFound(
			TapiClient client,
			bool disableNativeLocationValidation,
			bool disableNativeValidation)
		{
			// ARRANGE
			if (disableNativeLocationValidation)
			{
				AppSettings.Instance.TapiFileNotFoundErrorsRetry = false; // it does not make sense to retry because file does not exist
			}

			kCura.WinEDDS.Config.ConfigSettings["DisableNativeLocationValidation"] = disableNativeLocationValidation;
			kCura.WinEDDS.Config.ConfigSettings["DisableNativeValidation"] = disableNativeValidation;

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, NativeImportSettingsProvider.GetFileCopySettings((int)ArtifactType.Document));

			// Intentionally provide an invalid file before adding valid ones.
			const int NumberOfFilesToImport = 5;
			string missingFileName = $@"out{client}{disableNativeLocationValidation}{disableNativeValidation}.txt";
			string missingFilePath = $@"C:\abcdefghijklmnop\{missingFileName}";
			IEnumerable<DefaultImportDto> importData =
				DefaultImportDto.GetRandomTextFiles(this.TempDirectory.Directory, NumberOfFilesToImport)
				.Concat(Enumerable.Repeat(new DefaultImportDto(controlNumber: missingFileName, filePath: missingFilePath), 1)).ToArray();

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			// REL-910396 - item error is reported even if validation is disable
			this.ThenTheImportJobCompletedWithErrors(results, 1, NumberOfFilesToImport + 1);

			Assert.That(results.ErrorRows, Has.Count.EqualTo(1), "It should report one error, because one file was missing");
			var error = results.ErrorRows.Single();
			Assert.That(error["Line Number"], Is.EqualTo(6), "Error was in the last line");
			Assert.That(error["Identifier"], Is.EqualTo(missingFileName), "Identifier should be set correctly");

			int expectedNumberOfCompletedRows = disableNativeValidation && disableNativeLocationValidation ? NumberOfFilesToImport : NumberOfFilesToImport + 1;
			Assert.That(results.NumberOfCompletedRows, Is.EqualTo(expectedNumberOfCompletedRows));
			Assert.That(results.NumberOfJobMessages, Is.GreaterThan(0));
			ThenTheJobCompletedInCorrectTransferMode(results, client);
			if (disableNativeLocationValidation)
			{
				Assert.That(results.JobMessages, Has.Some.Contains("Unable to find the file"));
			}
		}
	}
}