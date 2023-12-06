// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadProgressManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class DownloadProgressManagerTests
	{
		private DownloadProgressManager _instance;
		private FileRequestRepository _nativeRepository;
		private ImageRepository _imageRepository;
		private LongTextRepository _longTextRepository;
		private FileRequestRepository _pdfRepository;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			this._nativeRepository = new FileRequestRepository();
			this._imageRepository = new ImageRepository();
			this._longTextRepository = new LongTextRepository(null, new TestNullLogger());
			this._pdfRepository = new FileRequestRepository();
			this._status = new Mock<IStatus>();
			this._instance = new DownloadProgressManager(
				this._nativeRepository,
				this._imageRepository,
				this._longTextRepository,
				this._pdfRepository,
				this._status.Object,
				new TestNullLogger());
		}

		[Test]
		public void ItShouldUpdateProgress()
		{
			// DATA SET
			// Artifact A - native
			FileRequest<ObjectExportInfo> nativeA = ModelFactory.GetNative(this._nativeRepository);

			// Artifact B - native and 2 images
			FileRequest<ObjectExportInfo> nativeB = ModelFactory.GetNative(this._nativeRepository);
			ImageRequest imageB1 = ModelFactory.GetImage(this._imageRepository, nativeB.Artifact.ArtifactID);
			ImageRequest imageB2 = ModelFactory.GetImage(this._imageRepository, nativeB.Artifact.ArtifactID);

			// Artifact C - native and long text
			FileRequest<ObjectExportInfo> nativeC = ModelFactory.GetNative(this._nativeRepository);
			LongText textC = ModelFactory.GetLongText(nativeC.Artifact.ArtifactID, this._longTextRepository);

			// Artifact D - native, image and long text
			FileRequest<ObjectExportInfo> nativeD = ModelFactory.GetNative(this._nativeRepository);
			ImageRequest imageD = ModelFactory.GetImage(this._imageRepository, nativeD.Artifact.ArtifactID);
			LongText textD = ModelFactory.GetLongText(nativeD.Artifact.ArtifactID, this._longTextRepository);

			int actualDocumentExportedCount = 0;
			int actualNativeExported = 0, actualImageExported = 0, actualLongTextExported = 0;
			string actualLine = string.Empty;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);
			this._status.Setup(x => x.WriteStatusLine(It.IsAny<EventType2>(), It.IsAny<string>(), It.IsAny<bool>()))
				.Callback((EventType2 eventType, string line, bool isEssential) => actualLine = line);
			this._status.Setup(x => x.UpdateFilesExportedCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Callback((int native, int pdf, int image, int longText) =>
						{
							actualNativeExported = native;
							actualImageExported = image;
							actualLongTextExported = longText;
						});

			// ACT
			this._instance.MarkFileAsCompleted(imageB1.ExportRequest.DestinationLocation, imageB1.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(0));
			Assert.That(actualNativeExported, Is.EqualTo(0));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(0));
			Assert.That(actualLine, Does.Contain(string.Empty));

			// 1 downloaded (A)
			this._instance.MarkFileAsCompleted(nativeA.ExportRequest.DestinationLocation, nativeA.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
			Assert.That(actualNativeExported, Is.EqualTo(1));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(0));

			this._instance.MarkFileAsCompleted(nativeC.ExportRequest.DestinationLocation, nativeC.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
			Assert.That(actualNativeExported, Is.EqualTo(2));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(0));

			// 2 downloaded (A, C)
			this._instance.MarkLongTextAsCompleted(textC.ExportRequest.DestinationLocation, textC.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));
			Assert.That(actualNativeExported, Is.EqualTo(2));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(1));

			this._instance.MarkFileAsCompleted(nativeB.ExportRequest.DestinationLocation, nativeB.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(1));

			// 3 downloaded (A, C, B)
			this._instance.MarkFileAsCompleted(imageB2.ExportRequest.DestinationLocation, imageB2.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(2));
			Assert.That(actualLongTextExported, Is.EqualTo(1));

			this._instance.MarkLongTextAsCompleted(textD.ExportRequest.DestinationLocation, textD.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(2));
			Assert.That(actualLongTextExported, Is.EqualTo(2));

			this._instance.MarkFileAsCompleted(imageD.ExportRequest.DestinationLocation, imageD.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(3));
			Assert.That(actualLongTextExported, Is.EqualTo(2));

			// 4 download (A, C, B, D)
			this._instance.MarkFileAsCompleted(nativeD.ExportRequest.DestinationLocation, nativeD.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
			Assert.That(actualNativeExported, Is.EqualTo(4));
			Assert.That(actualImageExported, Is.EqualTo(3));
			Assert.That(actualLongTextExported, Is.EqualTo(2));

			// None downloaded - just an error.
			this._instance.MarkArtifactAsError(999, "error message");
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));
			Assert.That(actualNativeExported, Is.EqualTo(4));
			Assert.That(actualImageExported, Is.EqualTo(3));
			Assert.That(actualLongTextExported, Is.EqualTo(2));

			// Handle duplicates
			this._instance.MarkArtifactAsError(999, "error message");
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));
			Assert.That(actualNativeExported, Is.EqualTo(4));
			Assert.That(actualImageExported, Is.EqualTo(3));
			Assert.That(actualLongTextExported, Is.EqualTo(2));
		}

		[Test]
		public void ItShouldUpdateProgressForDocumentsWithPdf()
		{
			// ARRANGE
			// Artifact A - native
			FileRequest<ObjectExportInfo> nativeA = ModelFactory.GetNative(this._nativeRepository);

			// Artifact B - native and PDF
			FileRequest<ObjectExportInfo> nativeB = ModelFactory.GetNative(this._nativeRepository);
			FileRequest<ObjectExportInfo> pdfB = ModelFactory.GetPdf(this._pdfRepository, nativeB.Artifact.ArtifactID);

			// Artifact C - native, long text and PDF
			FileRequest<ObjectExportInfo> nativeC = ModelFactory.GetNative(this._nativeRepository);
			LongText textC = ModelFactory.GetLongText(nativeC.Artifact.ArtifactID, this._longTextRepository);
			FileRequest<ObjectExportInfo> pdfC = ModelFactory.GetPdf(this._pdfRepository, nativeC.Artifact.ArtifactID);

			// Artifact D - native, image and PDF
			FileRequest<ObjectExportInfo> nativeD = ModelFactory.GetNative(this._nativeRepository);
			ImageRequest imageD = ModelFactory.GetImage(this._imageRepository, nativeD.Artifact.ArtifactID);
			FileRequest<ObjectExportInfo> pdfD = ModelFactory.GetPdf(this._pdfRepository, nativeD.Artifact.ArtifactID);

			// Artifact E - native, image, long text and PDF
			FileRequest<ObjectExportInfo> nativeE = ModelFactory.GetNative(this._nativeRepository);
			ImageRequest imageE = ModelFactory.GetImage(this._imageRepository, nativeE.Artifact.ArtifactID);
			LongText textE = ModelFactory.GetLongText(nativeE.Artifact.ArtifactID, this._longTextRepository);
			FileRequest<ObjectExportInfo> pdfE = ModelFactory.GetPdf(this._pdfRepository, nativeE.Artifact.ArtifactID);

			int actualDocumentExportedCount = 0;
			int actualNativeExported = 0, actualPdfExported = 0, actualImageExported = 0, actualLongTextExported = 0;
			string actualLine = string.Empty;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);
			this._status.Setup(x => x.WriteStatusLine(It.IsAny<EventType2>(), It.IsAny<string>(), It.IsAny<bool>()))
				.Callback((EventType2 eventType, string line, bool isEssential) => actualLine = line);
			this._status.Setup(x => x.UpdateFilesExportedCount(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
				.Callback((int native, int pdf, int image, int longText) =>
						{
							actualNativeExported = native;
							actualPdfExported = pdf;
							actualImageExported = image;
							actualLongTextExported = longText;
						});

			// ACT
			this._instance.MarkFileAsCompleted(pdfB.ExportRequest.DestinationLocation, pdfB.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(0));
			Assert.That(actualNativeExported, Is.EqualTo(0));
			Assert.That(actualPdfExported, Is.EqualTo(1));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(0));
			Assert.That(actualLine, Does.Contain(string.Empty));

			// Download A
			this._instance.MarkFileAsCompleted(nativeA.ExportRequest.DestinationLocation, nativeA.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
			Assert.That(actualNativeExported, Is.EqualTo(1));
			Assert.That(actualPdfExported, Is.EqualTo(1));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(0));

			// Download B
			this._instance.MarkFileAsCompleted(nativeB.ExportRequest.DestinationLocation, nativeB.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));
			Assert.That(actualNativeExported, Is.EqualTo(2));
			Assert.That(actualPdfExported, Is.EqualTo(1));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(0));

			// Download C
			this._instance.MarkFileAsCompleted(nativeC.ExportRequest.DestinationLocation, nativeC.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualPdfExported, Is.EqualTo(1));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(0));

			this._instance.MarkFileAsCompleted(pdfC.ExportRequest.DestinationLocation, pdfC.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualPdfExported, Is.EqualTo(2));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(0));

			this._instance.MarkLongTextAsCompleted(textC.ExportRequest.DestinationLocation, textC.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualPdfExported, Is.EqualTo(2));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(1));

			// Download D
			this._instance.MarkFileAsCompleted(pdfD.ExportRequest.DestinationLocation, pdfD.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualNativeExported, Is.EqualTo(3));
			Assert.That(actualPdfExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(1));

			this._instance.MarkFileAsCompleted(nativeD.ExportRequest.DestinationLocation, nativeD.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualNativeExported, Is.EqualTo(4));
			Assert.That(actualPdfExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(0));
			Assert.That(actualLongTextExported, Is.EqualTo(1));

			this._instance.MarkFileAsCompleted(imageD.ExportRequest.DestinationLocation, imageD.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
			Assert.That(actualNativeExported, Is.EqualTo(4));
			Assert.That(actualPdfExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(1));

			// Download E
			this._instance.MarkLongTextAsCompleted(textE.ExportRequest.DestinationLocation, textE.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
			Assert.That(actualNativeExported, Is.EqualTo(4));
			Assert.That(actualPdfExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(2));

			this._instance.MarkFileAsCompleted(nativeE.ExportRequest.DestinationLocation, nativeE.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
			Assert.That(actualNativeExported, Is.EqualTo(5));
			Assert.That(actualPdfExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(1));
			Assert.That(actualLongTextExported, Is.EqualTo(2));

			this._instance.MarkFileAsCompleted(imageE.ExportRequest.DestinationLocation, imageE.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
			Assert.That(actualNativeExported, Is.EqualTo(5));
			Assert.That(actualPdfExported, Is.EqualTo(3));
			Assert.That(actualImageExported, Is.EqualTo(2));
			Assert.That(actualLongTextExported, Is.EqualTo(2));

			this._instance.MarkFileAsCompleted(pdfE.ExportRequest.DestinationLocation, pdfE.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));
			Assert.That(actualNativeExported, Is.EqualTo(5));
			Assert.That(actualPdfExported, Is.EqualTo(4));
			Assert.That(actualImageExported, Is.EqualTo(2));
			Assert.That(actualLongTextExported, Is.EqualTo(2));
		}

		[Test]
		public void ItShouldCountDuplicateSourceNatives()
		{
			// ARRANGE
			const string SourceLocation = @"\\files\T001\Files\EDDS123456\8B845660-CBD8-456E-BDB9-15BFDB33BD7D";
			FileRequest<ObjectExportInfo> native1 = ModelFactory.GetNative(this._nativeRepository, SourceLocation, @"C:\temp\REL-0001.doc");
			FileRequest<ObjectExportInfo> native2 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToLowerInvariant(), @"C:\temp\REL-0002.doc");
			FileRequest<ObjectExportInfo> native3 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToUpperInvariant(), @"C:\temp\REL-0003.doc");
			FileRequest<ObjectExportInfo> native4 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToLowerInvariant() + " ", @"C:\temp\REL-0004.doc");
			FileRequest<ObjectExportInfo> native5 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToUpperInvariant() + " ", @"C:\temp\REL-0005.doc");
			int actualDocumentExportedCount = 0;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);

			// ACT
			foreach (FileRequest<ObjectExportInfo> native in new[] { native1, native2, native3, native4, native5 })
			{
				this._instance.MarkFileAsCompleted(native.ExportRequest.DestinationLocation, native.ExportRequest.Order, true);
			}

			// ASSERT
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));
		}

		[Test]
		public void ItShouldCountDuplicateSourceImages()
		{
			// ARRANGE
			const string SourceLocation = @"\\files\T001\Files\EDDS123456\F780BC4E-39FC-44E2-8F48-5608CBAA795C";
			FileRequest<ObjectExportInfo> native = ModelFactory.GetNative(this._nativeRepository);
			ImageRequest image1 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation, @"C:\temp\file1.tiff");
			ImageRequest image2 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToLowerInvariant(), @"C:\temp\file2.tiff");
			ImageRequest image3 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToUpperInvariant(), @"C:\temp\file3.tiff");
			ImageRequest image4 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToLowerInvariant() + " ", @"C:\temp\file4.tiff");
			ImageRequest image5 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToUpperInvariant() + " ", @"C:\temp\file5.tiff");
			int actualDocumentExportedCount = 0;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);
			this._instance.MarkFileAsCompleted(
				native.ExportRequest.DestinationLocation,
				native.ExportRequest.Order,
				true);

			// ACT
			foreach (ImageRequest image in new[] { image1, image2, image3, image4, image5 })
			{
				this._instance.MarkFileAsCompleted(image.ExportRequest.DestinationLocation, image.ExportRequest.Order, true);
			}

			// ASSERT
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
		}

		[Test]
		public void ItShouldFinalizeTheBatchProcessedCount()
		{
			// ARRANGE
			FileRequest<ObjectExportInfo> nativeWithImage = ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetImage(this._imageRepository, nativeWithImage.Artifact.ArtifactID);
			ModelFactory.GetImage(this._imageRepository, nativeWithImage.Artifact.ArtifactID);
			FileRequest<ObjectExportInfo> nativeWithText = ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetLongText(nativeWithText.Artifact.ArtifactID, this._longTextRepository);
			FileRequest<ObjectExportInfo> nativeWithPdf = ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetPdf(this._pdfRepository, nativeWithPdf.Artifact.ArtifactID);
			ModelFactory.GetNative(this._nativeRepository);
			int actualDocumentExportedCount = 0;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);

			// ACT
			this._instance.FinalizeBatchProcessedCount();

			// ASSERT
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
		}

		[Test]
		public void ItShouldThrowExceptionWhenSelectedImageFileIsNull()
		{
			// ARRANGE
			this._imageRepository = null;
			// Act and Assert
			Assert.Throws<ArgumentNullException>(() => ModelFactory.GetImage(this._imageRepository, 1, "sourceLocation", "targetFile"));
		}

		[Test]
		public void GetImage_WithValidArguments_ReturnsImageRequest()
		{
			// Arrange
			this._imageRepository = new ImageRepository();
			var artifactId = 1;
			var sourceLocation = "validSourceLocation";

			// Act
			var result = ModelFactory.GetImage(this._imageRepository, artifactId, sourceLocation);

			// Assert
			Assert.IsNotNull(result);
			Assert.AreEqual(artifactId, result.Artifact.ArtifactID);
			Assert.AreEqual(sourceLocation, result.Artifact.SourceLocation);
			
		}
	}
}