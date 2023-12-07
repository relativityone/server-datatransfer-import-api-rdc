// -----------------------------------------------------------------------------------------------------
// <copyright file="DownloaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Transfer;

	[TestFixture]
	public class DownloaderTests
	{
		private Downloader _instance;
		private FileRequestRepository _nativeRepository;
		private ImageRepository _imageRepository;
		private LongTextRepository _longTextRepository;
		private FileRequestRepository _pdfRepository;
		private IExportRequestRetriever _exportRequestRetriever;

		private Mock<IErrorFileWriter> _errorFileWriter;
		private Mock<IPhysicalFilesDownloader> _physicalFilesDownloader;

		private Mock<ILongTextDownloader> _longTextDownloader;
		private Mock<ILongTextFileDownloadSubscriber> _longTextFileDownloadSubscriber;
		private Mock<IFileDownloadSubscriber> _fileDownloadSubscriber;

		[SetUp]
		public void SetUp()
		{
			this._nativeRepository = new FileRequestRepository();
			this._imageRepository = new ImageRepository();
			this._longTextRepository = new LongTextRepository(null, new TestNullLogger());
			this._pdfRepository = new FileRequestRepository();
			this._exportRequestRetriever = new ExportRequestRetriever(this._nativeRepository, this._imageRepository, this._longTextRepository, this._pdfRepository);

			this._physicalFilesDownloader = new Mock<IPhysicalFilesDownloader>();
			this._longTextDownloader = new Mock<ILongTextDownloader>();
			this._longTextFileDownloadSubscriber = new Mock<ILongTextFileDownloadSubscriber>();
			this._fileDownloadSubscriber = new Mock<IFileDownloadSubscriber>();

			this._errorFileWriter = new Mock<IErrorFileWriter>();
			this._instance = new Downloader(
				this._exportRequestRetriever,
				this._physicalFilesDownloader.Object,
				this._longTextDownloader.Object,
				this._longTextFileDownloadSubscriber.Object,
				this._errorFileWriter.Object,
				this._fileDownloadSubscriber.Object,
				new TestNullLogger());
		}

		[Test]
		public async Task GoldWorkflow()
		{
			FileRequest<ObjectExportInfo> native = ModelFactory.GetNative(this._nativeRepository);
			ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID);
			ModelFactory.GetImage(this._imageRepository, native.Artifact.ArtifactID);
			ModelFactory.GetLongText(native.Artifact.ArtifactID, this._longTextRepository);

			// ACT
			await this._instance.DownloadFilesForArtifactsAsync(CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			this._physicalFilesDownloader.Verify(x => x.DownloadFilesAsync(It.Is<List<ExportRequest>>(list => list.Count == 3), CancellationToken.None));
			this._longTextDownloader.Verify(x => x.DownloadAsync(It.Is<List<LongTextExportRequest>>(list => list.Count == 1), CancellationToken.None), Times.Once);
		}

		[Test]
		public async Task ItShouldDownloadPdfFiles()
		{
			ModelFactory.GetPdf(this._pdfRepository, 1);
			ModelFactory.GetPdf(this._pdfRepository, 2);

			// ACT
			await this._instance.DownloadFilesForArtifactsAsync(CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			this._physicalFilesDownloader.Verify(x => x.DownloadFilesAsync(It.Is<List<ExportRequest>>(list => list.Count == 2), CancellationToken.None));
		}

		[Test]
		public void ItShouldWriteErrorWhenTransferExceptionOccurred()
		{
			this._physicalFilesDownloader.Setup(x => x.DownloadFilesAsync(It.IsAny<List<ExportRequest>>(), CancellationToken.None)).Throws<TransferException>();

			// ACT & ASSERT
			Assert.ThrowsAsync<TransferException>(async () => await this._instance.DownloadFilesForArtifactsAsync(CancellationToken.None).ConfigureAwait(false));

			this._errorFileWriter.Verify(x => x.Write(ErrorFileWriter.ExportFileType.Generic, It.IsAny<ObjectExportInfo>(), It.IsAny<string>(), It.IsAny<string>()));
		}

		[Test]
		public void ItShouldThrowExceptionWhenSelectedNativeFileIsNull()
		{
			// ARRANGE
			this._nativeRepository = null;

			// ACT and ASSERT
			Assert.Throws<ArgumentNullException>(
				() => ModelFactory.GetNative(this._nativeRepository, "sourceLocation", "targetFile",0));
		}

		[Test]
		public void ItShouldThrowExceptionWhenSelectedPdfFileIsNull()
		{
			// ARRANGE
			this._pdfRepository = null;

			// ACT and ASSERT
			Assert.Throws<ArgumentNullException>(
				() => ModelFactory.GetPdf(this._pdfRepository, 1, "sourceLocation", "targetFile"));
		}

	}
}