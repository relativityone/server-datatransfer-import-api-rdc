// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloadProgressManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Process;
	using Relativity.Logging;

	[TestFixture]
	public class DownloadProgressManagerTests
	{
		private DownloadProgressManager _instance;
		private NativeRepository _nativeRepository;
		private ImageRepository _imageRepository;
		private LongTextRepository _longTextRepository;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			this._nativeRepository = new NativeRepository();
			this._imageRepository = new ImageRepository();
			this._longTextRepository = new LongTextRepository(null, new NullLogger());
			this._status = new Mock<IStatus>();
			this._instance = new DownloadProgressManager(
				this._nativeRepository,
				this._imageRepository,
				this._longTextRepository,
				this._status.Object,
				new NullLogger());
		}

		[Test]
		public void ItShouldUpdateProgress()
		{
			// DATA SET
			Native nativeWithoutImagesOrText_A = ModelFactory.GetNative(this._nativeRepository);

			Native nativeWithTwoImages_B = ModelFactory.GetNative(this._nativeRepository);
			Image image1_B = ModelFactory.GetImage(this._imageRepository, nativeWithTwoImages_B.Artifact.ArtifactID);
			Image image2_B = ModelFactory.GetImage(this._imageRepository, nativeWithTwoImages_B.Artifact.ArtifactID);

			Native nativeWithText_C = ModelFactory.GetNative(this._nativeRepository);
			LongText text_C = ModelFactory.GetLongText(nativeWithText_C.Artifact.ArtifactID, this._longTextRepository);

			Native nativeWithImageAndText_D = ModelFactory.GetNative(this._nativeRepository);
			Image image_D = ModelFactory.GetImage(this._imageRepository, nativeWithImageAndText_D.Artifact.ArtifactID);
			LongText text_D = ModelFactory.GetLongText(nativeWithImageAndText_D.Artifact.ArtifactID, this._longTextRepository);

			int actualDocumentExportedCount = 0;
			string actualLine = string.Empty;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);
			this._status.Setup(x => x.WriteStatusLine(It.IsAny<EventType2>(), It.IsAny<string>(), It.IsAny<bool>()))
				.Callback((EventType2 eventType, string line, bool isEssential) => actualLine = line);

			// ACT
			this._instance.MarkFileAsCompleted(image1_B.ExportRequest.DestinationLocation, image1_B.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(0));
			Assert.That(actualLine, Does.Contain(string.Empty));

			// 1 downloaded (A)
			this._instance.MarkFileAsCompleted(nativeWithoutImagesOrText_A.ExportRequest.DestinationLocation, nativeWithoutImagesOrText_A.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			this._instance.MarkFileAsCompleted(nativeWithText_C.ExportRequest.DestinationLocation, nativeWithText_C.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			// 2 downloaded (A, C)
			this._instance.MarkLongTextAsCompleted(text_C.ExportRequest.DestinationLocation, text_C.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			this._instance.MarkFileAsCompleted(nativeWithTwoImages_B.ExportRequest.DestinationLocation, nativeWithTwoImages_B.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			// 3 downloaded (A, C, B)
			this._instance.MarkFileAsCompleted(image2_B.ExportRequest.DestinationLocation, image2_B.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			this._instance.MarkLongTextAsCompleted(text_D.ExportRequest.DestinationLocation, text_D.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			this._instance.MarkFileAsCompleted(image_D.ExportRequest.DestinationLocation, image_D.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			// 4 download (A, C, B, D)
			this._instance.MarkFileAsCompleted(nativeWithImageAndText_D.ExportRequest.DestinationLocation, nativeWithImageAndText_D.ExportRequest.Order, true);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));

			// None downloaded - just an error.
			this._instance.MarkArtifactAsError(999, "error message");
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));

			// Handle duplicates
			this._instance.MarkArtifactAsError(999, "error message");
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));
		}

		[Test]
		public void ItShouldCountDuplicateSourceNatives()
		{
			// ARRANGE
			const string SourceLocation = @"\\files\T001\Files\EDDS123456\8B845660-CBD8-456E-BDB9-15BFDB33BD7D";
			Native native1 = ModelFactory.GetNative(this._nativeRepository, SourceLocation, @"C:\temp\REL-0001.doc");
			Native native2 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToLowerInvariant(), @"C:\temp\REL-0002.doc");
			Native native3 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToUpperInvariant(), @"C:\temp\REL-0003.doc");
			Native native4 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToLowerInvariant() + " ", @"C:\temp\REL-0004.doc");
			Native native5 = ModelFactory.GetNative(this._nativeRepository, SourceLocation.ToUpperInvariant() + " ", @"C:\temp\REL-0005.doc");
			int actualDocumentExportedCount = 0;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);

			// ACT
			foreach (Native native in new[] { native1, native2, native3, native4, native5 })
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
			Native native = ModelFactory.GetNative(this._nativeRepository);
			Image image1 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation, @"C:\temp\file1.tiff");
			Image image2 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToLowerInvariant(), @"C:\temp\file2.tiff");
			Image image3 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToUpperInvariant(), @"C:\temp\file3.tiff");
			Image image4 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToLowerInvariant() + " ", @"C:\temp\file4.tiff");
			Image image5 = ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID, SourceLocation.ToUpperInvariant() + " ", @"C:\temp\file5.tiff");
			int actualDocumentExportedCount = 0;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);
			this._instance.MarkFileAsCompleted(
				native.ExportRequest.DestinationLocation,
				native.ExportRequest.Order,
				true);

			// ACT
			foreach (Image image in new[] { image1, image2, image3, image4, image5 })
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
			Native nativeWithImage = ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetImage(this._imageRepository, nativeWithImage.Artifact.ArtifactID);
			ModelFactory.GetImage(this._imageRepository, nativeWithImage.Artifact.ArtifactID);
			Native nativeWithText = ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetLongText(nativeWithText.Artifact.ArtifactID, this._longTextRepository);
			ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetNative(this._nativeRepository);
			int actualDocumentExportedCount = 0;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);

			// ACT
			this._instance.FinalizeBatchProcessedCount();

			// ASSERT
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
		}
	}
}