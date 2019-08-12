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
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Process;
	using Relativity.Logging;

	[TestFixture]
	public class DownloadProgressManagerTests
	{
		private DownloadProgressManager _instance;

		private NativeRepository _nativeRepository;
		private ImageRepository _imageRepository;
		private LongTextRepository _longTextRepository;

	    private Mock<IFile> _fileHelper;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			this._nativeRepository = new NativeRepository();
			this._imageRepository = new ImageRepository();
			this._longTextRepository = new LongTextRepository(null, new NullLogger());

            this._fileHelper = new Mock<IFile>();
			this._status = new Mock<IStatus>();

			this._instance = new DownloadProgressManager(this._nativeRepository, this._imageRepository, this._longTextRepository, this._fileHelper.Object, this._status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSaveAndRestoreState()
		{
			Native native1 = ModelFactory.GetNative(this._nativeRepository);
			Native native2 = ModelFactory.GetNative(this._nativeRepository);

			int actualDocumentExportedCount = 0;

			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int x) => actualDocumentExportedCount = x);

			// ACT
			this._instance.MarkFileAsDownloaded(native1.ExportRequest.FileName, native1.ExportRequest.Order);

			this._instance.SaveState();

			this._instance.MarkFileAsDownloaded(native2.ExportRequest.FileName, native2.ExportRequest.Order);

			this._instance.RestoreLastState();

			// ASSERT
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
		}

		[Test]
		public void ItShouldUpdateProgress()
		{
			// DATA SET
			Native nativeWithoutImagesOrText_A = ModelFactory.GetNative(this._nativeRepository);

			Native nativeWithTwoImages_B = ModelFactory.GetNative(this._nativeRepository);
			Image image1_B = ModelFactory.GetImage(nativeWithTwoImages_B.Artifact.ArtifactID, this._imageRepository);
			Image image2_B = ModelFactory.GetImage(nativeWithTwoImages_B.Artifact.ArtifactID, this._imageRepository);

			Native nativeWithText_C = ModelFactory.GetNative(this._nativeRepository);
			LongText text_C = ModelFactory.GetLongText(nativeWithText_C.Artifact.ArtifactID, this._longTextRepository);

			Native nativeWithImageAndText_D = ModelFactory.GetNative(this._nativeRepository);
			Image image_D = ModelFactory.GetImage(nativeWithImageAndText_D.Artifact.ArtifactID, this._imageRepository);
			LongText text_D = ModelFactory.GetLongText(nativeWithImageAndText_D.Artifact.ArtifactID, this._longTextRepository);

			int actualDocumentExportedCount = 0;
			string actualLine = string.Empty;
			this._status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);
			this._status.Setup(x => x.WriteStatusLine(It.IsAny<EventType2>(), It.IsAny<string>(), It.IsAny<bool>()))
				.Callback((EventType2 eventType, string line, bool isEssential) => actualLine = line);

			// ACT
			this._instance.MarkFileAsDownloaded(image1_B.ExportRequest.FileName, image1_B.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(0));
			Assert.That(actualLine, Does.Contain(string.Empty));

			// 1 downloaded (A)
			this._instance.MarkFileAsDownloaded(nativeWithoutImagesOrText_A.ExportRequest.FileName, nativeWithoutImagesOrText_A.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			this._instance.MarkFileAsDownloaded(nativeWithText_C.ExportRequest.FileName, nativeWithText_C.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			// 2 downloaded (A, C)
			this._instance.MarkLongTextAsDownloaded(text_C.ExportRequest.FileName, text_C.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			this._instance.MarkFileAsDownloaded(nativeWithTwoImages_B.ExportRequest.FileName, nativeWithTwoImages_B.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			// 3 downloaded (A, C, B)
			this._instance.MarkFileAsDownloaded(image2_B.ExportRequest.FileName, image2_B.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			this._instance.MarkLongTextAsDownloaded(text_D.ExportRequest.FileName, text_D.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			this._instance.MarkFileAsDownloaded(image_D.ExportRequest.FileName, image_D.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			// 4 download (A, C, B, D)
			this._instance.MarkFileAsDownloaded(nativeWithImageAndText_D.ExportRequest.FileName, nativeWithImageAndText_D.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));

			// None downloaded - just an error.
			this._instance.MarkArtifactAsError(999, "error message");
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));

			// Handle duplicates
			this._instance.MarkArtifactAsError(999, "error message");
			Assert.That(actualDocumentExportedCount, Is.EqualTo(5));
		}
	}
}