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
		public void ShouldImportImagesUsingDifferentTransferModesAndFileFormats(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(ImageFormat.Jpeg, ImageFormat.Tiff)] ImageFormat imageFormat)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			ForceClient(client);

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
			Assert.That(results.CompletedJobReport.TotalRows, Is.EqualTo(ExpectedNumberOfImportedImages));
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