using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
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
		private IExportRequestRetriever _exportRequestRetriever;

		private Mock<IErrorFileWriter> _errorFileWriter;
		private Mock<IPhysicalFilesDownloader> _physicalFilesDownloader;
		private Mock<ILongTextDownloader> _longTextDownloader;

		[SetUp]
		public void SetUp()
		{
			_nativeRepository = new NativeRepository();
			_imageRepository = new ImageRepository();
			_longTextRepository = new LongTextRepository(null, new NullLogger());
			_exportRequestRetriever = new ExportRequestRetriever(_nativeRepository, _imageRepository, _longTextRepository);

			_physicalFilesDownloader = new Mock<IPhysicalFilesDownloader>();
			_longTextDownloader = new Mock<ILongTextDownloader>();

			_errorFileWriter = new Mock<IErrorFileWriter>();
			_instance = new Downloader(_exportRequestRetriever, _physicalFilesDownloader.Object, _longTextDownloader.Object, _errorFileWriter.Object, new NullLogger());
		}

		[Test]
		public async Task GoldWorkflow()
		{
			Native native = ModelFactory.GetNative(_nativeRepository);
			ModelFactory.GetImage(native.Artifact.ArtifactID, _imageRepository);
			ModelFactory.GetImage(native.Artifact.ArtifactID, _imageRepository);
			ModelFactory.GetLongText(native.Artifact.ArtifactID, _longTextRepository);

			//ACT
			_instance.DownloadFilesForArtifacts(CancellationToken.None);

			//ASSERT
			_physicalFilesDownloader.Verify(x => x.DownloadFilesAsync(It.Is<List<ExportRequest>>(list => list.Count == 3), CancellationToken.None));
			_longTextDownloader.Verify(x => x.DownloadAsync(It.Is<List<LongTextExportRequest>>(list => list.Count == 1), CancellationToken.None), Times.Once);
		}

		[Test]
		public void ItShouldWriteErrorWhenTransferExceptionOccurred()
		{
			_physicalFilesDownloader.Setup(x => x.DownloadFilesAsync(It.IsAny<List<ExportRequest>>(), CancellationToken.None)).Throws<TransferException>();

			//ACT & ASSERT
			Assert.Throws<TransferException>(() => _instance.DownloadFilesForArtifacts(CancellationToken.None));

			_errorFileWriter.Verify(x => x.Write(ErrorFileWriter.ExportFileType.Generic, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
		}
	}
}