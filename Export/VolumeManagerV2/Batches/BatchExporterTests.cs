using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Batches
{
	[TestFixture]
	public class BatchExporterTests
	{
		private BatchExporter _instance;
		private Mock<IDownloader> _downloader;
		private Mock<IImagesRollupManager> _imagesRollupManager;
		private Mock<IImageLoadFile> _imageLoadFile;
		private Mock<ILoadFile> _loadFile;

		[SetUp]
		public void SetUp()
		{
			_downloader = new Mock<IDownloader>();
			_imagesRollupManager = new Mock<IImagesRollupManager>();
			_imageLoadFile = new Mock<IImageLoadFile>();
			_loadFile = new Mock<ILoadFile>();

			_instance = new BatchExporter(_downloader.Object, _imagesRollupManager.Object, new Mock<IMessenger>().Object, _imageLoadFile.Object, _loadFile.Object);
		}

		[Test]
		public void GoldWorkflow()
		{
			ObjectExportInfo[] artifacts = new ObjectExportInfo[1];

			//ACT
			_instance.Export(artifacts, CancellationToken.None);

			//ASSERT
			_downloader.Verify(x => x.DownloadFilesForArtifacts(CancellationToken.None), Times.Once);
			_imagesRollupManager.Verify(x => x.RollupImagesForArtifacts(artifacts, CancellationToken.None), Times.Once);
			_imageLoadFile.Verify(x => x.Create(artifacts, CancellationToken.None), Times.Once);
			_loadFile.Verify(x => x.Create(artifacts, CancellationToken.None), Times.Once);
		}
	}
}