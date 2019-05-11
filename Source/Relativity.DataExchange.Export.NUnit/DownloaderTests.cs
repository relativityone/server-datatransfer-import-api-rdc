// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;
	using Relativity.Transfer;

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
			this._nativeRepository = new NativeRepository();
			this._imageRepository = new ImageRepository();
			this._longTextRepository = new LongTextRepository(null, new NullLogger());
			this._exportRequestRetriever = new ExportRequestRetriever(this._nativeRepository, this._imageRepository, this._longTextRepository);

			this._physicalFilesDownloader = new Mock<IPhysicalFilesDownloader>();
			this._longTextDownloader = new Mock<ILongTextDownloader>();

			this._errorFileWriter = new Mock<IErrorFileWriter>();
			this._instance = new Downloader(this._exportRequestRetriever, this._physicalFilesDownloader.Object, this._longTextDownloader.Object, this._errorFileWriter.Object, new NullLogger());
		}

		[Test]
		public void GoldWorkflow()
		{
			Native native = ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetImage(native.Artifact.ArtifactID, this._imageRepository);
			ModelFactory.GetImage(native.Artifact.ArtifactID, this._imageRepository);
			ModelFactory.GetLongText(native.Artifact.ArtifactID, this._longTextRepository);

			// ACT
			this._instance.DownloadFilesForArtifacts(CancellationToken.None);

			// ASSERT
			this._physicalFilesDownloader.Verify(x => x.DownloadFilesAsync(It.Is<List<ExportRequest>>(list => list.Count == 3), CancellationToken.None));
			this._longTextDownloader.Verify(x => x.DownloadAsync(It.Is<List<LongTextExportRequest>>(list => list.Count == 1), CancellationToken.None), Times.Once);
		}

		[Test]
		public void ItShouldWriteErrorWhenTransferExceptionOccurred()
		{
			this._physicalFilesDownloader.Setup(x => x.DownloadFilesAsync(It.IsAny<List<ExportRequest>>(), CancellationToken.None)).Throws<TransferException>();

			// ACT & ASSERT
			Assert.Throws<TransferException>(() => this._instance.DownloadFilesForArtifacts(CancellationToken.None));

			this._errorFileWriter.Verify(x => x.Write(ErrorFileWriter.ExportFileType.Generic, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()));
		}
	}
}