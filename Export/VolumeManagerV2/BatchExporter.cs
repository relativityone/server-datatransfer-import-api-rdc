using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class BatchExporter : IBatchExporter
	{
		private readonly LoadFileMetadataBuilder _loadFileMetadataBuilder;
		private readonly IImageLoadFileMetadataBuilder _imageLoadFileMetadataBuilder;
		private readonly ILoadFileWriter _loadFileWriter;
		private readonly IImageLoadFileWriter _imageLoadFileWriter;
		private readonly FilesDownloader _filesDownloader;
		private readonly IImagesRollupManager _imagesRollupManager;
		private readonly IBatchInitialization _batchInitialization;
		private readonly IBatchCleanUp _batchCleanUp;
		private readonly IBatchValidator _batchValidator;
		private readonly IBatchState _batchState;

		public BatchExporter(FilesDownloader filesDownloader, IImagesRollupManager imagesRollupManager, IImageLoadFileMetadataBuilder imageLoadFileMetadataBuilder,
			IImageLoadFileWriter imageLoadFileWriter, LoadFileMetadataBuilder loadFileMetadataBuilder, ILoadFileWriter loadFileWriter, IBatchCleanUp batchCleanUp,
			IBatchInitialization batchInitialization, IBatchValidator batchValidator, IBatchState batchState)
		{
			_filesDownloader = filesDownloader;
			_imagesRollupManager = imagesRollupManager;
			_imageLoadFileMetadataBuilder = imageLoadFileMetadataBuilder;
			_imageLoadFileWriter = imageLoadFileWriter;
			_loadFileMetadataBuilder = loadFileMetadataBuilder;
			_loadFileWriter = loadFileWriter;
			_batchCleanUp = batchCleanUp;
			_batchInitialization = batchInitialization;
			_batchValidator = batchValidator;
			_batchState = batchState;
		}

		public void Export(ObjectExportInfo[] artifacts, object[] records, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			try
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchInitialization.PrepareBatch(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_filesDownloader.DownloadFilesForArtifacts(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_imagesRollupManager.RollupImagesForArtifacts(artifacts, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

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

				IDictionary<int, ILoadFileEntry> loadFileEntries = _loadFileMetadataBuilder.AddLines(artifacts, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_loadFileWriter.Write(loadFileEntries, artifacts, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchValidator.ValidateExportedBatch(artifacts, volumePredictions, cancellationToken);

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_batchState.SaveState();
			}
			finally
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_batchState.RestoreState();
				}
				_batchCleanUp.CleanUp();
			}
		}
	}
}