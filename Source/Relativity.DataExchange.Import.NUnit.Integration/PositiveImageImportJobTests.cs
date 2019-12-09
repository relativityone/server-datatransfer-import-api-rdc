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
		[Test]
		[Pairwise]
		public void ShouldImportTheFiles(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client,
			[Values(true, false)] bool disableImageLocationValidation,
			[Values(true, false)] bool disableImageTypeValidation,
			[Values(ImageFormat.Jpeg, ImageFormat.Tiff)] ImageFormat imageFormat)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			ForceClient(client);
			AppSettings.Instance.DisableImageLocationValidation = disableImageLocationValidation;
			AppSettings.Instance.DisableImageTypeValidation = disableImageTypeValidation;

			this.InitializeImportApiWithUserAndPassword(ImageImportSettingsProvider.GetImageFilePathSourceDocumentImportSettings());

			const int NumberOfDocumentsToImport = 5;
			const int NumberOfImagesPerDocument = 3;
			IEnumerable<ImageImportDto> importData = GetRandomImageFiles(this.TempDirectory.Directory, NumberOfDocumentsToImport, NumberOfImagesPerDocument, imageFormat);

			ImportTestJobResult results = this.Execute(importData);

			// ASSERT
			const int TotalNumberOfImagesToImport = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(TotalNumberOfImagesToImport);
			Assert.That(results.JobMessages, Has.Count.Positive);

			// test result job returns ProgressCompletedRows larger by 1 than real files count
			Assert.That(results.ProgressCompletedRows, Has.Count.EqualTo(TotalNumberOfImagesToImport + 1));
		}

		private static IEnumerable<ImageImportDto> GetRandomImageFiles(string directory, int numberOfDocumentsToImport, int numberOfImagesPerDocument, ImageFormat imageFormat)
		{
			for (int i = 1; i <= numberOfDocumentsToImport; i++)
			{
				string documentIdentifier = $"{Guid.NewGuid():D}";
				for (int j = 1; j <= numberOfImagesPerDocument; j++)
				{
					yield return new ImageImportDto($"{documentIdentifier}_{j}", $"{documentIdentifier}", RandomHelper.NextImageFile(imageFormat, directory));
				}
			}
		}
	}
}