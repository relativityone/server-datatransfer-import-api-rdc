﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;

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

		public BatchExporter(IDownloader downloader, IImagesRollupManager imagesRollupManager,
			IMessenger messenger, IImageLoadFile imageLoadFile, ILoadFile loadFile, ILongTextEncodingConverterFactory longTextEncodingConverterFactory)
		{
			_downloader = downloader;
			_imagesRollupManager = imagesRollupManager;
			_messenger = messenger;
			_imageLoadFile = imageLoadFile;
			_loadFile = loadFile;
		}

		public void Export(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			this._downloader.DownloadFilesForArtifacts(cancellationToken);

			_messenger.FilesDownloadCompleted();

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