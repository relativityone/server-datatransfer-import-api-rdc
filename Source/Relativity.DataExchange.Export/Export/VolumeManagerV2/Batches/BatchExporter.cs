namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.ImagesRollup;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Natives;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;

	public class BatchExporter : IBatchExporter
	{
		private readonly IDownloader _downloader;
		private readonly IImagesRollupManager _imagesRollupManager;
		private readonly IMessenger _messenger;
		private readonly IImageLoadFile _imageLoadFile;
		private readonly ILoadFile _loadFile;

		private readonly IFileDownloadSubscriber _fileDownloadSubscriber;

		public BatchExporter(IDownloader downloader, IImagesRollupManager imagesRollupManager,
			IMessenger messenger, IImageLoadFile imageLoadFile, ILoadFile loadFile, IFileDownloadSubscriber fileDownloadSubscriber)
		{
			_downloader = downloader;
			_imagesRollupManager = imagesRollupManager;
			_messenger = messenger;
			_imageLoadFile = imageLoadFile;
			_loadFile = loadFile;
			_fileDownloadSubscriber = fileDownloadSubscriber;
		}

		public async Task ExportAsync(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			await this._downloader.DownloadFilesForArtifactsAsync(cancellationToken).ConfigureAwait(false);

			_messenger.FilesDownloadCompleted();

			await _fileDownloadSubscriber.WaitForConversionCompletion().ConfigureAwait(false);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_messenger.StartingRollupImages();

			_imagesRollupManager.RollupImagesForArtifacts(artifacts, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_messenger.CreatingImageLoadFileMetadata();

			_imageLoadFile.Create(artifacts, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_messenger.CreatingLoadFileMetadata();

			_loadFile.Create(artifacts, cancellationToken);
		}
	}
}