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

		private Mock<IStatus> _status;

		private static int _artifactId = 1;

		[SetUp]
		public void SetUp()
		{
			_nativeRepository = new NativeRepository();
			_imageRepository = new ImageRepository();
			_longTextRepository = new LongTextRepository(null, new NullLogger());

			_status = new Mock<IStatus>();

			_instance = new DownloadProgressManager(_nativeRepository, _imageRepository, _longTextRepository, _status.Object, new NullLogger());
		}

		[Test]
		public void ItShouldSaveAndRestoreState()
		{
			Native native1 = ModelFactory.GetNative(_nativeRepository);
			Native native2 = ModelFactory.GetNative(_nativeRepository);

			int actualDocumentExportedCount = 0;

			_status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int x) => actualDocumentExportedCount = x);

			//ACT
			_instance.MarkNativeAsDownloaded(native1.ExportRequest.UniqueId);

			_instance.SaveState();

			_instance.MarkNativeAsDownloaded(native2.ExportRequest.UniqueId);

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
			_status.Setup(x => x.UpdateDocumentExportedCount(It.IsAny<int>())).Callback((int docs) => actualDocumentExportedCount = docs);

			//ACT

			_instance.MarkImageAsDownloaded(image1_B.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(0));

			// 1 downloaded (A)
			_instance.MarkNativeAsDownloaded(nativeWithoutImagesOrText_A.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			_instance.MarkNativeAsDownloaded(nativeWithText_C.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));

			// 2 downloaded (A, C)
			_instance.MarkLongTextAsDownloaded(text_C.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			_instance.MarkNativeAsDownloaded(nativeWithTwoImages_B.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));

			// 3 downloaded (A, C, B)
			_instance.MarkImageAsDownloaded(image2_B.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			_instance.MarkLongTextAsDownloaded(text_D.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			_instance.MarkImageAsDownloaded(image_D.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));

			// 4 download (A, C, B, D)
			_instance.MarkNativeAsDownloaded(nativeWithImageAndText_D.ExportRequest.UniqueId);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
		}
	}
}