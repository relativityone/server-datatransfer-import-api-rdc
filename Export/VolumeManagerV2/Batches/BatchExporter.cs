using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class BatchExporter : IBatchExporter
	{
		private readonly LoadFileMetadataBuilder _loadFileMetadataBuilder;
		private readonly IImageLoadFileMetadataBuilder _imageLoadFileMetadataBuilder;
		private readonly ILoadFileWriter _loadFileWriter;
		private readonly IImageLoadFileWriter _imageLoadFileWriter;
		private readonly FilesDownloader _filesDownloader;
		private readonly IImagesRollupManager _imagesRollupManager;
		private readonly IMessenger _messenger;

		public BatchExporter(LoadFileMetadataBuilder loadFileMetadataBuilder, IImageLoadFileMetadataBuilder imageLoadFileMetadataBuilder, ILoadFileWriter loadFileWriter,
			IImageLoadFileWriter imageLoadFileWriter, FilesDownloader filesDownloader, IImagesRollupManager imagesRollupManager, IMessenger messenger)
		{
			_loadFileMetadataBuilder = loadFileMetadataBuilder;
			_imageLoadFileMetadataBuilder = imageLoadFileMetadataBuilder;
			_loadFileWriter = loadFileWriter;
			_imageLoadFileWriter = imageLoadFileWriter;
			_filesDownloader = filesDownloader;
			_imagesRollupManager = imagesRollupManager;
			_messenger = messenger;
		}

		public void Export(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			_filesDownloader.DownloadFilesForArtifacts(artifacts, volumePredictions, cancellationToken);

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

			IList<KeyValuePair<string, string>> entries = _imageLoadFileMetadataBuilder.CreateLoadFileEntries(artifacts, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_imageLoadFileWriter.Write(entries, artifacts, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_messenger.CreatingLoadFileMetadata();

			IDictionary<int, ILoadFileEntry> loadFileEntries = _loadFileMetadataBuilder.AddLines(artifacts, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_loadFileWriter.Write(loadFileEntries, artifacts, cancellationToken);
		}
	}
}