// -----------------------------------------------------------------------------------------------------
// <copyright file="PositiveImageImportJobTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents positive import job tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.Integration
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.Transfer;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImageImportJobTests : ImportJobTestBase<ImageImportBulkArtifactJob, ImageSettings>
	{
		public PositiveImageImportJobTests()
			: base(new ImageImportApiSetUp())
		{
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("9db2e7f4-0bc8-46a8-9e95-621ca9bcc5c1")]
		[Pairwise]
		public void ShouldImportTheFilesTransferModes(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			ForceClient(client);

			this.InitializeImportApiWithUserAndPassword(ImageImportSettingsProvider.GetImageFilePathSourceDocumentImportSettings());

			const int NumberOfDocumentsToImport = 5;
			const int NumberOfImagesPerDocument = 3;
			IEnumerable<ImageImportDto> importData = GetRandomImageFiles(this.TempDirectory.Directory, NumberOfDocumentsToImport, NumberOfImagesPerDocument, ImageFormat.Jpeg);

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(ExpectedNumberOfImportedImages);
			Assert.That(results.JobMessages, Has.Count.Positive);

			// test result job returns ProgressCompletedRows larger by 1 than real files count
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(ExpectedNumberOfImportedImages + 1));
		}

		[Category(TestCategories.ImportDoc)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("8a855ae4-93e6-4340-bfb2-59886cef953d")]
		[Pairwise]
		public void ShouldImportTheFilesConfiguration(
			[Values(true, false)] bool disableImageLocationValidation,
			[Values(true, false)] bool disableImageTypeValidation,
			[Values(ImageFormat.Jpeg, ImageFormat.Tiff)] ImageFormat imageFormat)
		{
			// ARRANGE
			AppSettings.Instance.DisableImageLocationValidation = disableImageLocationValidation;
			AppSettings.Instance.DisableImageTypeValidation = disableImageTypeValidation;

			this.InitializeImportApiWithUserAndPassword(ImageImportSettingsProvider.GetImageFilePathSourceDocumentImportSettings());

			const int NumberOfDocumentsToImport = 5;
			const int NumberOfImagesPerDocument = 3;
			IEnumerable<ImageImportDto> importData = GetRandomImageFiles(this.TempDirectory.Directory, NumberOfDocumentsToImport, NumberOfImagesPerDocument, imageFormat);

			// ACT
			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(ExpectedNumberOfImportedImages);
			Assert.That(results.JobMessages, Has.Count.Positive);

			// test result job returns ProgressCompletedRows larger by 1 than real files count
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(ExpectedNumberOfImportedImages + 1));
		}

		private static IEnumerable<ImageImportDto> GetRandomImageFiles(string directory, int numberOfDocumentsToImport, int numberOfImagesPerDocument, ImageFormat imageFormat)
		{
			int imageWidth = 200;
			int imageHeight = 200;

			for (int documentIndex = 1; documentIndex <= numberOfDocumentsToImport; documentIndex++)
			{
				string documentIdentifier = Guid.NewGuid().ToString();
				for (int imageIndex = 1; imageIndex <= numberOfImagesPerDocument; imageIndex++)
				{
					string imageIdentifier = $"{documentIdentifier}_{imageIndex}";
					yield return new ImageImportDto(imageIdentifier, documentIdentifier, RandomHelper.NextImageFile(imageFormat, directory, imageWidth, imageHeight));
				}
			}
		}
	}
}