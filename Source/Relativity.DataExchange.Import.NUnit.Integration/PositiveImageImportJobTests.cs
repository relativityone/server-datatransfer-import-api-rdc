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
	using System.Data;
	using System.IO;
	using System.Linq;
	using System.Net;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Import.NUnit.Integration.SetUp;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportDocuments]
	public class PositiveImageImportJobTests : ImportJobTestBase<ImageImportBulkArtifactJob, ImageSettings>
	{
		public PositiveImageImportJobTests()
			: base(new ImageImportApiSetUp())
		{
		}

		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("9db2e7f4-0bc8-46a8-9e95-621ca9bcc5c1")]
		[Pairwise]
		public void ShouldImportManyImagesUsingDifferentTransferModes(
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			// ARRANGE
			TapiClientModeAvailabilityChecker.SkipTestIfModeNotAvailable(AssemblySetup.TestParameters, client);

			ForceClient(client);

			this.InitializeImportApiWithUserAndPassword(ImageImportSettingsProvider.GetImageFilePathSourceDocumentImportSettings(false));

			const int NumberOfDocumentsToImport = 5;
			const int NumberOfImagesPerDocument = 3;

			IEnumerable<ImageImportDto> importData = GetRandomImageFiles(this.TempDirectory.Directory, NumberOfDocumentsToImport, NumberOfImagesPerDocument, ImageFormat.Jpeg);

			// ACT
			this.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(ExpectedNumberOfImportedImages);
		}

		[Test]
		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("6fbd96bd-7c94-48af-b156-77e02f4c94fa")]
		[Pairwise]
		public void ShouldImportTheImage(
			[Values(true, false)] bool useFileNames,
			[Values(true, false)] bool useDefaultFieldNames,
			[Values(ImageFormat.Jpeg, ImageFormat.Tiff)] ImageFormat imageFormat)
		{
			this.InitializeImportApiWithUserAndPassword(ImageImportSettingsProvider.GetImageFilePathSourceDocumentImportSettings(useDefaultFieldNames));

			// ARRANGE
			const int ExpectedNumberOfImportedImages = 1;
			RdoHelper.DeleteAllObjectsByType(AssemblySetup.TestParameters, WellKnownArtifactTypes.DocumentArtifactTypeId).Wait();

			string batesNumber = RandomHelper.NextString(10, 10);
			string controlNumber = RandomHelper.NextString(10, 10);

			// The Bates field for the first image in a set must be identical to the doc identifier.
			// batesNumber = controlNumber;
			int imageWidth = 200;
			int imageHeight = 200;
			string filePath = RandomHelper.NextImageFile(imageFormat, this.TempDirectory.Directory, imageWidth, imageHeight);
			string fileName = AddSpecialCharacters($"{RandomHelper.NextString(10, 10)}.{Path.GetExtension(filePath)}");

			var imageImportDto = new ImageImportDto(controlNumber, batesNumber, fileName, filePath);
			var importData = new List<ImageImportDto>() { imageImportDto };

			this.ImportApiSetUp<ImageImportApiSetUp>().UseFileNames = useFileNames;
			this.ImportApiSetUp<ImageImportApiSetUp>().UseDefaultFieldNames = useDefaultFieldNames;

			// ACT
			this.Execute(importData);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(ExpectedNumberOfImportedImages);
			ThenRelativityObjectCountsIsCorrect(ExpectedNumberOfImportedImages);
			ThenTheImportedDocumentIsCorrect(imageImportDto, useFileNames);
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
					string controlNumber = $"{documentIdentifier}_{imageIndex}";
					string filePath = RandomHelper.NextImageFile(imageFormat, directory, imageWidth, imageHeight);
					string fileName = AddSpecialCharacters($"{RandomHelper.NextString(10, 10)}.{Path.GetExtension(filePath)}");
					yield return new ImageImportDto(controlNumber, documentIdentifier, fileName, filePath);
				}
			}
		}

		private static string AddSpecialCharacters(string text)
		{
			return $"ႝ\\ /:*?{text}";
		}

		private static void ThenRelativityObjectCountsIsCorrect(int expectedNumberOfImportedDocuments)
		{
			int actualDocCount = RdoHelper.QueryRelativityObjectCount(AssemblySetup.TestParameters, WellKnownArtifactTypes.DocumentArtifactTypeId);
			Assert.That(actualDocCount, Is.EqualTo(expectedNumberOfImportedDocuments));
		}

		private static void ThenTheImportedDocumentIsCorrect(ImageImportDto imageImportDto, bool useFileNames)
		{
			RelativityObject document = QueryDocument(imageImportDto.ControlNumber);

			Assert.That(document, Is.Not.Null);
			Choice hasImagesField = GetDocumentFieldValue(document, WellKnownFields.HasImages) as Choice;
			Assert.That(hasImagesField, Is.Not.Null);
			Assert.That(hasImagesField.Name, Is.Not.Null);
			Assert.That(hasImagesField.Name, Is.EqualTo("Yes"));
			bool hasNativeField = Convert.ToBoolean(GetDocumentFieldValue(document, WellKnownFields.HasNative));
			Assert.That(hasNativeField, Is.False);
			int? relativityImageCount = Convert.ToInt32(GetDocumentFieldValue(document, WellKnownFields.RelativityImageCount));
			Assert.That(relativityImageCount, Is.Positive);

			IList<FileDto> documentImages = QueryImageFileInfo(document.ArtifactID).ToList();
			Assert.That(documentImages, Is.Not.Null);
			Assert.That(documentImages.Count, Is.EqualTo(1));
			FileDto imageFile = documentImages[0];
			Assert.That(imageFile.DocumentArtifactId, Is.EqualTo(document.ArtifactID));
			Assert.That(imageFile.FileId, Is.Positive);
			if (useFileNames)
			{
				Assert.That(imageFile.FileName, Is.EqualTo(imageImportDto.FileName));
			}

			Assert.That(imageFile.FileType, Is.EqualTo((int)FileType.Tif));
			Assert.That(imageFile.Identifier, Is.EqualTo(imageImportDto.ControlNumber).Or.EqualTo(imageImportDto.DocumentIdentifier));
			Assert.That(imageFile.InRepository, Is.True);
			Assert.That(imageFile.Path, Is.Not.Null.Or.Empty);
			Assert.That(imageFile.Size, Is.Positive);
		}

		private static object GetDocumentFieldValue(RelativityObject relativityObject, string name)
		{
			FieldValuePair pair = relativityObject.FieldValues.FirstOrDefault(x => x.Field.Name == name);
			return pair?.Value;
		}

		private static RelativityObject QueryDocument(string controlNumber)
		{
			var documents = RdoHelper.QueryRelativityObjects(
				AssemblySetup.TestParameters,
				WellKnownArtifactTypes.DocumentArtifactTypeId,
				new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber, WellKnownFields.HasImages, WellKnownFields.HasNative, WellKnownFields.BatesNumber, WellKnownFields.RelativityImageCount });

			return (from document in documents
			        from pair in document.FieldValues
			        where pair.Field.Name == WellKnownFields.ControlNumber && pair.Value.ToString() == controlNumber
			        select document).FirstOrDefault();
		}

		private static IEnumerable<FileDto> QueryImageFileInfo(int artifactId)
		{
			using (kCura.WinEDDS.Service.Export.ISearchManager searchManager = CreateExportSearchManager())
			{
				var dataSet = searchManager.RetrieveImagesForDocuments(
					AssemblySetup.TestParameters.WorkspaceId,
					new[] { artifactId });

				if (dataSet == null || dataSet.Tables.Count == 0)
				{
					return new List<FileDto>();
				}

				DataTable dataTable = dataSet.Tables[0];
				return dataTable.Rows.Cast<DataRow>().Select(dataRow => new FileDto(dataRow));
			}
		}

		private static kCura.WinEDDS.Service.Export.ISearchManager CreateExportSearchManager()
		{
			var credentials = new NetworkCredential(AssemblySetup.TestParameters.RelativityUserName, AssemblySetup.TestParameters.RelativityPassword);
			return new kCura.WinEDDS.Service.SearchManager(credentials, new CookieContainer());
		}
	}
}