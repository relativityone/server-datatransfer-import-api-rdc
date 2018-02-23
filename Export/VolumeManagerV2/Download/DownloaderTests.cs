using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Moq;
using NUnit.Framework;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download
{
	[TestFixture]
	public class DownloaderTests
	{
		private Downloader _instance;
		private NativeRepository _nativeRepository;
		private ImageRepository _imageRepository;
		private LongTextRepository _longTextRepository;

		private Mock<IErrorFileWriter> _errorFileWriter;
		private Mock<IDownloadTapiBridge> _fileBridge;
		private Mock<IDownloadTapiBridge> _textBridge;

		[SetUp]
		public void SetUp()
		{
			_nativeRepository = new NativeRepository();
			_imageRepository = new ImageRepository();
			_longTextRepository = new LongTextRepository(null, new NullLogger());

			_fileBridge = new Mock<IDownloadTapiBridge>();
			_textBridge = new Mock<IDownloadTapiBridge>();

			Mock<IExportTapiBridgeFactory> exportTapiBridgeFactory = new Mock<IExportTapiBridgeFactory>();
			exportTapiBridgeFactory.Setup(x => x.CreateForFiles(CancellationToken.None)).Returns(_fileBridge.Object);
			exportTapiBridgeFactory.Setup(x => x.CreateForLongText(CancellationToken.None)).Returns(_textBridge.Object);

			_errorFileWriter = new Mock<IErrorFileWriter>();
			_instance = new Downloader(_nativeRepository, _imageRepository, _longTextRepository, exportTapiBridgeFactory.Object, _errorFileWriter.Object, new NullLogger());
		}

		[Test]
		public void GoldWorkflow()
		{
			Native native = ModelFactory.GetNative(_nativeRepository);
			Image image1 = ModelFactory.GetImage(native.Artifact.ArtifactID, _imageRepository);
			Image image2 = ModelFactory.GetImage(native.Artifact.ArtifactID, _imageRepository);
			LongText longText = ModelFactory.GetLongText(native.Artifact.ArtifactID, _longTextRepository);

			const string nativeUniqueID = "native_unique_id";
			const string imageUniqueID = "image_unique_id";
			const string textUniqueID = "text_unique_id";

			_fileBridge.Setup(x => x.AddPath(It.Is<TransferPath>(y => y.Order == native.ExportRequest.Order))).Returns(nativeUniqueID);
			_fileBridge.Setup(x => x.AddPath(It.Is<TransferPath>(y => y.Order == image1.ExportRequest.Order))).Returns(imageUniqueID);
			_fileBridge.Setup(x => x.AddPath(It.Is<TransferPath>(y => y.Order == image2.ExportRequest.Order))).Returns(imageUniqueID);
			_textBridge.Setup(x => x.AddPath(It.IsAny<TransferPath>())).Returns(textUniqueID);

			//ACT
			_instance.DownloadFilesForArtifacts(CancellationToken.None);

			//ASSERT
			_fileBridge.Verify(x => x.AddPath(It.IsAny<TransferPath>()), Times.Exactly(3));
			_textBridge.Verify(x => x.AddPath(It.IsAny<TransferPath>()), Times.Once);

			_fileBridge.Verify(x => x.WaitForTransferJob(), Times.Once);
			_textBridge.Verify(x => x.WaitForTransferJob(), Times.Once);

			_fileBridge.Verify(x => x.Dispose(), Times.Once);
			_textBridge.Verify(x => x.Dispose(), Times.Once);

			Assert.That(native.ExportRequest.UniqueId, Is.EqualTo(nativeUniqueID));
			Assert.That(image1.ExportRequest.UniqueId, Is.EqualTo(imageUniqueID));
			Assert.That(image2.ExportRequest.UniqueId, Is.EqualTo(imageUniqueID));
			Assert.That(longText.ExportRequest.UniqueId, Is.EqualTo(textUniqueID));
		}

		[Test]
		public void ItShouldWriteErrorWhenTransferExceptionOccurred()
		{
			_fileBridge.Setup(x => x.WaitForTransferJob()).Throws<TransferException>();

			//ACT & ASSERT
			Assert.Throws<TransferException>(() => _instance.DownloadFilesForArtifacts(CancellationToken.None));

			_errorFileWriter.Verify(x => x.Write(ErrorFileWriter.ExportFileType.Generic, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
		}

		[Test]
		public void ItShouldAlwaysDisposeTapiBridges()
		{
			_fileBridge.Setup(x => x.WaitForTransferJob()).Throws<Exception>();
			_fileBridge.Setup(x => x.Dispose()).Throws<Exception>();
			_textBridge.Setup(x => x.WaitForTransferJob()).Throws<Exception>();

			//ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.DownloadFilesForArtifacts(CancellationToken.None));

			_fileBridge.Verify(x => x.Dispose(), Times.Once);
			_textBridge.Verify(x => x.Dispose(), Times.Once);
		}
	}
}