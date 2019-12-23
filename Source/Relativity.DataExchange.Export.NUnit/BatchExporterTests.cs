// -----------------------------------------------------------------------------------------------------
// <copyright file="BatchExporterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Batches;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;

	[TestFixture]
	public class BatchExporterTests
	{
		private BatchExporter _instance;
		private Mock<IDownloader> _downloader;
		private Mock<IImagesRollupManager> _imagesRollupManager;
		private Mock<IImageLoadFile> _imageLoadFile;
		private Mock<ILoadFile> _loadFile;

		private Mock<IFileDownloadSubscriber> _fileDownloadSubscriber;

		[SetUp]
		public void SetUp()
		{
			this._downloader = new Mock<IDownloader>();
			this._imagesRollupManager = new Mock<IImagesRollupManager>();
			this._imageLoadFile = new Mock<IImageLoadFile>();
			this._loadFile = new Mock<ILoadFile>();
			this._fileDownloadSubscriber = new Mock<IFileDownloadSubscriber>();
			this._instance = new BatchExporter(
				this._downloader.Object, this._imagesRollupManager.Object, new Mock<IMessenger>().Object, this._imageLoadFile.Object, this._loadFile.Object, this._fileDownloadSubscriber.Object);
		}

		[Test]
		public async Task GoldWorkflow()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];

			// ACT
			await this._instance.ExportAsync(artifacts, CancellationToken.None).ConfigureAwait(false);

			// ASSERT
			this._downloader.Verify(x => x.DownloadFilesForArtifactsAsync(CancellationToken.None), Times.Once);
			this._imagesRollupManager.Verify(x => x.RollupImagesForArtifacts(artifacts, CancellationToken.None), Times.Once);
			this._imageLoadFile.Verify(x => x.Create(artifacts, CancellationToken.None), Times.Once);
			this._loadFile.Verify(x => x.Create(artifacts, CancellationToken.None), Times.Once);
			this._fileDownloadSubscriber.Verify(x => x.WaitForConversionCompletion(), Times.Once);
		}
	}
}