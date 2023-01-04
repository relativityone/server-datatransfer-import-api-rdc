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
	using System.IO;
	using System.Linq;
	using System.Text;
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
	[Feature.DataTransfer.TransferApi]
	[TestExecutionCategory.CI]
	public class PositiveImageImportJobTests : ImportJobTestBase<ImageImportExecutionContext>
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "This field is used as ValueSource")]
		private static readonly TapiClient[] AvailableTapiClients = TapiClientModeAvailabilityChecker.GetAvailableTapiClients();
		private readonly int initialBatchSize = AppSettings.Instance.ImportBatchSize;

		[OneTimeSetUp]
		public Task OneTimeSetUp()
		{
			return this.ResetContextAsync();
		}

		[TearDown]
		public Task TearDown()
		{
			AppSettings.Instance.ImportBatchSize = this.initialBatchSize;
			return RdoHelper.DeleteAllObjectsByTypeAsync(this.TestParameters, (int)ArtifactType.Document);
		}

		[IdentifiedTest("9db2e7f4-0bc8-46a8-9e95-621ca9bcc5c1")]
		[TestType.MainFlow]
		[Pairwise]
		public void ShouldImportManyImages()
		{
			const int NumberOfDocumentsToImport = 20;
			const int NumberOfImagesPerDocument = 10;

			long expectedFileBytes = 0L;
			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg)
				.WithFileSizeBytesAggregator(fileSize => expectedFileBytes += fileSize)
				.Build();

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

		[IdentifiedTest("577e9faa-31e6-4bd8-b406-7a066cc0aeb4")]
		[TestType.Error]
		[Pairwise]
		public void ShouldReturnAppendErrorsWhenImagesAlreadyExists()
		{
			const int NumberOfDocumentsToImport = 20;
			const int NumberOfImagesPerDocument = 10;

			long expectedFileBytes = 0L;
			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg)
				.WithFileSizeBytesAggregator(fileSize => expectedFileBytes += fileSize).Build();

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

		[IdentifiedTest("6bfb799e-5c8f-4a5c-8092-c9042af62072")]
		[TestType.Error]
		[Pairwise]
		public void ShouldReturnOverlayErrorsWhenNoImagesExists()
		{
			const int NumberOfDocumentsToImport = 20;
			const int NumberOfImagesPerDocument = 10;

			long expectedFileBytes = 0L;
			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg).WithFileSizeBytesAggregator(fileSize => expectedFileBytes += fileSize).Build();

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

		[IdentifiedTest("77b7d0a8-a968-4e16-841f-6b123f050e90")]
		[TestType.MainFlow]
		public async Task ShouldOverlayImagesUsingIdentityField()
		{
			// ARRANGE
			const int NumberOfDocumentsToImport = 2;
			const int NumberOfImagesPerDocument = 2;

			List<ImageImportWithFileNameDto> imagesToImport = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg).Build().ToList();

			// append documents
			string[] beginBates = imagesToImport.Select(x => x.DocumentIdentifier).Distinct().ToArray();
			string[] controlNumbers = beginBates.Select(bates => $"Different than begin bates: {bates}").ToArray();

			var dataSource = new ImportDataSourceBuilder()
				.AddField(WellKnownFields.ControlNumber, controlNumbers)
				.AddField(WellKnownFields.BatesNumber, beginBates)
				.Build();

			ImportHelper.ImportDocumentsMetadata(this.TestParameters, dataSource);

			// overlay images
			var batesNumberFieldId = await FieldHelper.GetFieldArtifactIdAsync(this.TestParameters, WellKnownFields.BatesNumber).ConfigureAwait(false);

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.Overlay)
				.WithIdentityField(batesNumberFieldId);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(imagesToImport);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, expectedTotalRows: NumberOfDocumentsToImport * NumberOfImagesPerDocument);
		}

		[IdentifiedTest("b5acca4f-fea7-4353-9d42-46b32307fbf7")]
		[TestType.MainFlow]
		public async Task ShouldOverlayProductionImagesUsingBatesNumberField()
		{
			// ARRANGE
			const int NumberOfDocumentsToImport = 2;
			const int NumberOfImagesPerDocument = 2;

			var productionArtifactId = await ProductionHelper.CreateProductionAsync(
				                           this.TestParameters,
				                           productionName: "ProductionToImport",
				                           batesPrefix: "BatesPrefix").ConfigureAwait(false);

			List<ImageImportWithFileNameDto> imagesToImport = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg).Build().ToList();

			// append documents
			string[] beginBates = imagesToImport.Select(x => x.DocumentIdentifier).Distinct().ToArray();
			string[] controlNumbers = beginBates.Select(bates => $"Different than begin bates: {bates}").ToArray();

			var dataSource = new ImportDataSourceBuilder()
				.AddField(WellKnownFields.ControlNumber, controlNumbers)
				.AddField(WellKnownFields.BatesNumber, beginBates)
				.Build();

			ImportHelper.ImportDocumentsMetadata(this.TestParameters, dataSource);

			// overlay images
			var batesNumberFieldId = await FieldHelper.GetFieldArtifactIdAsync(this.TestParameters, WellKnownFields.BatesNumber).ConfigureAwait(false);

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.ForProduction(productionArtifactId)
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.Overlay)
				.WithBeginBatesField(batesNumberFieldId);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(imagesToImport);

			// ASSERT
			this.ThenTheImportJobIsSuccessful(results, expectedTotalRows: NumberOfDocumentsToImport * NumberOfImagesPerDocument);
		}

		[IdentifiedTest("6bfb799e-5c8f-4a5c-8092-c9042af62072")]
		[TestType.Error]
		[Pairwise]
		public void ShouldReturnItemErrorsWhenIdentifierContainsComma()
		{
			const int NumberOfDocumentsToImport = 5;
			const int NumberOfImagesPerDocument = 2;

			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg).WithInvalidDocumentIdentifier().Build();

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

		[Category(TestCategories.Regression)]
		[IdentifiedTest("f5b4a1d7-9dfc-4931-ba55-0fb0d56564ad")]
		[TestType.MainFlow]
		[Pairwise]
		public void ShouldImportTheImage(
			[Values(true, false)] bool useFileNames,
			[Values(true, false)] bool useDefaultFieldNames,
			[Values(true, false)] bool useDataTableSource,
			[Values(ImageFormat.Jpeg, ImageFormat.Tiff)] ImageFormat imageFormat,
			[ValueSource(nameof(AvailableTapiClients))] TapiClient client)
		{
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
				string fileName = $"ႝ\\ /:*?{RandomHelper.NextString(10, 10)}.{Path.GetExtension(imageFile.FullName)}";
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

		[TestCaseSource(nameof(GetExtractedTextDataSource))]
		[TestType.MainFlow]
		public void ShouldImportImagesWithExtractedText(ImageFormat imageFormat, Encoding extractedTextEncoding, bool disableEncodingCheck)
		{
			// ARRANGE
			const int NumberOfDocumentsToImport = 1;
			const int NumberOfImagesPerDocument = 2;
			const int ExtractedTextLength = 100;

			List<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				imageFormat)
				.WithExtractedText(ExtractedTextLength, extractedTextEncoding).Build().ToList();

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithExtractedText(extractedTextEncoding, disableEncodingCheck)
				.WithOverlayMode(OverwriteModeEnum.Append);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			ImportTestJobResult testResult = this.JobExecutionContext.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(testResult, ExpectedNumberOfImportedImages);
			ThenTheExtractedTextIsCorrect(importData, NumberOfImagesPerDocument, extractedTextEncoding);
		}

		[IdentifiedTest("ebf17f63-5b1b-4731-90c4-963410ad7902")]
		[TestType.MainFlow]
		public async Task ShouldAuditImportOfImagesWithExtractedTextAsync()
		{
			// ARRANGE
			const int NumberOfDocumentsToImport = 1;
			const int NumberOfImagesPerDocument = 2;
			const int ExtractedTextLength = 100;
			Encoding extractedTextEncoding = Encoding.UTF8;

			List<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
					this.TempDirectory.Directory,
					NumberOfDocumentsToImport,
					NumberOfImagesPerDocument,
					ImageFormat.Tiff)
				.WithExtractedText(ExtractedTextLength, extractedTextEncoding).Build().ToList();

			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithExtractedText(extractedTextEncoding, true)
				.WithOverlayMode(OverwriteModeEnum.Append);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			// ACT
			var executionTime = DateTime.Now;
			ImportTestJobResult testResult = this.JobExecutionContext.Execute(importData);

			// ASSERT
			const int ExpectedNumberOfImportedImages = NumberOfDocumentsToImport * NumberOfImagesPerDocument;
			this.ThenTheImportJobIsSuccessful(testResult, ExpectedNumberOfImportedImages);
			await this.ThenTheAuditsContainExtractedTextDetails(importData, NumberOfImagesPerDocument, executionTime).ConfigureAwait(false);
		}

		[IdentifiedTest("0d331774-f9d8-4384-b167-196fae723a71")]
		[TestType.MainFlow]
		public void ShouldImportImagesInBatches()
		{
			const int BatchSize = 50;
			const int NumberOfDocumentsToImport = 20;
			const int NumberOfImagesPerDocument = 10;
			const int NumberOfErrorsPerBatch = 20;
			const int ExpectedNumberOfBatches = NumberOfDocumentsToImport * NumberOfImagesPerDocument / BatchSize;

			AppSettings.Instance.ImportBatchSize = BatchSize;
			var imageSettingsBuilder = new ImageSettingsBuilder()
				.WithDefaultFieldNames()
				.WithOverlayMode(OverwriteModeEnum.Append);

			this.JobExecutionContext.InitializeImportApiWithUserAndPassword(this.TestParameters, imageSettingsBuilder);
			this.JobExecutionContext.UseFileNames = true;

			IEnumerable<ImageImportWithFileNameDto> importData = new ImageImportWithFileNameDtoBuilder(
				this.TempDirectory.Directory,
				NumberOfDocumentsToImport,
				NumberOfImagesPerDocument,
				ImageFormat.Jpeg).Build().ToList();

			IEnumerable<ImageImportWithFileNameDto> importDataErrors = importData.Where((_, i) => i % BatchSize < NumberOfErrorsPerBatch).ToList();
			this.JobExecutionContext.Execute(importDataErrors);

			// ACT
			ImportTestJobResult results = this.JobExecutionContext.Execute(importData);

			// ASSERT
			Assert.That(results.JobReportMetadataBytes, Is.Positive);
			Assert.That(results.BatchReports, Has.Count.EqualTo(ExpectedNumberOfBatches));

			foreach (BatchReport batchReport in results.BatchReports)
			{
				Assert.That(batchReport.NumberOfFiles, Is.EqualTo(BatchSize - NumberOfErrorsPerBatch));
				Assert.That(batchReport.NumberOfRecords, Is.EqualTo((BatchSize - NumberOfErrorsPerBatch) / NumberOfImagesPerDocument));
				Assert.That(batchReport.NumberOfRecordsWithErrors, Is.EqualTo(NumberOfErrorsPerBatch));
			}
		}

		private static IEnumerable<TestCaseData> GetExtractedTextDataSource()
		{
			const string IdPrefix = "dd4a5bc2-1ced-4150-9f5e-e601aad1f";
			ImageFormat[] imageFormatSource = { ImageFormat.Jpeg, ImageFormat.Tiff };
			Encoding[] encodingSource = { Encoding.UTF8, Encoding.ASCII, Encoding.BigEndianUnicode, Encoding.UTF7 };
			bool[] disableEncodingCheckSource = { true, false };

			for (var i = 0; i < imageFormatSource.Length; i++)
			{
				for (var j = 0; j < encodingSource.Length; j++)
				{
					for (var k = 0; k < disableEncodingCheckSource.Length; k++)
					{
						yield return new TestCaseData(imageFormatSource[i], encodingSource[j], disableEncodingCheckSource[k]).WithId($"{IdPrefix}{i}{j}{k}");
					}
				}
			}
		}

		private static void ThenTheExtractedTextIsCorrect(List<ImageImportWithFileNameDto> importData, int numberOfImagesPerDocument, Encoding extractedTextEncoding)
		{
			for (int documentIndex = 0; documentIndex < importData.Count; documentIndex += numberOfImagesPerDocument)
			{
				string actualExtractedText = (string)GetDocumentFieldValue(QueryDocument(importData[documentIndex].BatesNumber), WellKnownFields.ExtractedText);
				StringBuilder expectedExtractedTextBuilder = new StringBuilder();
				for (int imageIndex = 0; imageIndex < numberOfImagesPerDocument; imageIndex++)
				{
					string extractedTextPath = Path.ChangeExtension(importData[documentIndex + imageIndex].FileLocation, ".txt");
					expectedExtractedTextBuilder.Append(File.ReadAllText(extractedTextPath, extractedTextEncoding));
				}

				Assert.That(actualExtractedText, Is.EqualTo(expectedExtractedTextBuilder.ToString()), $"Invalid value of extracted text field for document: {importData[documentIndex].BatesNumber}");
			}
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

			IList<FileDto> documentImages = FileExportHelper.QueryImageFileInfo(document.ArtifactID).ToList();
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
				new[] { WellKnownFields.ArtifactId, WellKnownFields.ControlNumber, WellKnownFields.HasImages, WellKnownFields.HasNative, WellKnownFields.BatesNumber, WellKnownFields.RelativityImageCount, WellKnownFields.ExtractedText });

			return (from document in documents
					from pair in document.FieldValues
					where pair.Field.Name == WellKnownFields.ControlNumber && pair.Value.ToString() == controlNumber
					select document).FirstOrDefault();
		}

		private async Task ThenTheAuditsContainExtractedTextDetails(List<ImageImportWithFileNameDto> importData, int numberOfImagesPerDocument, DateTime executionTime)
		{
			for (int documentIndex = 0; documentIndex < importData.Count; documentIndex += numberOfImagesPerDocument)
			{
				string documentControlNumber = importData[documentIndex].BatesNumber;
				int documentArtifactId = (int)GetDocumentFieldValue(QueryDocument(documentControlNumber), WellKnownFields.ArtifactId);

				IList<string> auditDetails = await AuditHelper.GetAuditActionDetailsForObjectAsync(this.TestParameters, executionTime, AuditHelper.AuditAction.Create, numberOfImagesPerDocument, documentArtifactId).ConfigureAwait(false);
				foreach (var details in auditDetails)
				{
					Assert.That(details, Contains.Substring("extractedTextEncodingPageCode"), $"Extracted text encoding missing from audit details: {details}");
				}
			}
		}
	}
}