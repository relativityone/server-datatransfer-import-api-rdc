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
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;

	using Relativity.DataExchange.Import.NUnit.Integration.Dto;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.Import.JobExecutionContext;
	using Relativity.DataExchange.TestFramework.Import.SimpleFieldsImport;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;
	using Relativity.DataExchange.Transfer;
	using Relativity.Services.Objects.DataContracts;
	using Relativity.Testing.Identification;

	[TestFixture]
	[Feature.DataTransfer.ImportApi.Operations.ImportImages]
	public class PositiveImageImportJobTests : ImportJobTestBase<ImageImportExecutionContext>
	{
		[OneTimeSetUp]
		public Task OneTimeSetUp()
		{
			return this.ResetContextAsync();
		}

		[TearDown]
		public Task TearDown()
		{
			return RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document);
		}

		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("9db2e7f4-0bc8-46a8-9e95-621ca9bcc5c1")]
		[Pairwise]
		public void ShouldImportManyImages()
		{
			const int NumberOfDocumentsToImport = 20;
			const int NumberOfImagesPerDocument = 10;

			long expectedFileBytes = 0L;
			IEnumerable<ImageImportWithFileNameDto> importData = ImageImportWithFileNameDto.GetRandomImageFiles(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg,
				fileSize => expectedFileBytes += fileSize);

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.AppendOverlay);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(results, ExpectedNumberOfImportedImages);
			Assert.That(results.JobReportFileBytes, Is.EqualTo(expectedFileBytes));
			Assert.That(results.JobReportMetadataBytes, Is.Positive);
		}

		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("577e9faa-31e6-4bd8-b406-7a066cc0aeb4")]
		[Pairwise]
		public void ShouldReturnAppendErrorsWhenImagesAlreadyExists()
		{
			const int NumberOfDocumentsToImport = 20;
			const int NumberOfImagesPerDocument = 10;

			long expectedFileBytes = 0L;
			IEnumerable<ImageImportWithFileNameDto> importData = ImageImportWithFileNameDto.GetRandomImageFiles(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg,
				fileSize => expectedFileBytes += fileSize);

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.Append);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			this.JobExecutionContext.Execute(importData);
			expectedFileBytes = 0L;

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			const int ExpectedNumberOfErrors = ExpectedNumberOfImportedImages;
			this.ThenTheImportJobCompletedWithErrors(results, ExpectedNumberOfErrors, ExpectedNumberOfImportedImages);
			Assert.That(results.JobReportFileBytes, Is.EqualTo(expectedFileBytes));
			Assert.That(results.JobReportMetadataBytes, Is.Positive);
		}

		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("6bfb799e-5c8f-4a5c-8092-c9042af62072")]
		[Pairwise]
		public void ShouldReturnOverlayErrorsWhenNoImagesExists()
		{
			const int NumberOfDocumentsToImport = 20;
			const int NumberOfImagesPerDocument = 10;

			long expectedFileBytes = 0L;
			IEnumerable<ImageImportWithFileNameDto> importData = ImageImportWithFileNameDto.GetRandomImageFiles(this.TempDirectory.Directory, NumberOfDocumentsToImport, NumberOfImagesPerDocument, ImageFormat.Jpeg, fileSize => expectedFileBytes += fileSize);

			var imageSettingsBuilder = new ImageSettingsBuilder();
			imageSettingsBuilder.WithDefaultFieldNames();
			imageSettingsBuilder.WithOverlayMode(OverwriteModeEnum.Overlay);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			const int ExpectedNumberOfErrors = ExpectedNumberOfImportedImages;
			this.ThenTheImportJobCompletedWithErrors(results, ExpectedNumberOfErrors, ExpectedNumberOfImportedImages);
			Assert.That(results.JobReportFileBytes, Is.EqualTo(expectedFileBytes));
			Assert.That(results.JobReportMetadataBytes, Is.Positive);
		}

		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("6bfb799e-5c8f-4a5c-8092-c9042af62072")]
		[Pairwise]
		public void ShouldReturnItemErrorsWhenIdentifierContainsComma()
		{
			const int NumberOfDocumentsToImport = 5;
			const int NumberOfImagesPerDocument = 2;
			const bool UseInvalidIdentifier = true;

			IEnumerable<ImageImportWithFileNameDto> importData = ImageImportWithFileNameDto.GetRandomImageFiles(this.TempDirectory.Directory, NumberOfDocumentsToImport, NumberOfImagesPerDocument, ImageFormat.Jpeg, UseInvalidIdentifier, fileSize => { });

			var imageSettingsBuilder = new ImageSettingsBuilder();
			imageSettingsBuilder.WithDefaultFieldNames();
			imageSettingsBuilder.WithOverlayMode(OverwriteModeEnum.Append);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			const int ExpectedNumberOfErrors = ExpectedNumberOfImportedImages;
			this.ThenTheImportJobCompletedWithErrors(results, ExpectedNumberOfErrors, ExpectedNumberOfImportedImages);
		}

		[Test]
		[Category(TestCategories.ImportImage)]
		[Category(TestCategories.Integration)]
		[Category(TestCategories.TransferApi)]
		[IdentifiedTest("f5b4a1d7-9dfc-4931-ba55-0fb0d56564ad")]
		[Pairwise]
		public void ShouldImportTheImage(
			[Values(true, false)] bool useFileNames,
			[Values(true, false)] bool useDefaultFieldNames,
			[Values(true, false)] bool useDataTableSource,
			[Values(ImageFormat.Jpeg, ImageFormat.Tiff)] ImageFormat imageFormat,
			[Values(TapiClient.Aspera, TapiClient.Direct, TapiClient.Web)] TapiClient client)
		{
			ForceClient(client);

			var imageSettingsBuilder = new ImageSettingsBuilder();
			if (useDefaultFieldNames || !useDataTableSource)
			{
				imageSettingsBuilder.WithDefaultFieldNames();
			}

			imageSettingsBuilder.WithOverlayMode(OverwriteModeEnum.AppendOverlay);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);

			const int ExpectedNumberOfImportedImages = 1;
			RdoHelper.DeleteAllObjectsByTypeAsync(AssemblySetup.TestParameters, WellKnownArtifactTypes.DocumentArtifactTypeId).Wait();

			string documentIdentifier = RandomHelper.NextString(10, 10);
			string batesNumber = RandomHelper.NextString(10, 10);

			// The Bates field for the first image in a set must be identical to the doc identifier.
			// batesNumber = controlNumber;
			int imageWidth = 200;
			int imageHeight = 200;

			FileInfo imageFile = RandomHelper.NextImageFile(imageFormat, this.TempDirectory.Directory, imageWidth, imageHeight);
			ImageImportDto imageImportDto;

			this.JobExecutionContext.UseFileNames = useFileNames;
			this.JobExecutionContext.UseDataTableSource = useDataTableSource;
			this.JobExecutionContext.UseDefaultFieldNames = useDefaultFieldNames;

			// ACT
			ImportTestJobResult testResult;
			if (useFileNames)
			{
				string fileName = ImageImportWithFileNameDto.AddSpecialCharacters($"{RandomHelper.NextString(10, 10)}.{Path.GetExtension(imageFile.FullName)}");
				var imageImportWithFileNameDto = new ImageImportWithFileNameDto(batesNumber, documentIdentifier, imageFile.FullName, fileName);
				imageImportDto = imageImportWithFileNameDto;
				testResult = this.JobExecutionContext.Execute(new List<ImageImportWithFileNameDto>() { imageImportWithFileNameDto });
			}
			else
			{
				imageImportDto = new ImageImportDto(batesNumber, documentIdentifier, imageFile.FullName);
				testResult = this.JobExecutionContext.Execute(new List<ImageImportDto>() { imageImportDto });
			}

			// ASSERT
			this.ThenTheImportJobIsSuccessful(testResult, ExpectedNumberOfImportedImages);
			ThenRelativityObjectCountsIsCorrect(ExpectedNumberOfImportedImages);
			ThenTheImportedDocumentIsCorrect(imageImportDto, useFileNames);
			Assert.That(testResult.JobReportFileBytes, Is.EqualTo(imageFile.Length));
			Assert.That(testResult.JobReportMetadataBytes, Is.Positive);
			ThenTheJobCompletedInCorrectTransferMode(testResult, client);
		}

		private static void ThenRelativityObjectCountsIsCorrect(int expectedNumberOfImportedDocuments)
		{
			int actualDocCount = RdoHelper.QueryRelativityObjectCount(AssemblySetup.TestParameters, WellKnownArtifactTypes.DocumentArtifactTypeId);
			Assert.That(actualDocCount, Is.EqualTo(expectedNumberOfImportedDocuments));
		}

		private static void ThenTheImportedDocumentIsCorrect(ImageImportDto imageImportDto, bool useFileNames)
		{
			RelativityObject document = QueryDocument(imageImportDto.BatesNumber);

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
				Assert.That(imageFile.FileName, Is.EqualTo((imageImportDto as ImageImportWithFileNameDto)?.FileName));
			}

			Assert.That(imageFile.FileType, Is.EqualTo((int)FileType.Tif));
			Assert.That(imageFile.Identifier, Is.EqualTo(imageImportDto.BatesNumber).Or.EqualTo(imageImportDto.DocumentIdentifier));
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