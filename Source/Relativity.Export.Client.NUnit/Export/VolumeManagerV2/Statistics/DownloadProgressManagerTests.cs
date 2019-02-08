using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Statistics
{
	[TestFixture]
	public class DownloadProgressManagerTests
	{
		private DownloadProgressManager _instance;

		private NativeRepository _nativeRepository;
		private ImageRepository _imageRepository;
		private LongTextRepository _longTextRepository;

	    private Mock<IFileHelper> _fileHelper;
		private Mock<IStatus> _status;

		[SetUp]
		public void SetUp()
		{
			_nativeRepository = new NativeRepository();
			_imageRepository = new ImageRepository();
			_longTextRepository = new LongTextRepository(null, new NullLogger());

            _fileHelper = new Mock<IFileHelper>();
			_status = new Mock<IStatus>();

			_instance = new DownloadProgressManager(_nativeRepository, _imageRepository, _longTextRepository, _fileHelper.Object, _status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSaveAndRestoreState()
		{
			Native native1 = ModelFactory.GetNative(_nativeRepository);
			Native native2 = ModelFactory.GetNative(_nativeRepository);

			int actualDocumentExportedCount = 0;

			_status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int x) => actualDocumentExportedCount = x);

			//ACT
			_instance.MarkFileAsDownloaded(native1.ExportRequest.FileName, native1.ExportRequest.Order);

			_instance.SaveState();

			_instance.MarkFileAsDownloaded(native2.ExportRequest.FileName, native2.ExportRequest.Order);

			_instance.RestoreLastState();

			//ASSERT
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
		}

		[Test]
		public void ItShouldUpdateProgress()
		{
			//DATA SET
			Native nativeWithoutImagesOrText_A = ModelFactory.GetNative(_nativeRepository);

			Native nativeWithTwoImages_B = ModelFactory.GetNative(_nativeRepository);
			Image image1_B = ModelFactory.GetImage(nativeWithTwoImages_B.Artifact.ArtifactID, _imageRepository);
			Image image2_B = ModelFactory.GetImage(nativeWithTwoImages_B.Artifact.ArtifactID, _imageRepository);

			Native nativeWithText_C = ModelFactory.GetNative(_nativeRepository);
			LongText text_C = ModelFactory.GetLongText(nativeWithText_C.Artifact.ArtifactID, _longTextRepository);

			Native nativeWithImageAndText_D = ModelFactory.GetNative(_nativeRepository);
			Image image_D = ModelFactory.GetImage(nativeWithImageAndText_D.Artifact.ArtifactID, _imageRepository);
			LongText text_D = ModelFactory.GetLongText(nativeWithImageAndText_D.Artifact.ArtifactID, _longTextRepository);
			//********

			int actualDocumentExportedCount = 0;
			string actualLine = string.Empty;
			_status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);
			_status.Setup(x => x.WriteStatusLine(It.IsAny<EventType>(), It.IsAny<string>(), It.IsAny<bool>()))
				.Callback((EventType eventType, string line, bool isEssential) => actualLine = line);

			//ACT

			_instance.MarkFileAsDownloaded(image1_B.ExportRequest.FileName, image1_B.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(0));
			Assert.That(actualLine, Does.Contain(string.Empty));

			// 1 downloaded (A)
			_instance.MarkFileAsDownloaded(nativeWithoutImagesOrText_A.ExportRequest.FileName, nativeWithoutImagesOrText_A.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			_instance.MarkFileAsDownloaded(nativeWithText_C.ExportRequest.FileName, nativeWithText_C.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			// 2 downloaded (A, C)
			_instance.MarkLongTextAsDownloaded(text_C.ExportRequest.FileName, text_C.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			_instance.MarkFileAsDownloaded(nativeWithTwoImages_B.ExportRequest.FileName, nativeWithTwoImages_B.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			// 3 downloaded (A, C, B)
			_instance.MarkFileAsDownloaded(image2_B.ExportRequest.FileName, image2_B.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			_instance.MarkLongTextAsDownloaded(text_D.ExportRequest.FileName, text_D.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			_instance.MarkFileAsDownloaded(image_D.ExportRequest.FileName, image_D.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			// 4 download (A, C, B, D)
			_instance.MarkFileAsDownloaded(nativeWithImageAndText_D.ExportRequest.FileName, nativeWithImageAndText_D.ExportRequest.Order);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
		}
	}
}