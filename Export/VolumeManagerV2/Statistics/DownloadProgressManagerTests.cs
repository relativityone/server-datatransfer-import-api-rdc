using System;
using System.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
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
			Native native1 = GetNative();
			Native native2 = GetNative();

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
			Native nativeWithoutImagesOrText_A = GetNative();

			Native nativeWithTwoImages_B = GetNative();
			Image image1_B = GetImage(nativeWithTwoImages_B.Artifact.ArtifactID);
			Image image2_B = GetImage(nativeWithTwoImages_B.Artifact.ArtifactID);

			Native nativeWithText_C = GetNative();
			LongText text_C = GetLongText(nativeWithText_C.Artifact.ArtifactID);

			Native nativeWithImageAndText_D = GetNative();
			Image image_D = GetImage(nativeWithImageAndText_D.Artifact.ArtifactID);
			LongText text_D = GetLongText(nativeWithImageAndText_D.Artifact.ArtifactID);
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

		private Native GetNative()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = _artifactId++
			};
			ExportRequest exportRequest = new NativeFileExportRequest(artifact, "")
			{
				UniqueId = Guid.NewGuid().ToString()
			};
			Native native = new Native(artifact)
			{
				HasBeenDownloaded = false,
				ExportRequest = exportRequest
			};
			_nativeRepository.Add(native.InList());
			return native;
		}

		private Image GetImage(int artifactId)
		{
			ImageExportInfo artifact = new ImageExportInfo
			{
				ArtifactID = artifactId
			};
			ExportRequest exportRequest = new NativeFileExportRequest(artifact, "")
			{
				UniqueId = Guid.NewGuid().ToString()
			};
			Image image = new Image(artifact)
			{
				HasBeenDownloaded = false,
				ExportRequest = exportRequest
			};
			_imageRepository.Add(image.InList());
			return image;
		}

		private LongText GetLongText(int artifactId)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				ArtifactID = artifactId
			};
			LongTextExportRequest exportRequest = LongTextExportRequest.CreateRequestForLongText(artifact, 1, "");
			exportRequest.UniqueId = Guid.NewGuid().ToString();
			LongText longText = LongText.CreateFromMissingValue(artifactId, 1, exportRequest, Encoding.Unicode);
			_longTextRepository.Add(longText.InList());
			return longText;
		}
	}
}