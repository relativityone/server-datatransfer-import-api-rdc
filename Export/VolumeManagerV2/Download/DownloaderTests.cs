using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
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
		private Mock<IPhysicalFilesDownloader> _physicalFilesDownloader;

		[SetUp]
		public void SetUp()
		{
			_nativeRepository = new NativeRepository();
			_imageRepository = new ImageRepository();
			_longTextRepository = new LongTextRepository(null, new NullLogger());

			_fileBridge = new Mock<IDownloadTapiBridge>();
			_textBridge = new Mock<IDownloadTapiBridge>();
			_physicalFilesDownloader = new Mock<IPhysicalFilesDownloader>();

			Mock<IExportTapiBridgeFactory> exportTapiBridgeFactory = new Mock<IExportTapiBridgeFactory>();
			exportTapiBridgeFactory.Setup(x => x.CreateForFiles(It.IsAny<RelativityFileShareSettings>(), CancellationToken.None)).Returns(_fileBridge.Object);
			exportTapiBridgeFactory.Setup(x => x.CreateForLongText(CancellationToken.None)).Returns(_textBridge.Object);

			_errorFileWriter = new Mock<IErrorFileWriter>();
			_instance = new Downloader(_nativeRepository, _imageRepository, _longTextRepository, _physicalFilesDownloader.Object,
				new SafeIncrement(), exportTapiBridgeFactory.Object, _errorFileWriter.Object,
				new NullLogger());
		}

		[Test]
		public async Task GoldWorkflow()
		{
			Native native = ModelFactory.GetNative(_nativeRepository);
			ModelFactory.GetImage(native.Artifact.ArtifactID, _imageRepository);
			ModelFactory.GetImage(native.Artifact.ArtifactID, _imageRepository);
			LongText longText = ModelFactory.GetLongText(native.Artifact.ArtifactID, _longTextRepository);

			const string textUniqueID = "text_unique_id";

			_textBridge.Setup(x => x.QueueDownload(It.IsAny<TransferPath>())).Returns(textUniqueID);

			//ACT
			_instance.DownloadFilesForArtifacts(CancellationToken.None);

			//ASSERT
			_physicalFilesDownloader.Verify(x => x.DownloadFilesAsync(It.Is<List<ExportRequest>>(list => list.Count == 3), CancellationToken.None));
			_textBridge.Verify(x => x.QueueDownload(It.IsAny<TransferPath>()), Times.Once);

			_textBridge.Verify(x => x.WaitForTransferJob(), Times.Once);

			_textBridge.Verify(x => x.Dispose(), Times.Once);

			Assert.That(longText.ExportRequest.FileName, Is.EqualTo(textUniqueID));
		}

		[Test]
		public void ItShouldWriteErrorWhenTransferExceptionOccurred()
		{
			_textBridge.Setup(x => x.WaitForTransferJob()).Throws<TransferException>();

			//ACT & ASSERT
			Assert.Throws<TransferException>(() => _instance.DownloadFilesForArtifacts(CancellationToken.None));

			_errorFileWriter.Verify(x => x.Write(ErrorFileWriter.ExportFileType.Generic, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
		}

		[Test]
		public void ItShouldAlwaysDisposeTapiBridges()
		{
			_textBridge.Setup(x => x.WaitForTransferJob()).Throws<Exception>();

			//ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.DownloadFilesForArtifacts(CancellationToken.None));

			_textBridge.Verify(x => x.Dispose(), Times.Once);
		}
	}
}