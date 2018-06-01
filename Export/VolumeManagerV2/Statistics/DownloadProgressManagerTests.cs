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
			_instance.MarkNativeAsDownloaded(native1.ExportRequest.UniqueId, 1);

			_instance.SaveState();

			_instance.MarkNativeAsDownloaded(native2.ExportRequest.UniqueId, 2);

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

			_instance.MarkImageAsDownloaded(image1_B.ExportRequest.UniqueId, 1);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(0));
			Assert.That(actualLine, Does.Contain(string.Empty));

			// 1 downloaded (A)
			_instance.MarkNativeAsDownloaded(nativeWithoutImagesOrText_A.ExportRequest.UniqueId, 1);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
			Assert.That(actualLine, Does.Contain("line number: 1"));

			_instance.MarkNativeAsDownloaded(nativeWithText_C.ExportRequest.UniqueId, 2);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(1));
			Assert.That(actualLine, Does.Contain("line number: 1"));

			// 2 downloaded (A, C)
			_instance.MarkLongTextAsDownloaded(text_C.ExportRequest.UniqueId, 3);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));
			Assert.That(actualLine, Does.Contain("line number: 3"));

			_instance.MarkNativeAsDownloaded(nativeWithTwoImages_B.ExportRequest.UniqueId, 4);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(2));
			Assert.That(actualLine, Does.Contain("line number: 3"));

			// 3 downloaded (A, C, B)
			_instance.MarkImageAsDownloaded(image2_B.ExportRequest.UniqueId, 5);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualLine, Does.Contain("line number: 5"));

			_instance.MarkLongTextAsDownloaded(text_D.ExportRequest.UniqueId, 6);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualLine, Does.Contain("line number: 5"));

			_instance.MarkImageAsDownloaded(image_D.ExportRequest.UniqueId, 7);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(3));
			Assert.That(actualLine, Does.Contain("line number: 5"));

			// 4 download (A, C, B, D)
			_instance.MarkNativeAsDownloaded(nativeWithImageAndText_D.ExportRequest.UniqueId, 8);
			Assert.That(actualDocumentExportedCount, Is.EqualTo(4));
			Assert.That(actualLine, Does.Contain("line number: 8"));
		}
	}
}