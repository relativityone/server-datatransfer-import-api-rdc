using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Natives;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.LoadFileEntry;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class BatchExporter : IBatchExporter
	{
		private readonly LoadFileMetadataBuilder _loadFileMetadataBuilder;
		private readonly Metadata.Images.ImageLoadFileMetadataBuilder _imageLoadFileMetadataBuilder;
		private readonly LoadFileWriter _loadFileWriter;
		private readonly ImageLoadFileWriter _imageLoadFileWriter;
		private readonly FilesDownloader _filesDownloader;
		private readonly IImagesRollup _imagesRollup;
		private readonly LongTextRepositoryBuilder _longTextRepositoryBuilder;

		public BatchExporter(FilesDownloader filesDownloader, IImagesRollup imagesRollup, Metadata.Images.ImageLoadFileMetadataBuilder imageLoadFileMetadataBuilder, ImageLoadFileWriter imageLoadFileWriter,
			LoadFileMetadataBuilder loadFileMetadataBuilder, LoadFileWriter loadFileWriter, LongTextRepositoryBuilder longTextRepositoryBuilder)
		{
			_filesDownloader = filesDownloader;
			_imagesRollup = imagesRollup;
			_imageLoadFileMetadataBuilder = imageLoadFileMetadataBuilder;
			_imageLoadFileWriter = imageLoadFileWriter;
			_loadFileMetadataBuilder = loadFileMetadataBuilder;
			_loadFileWriter = loadFileWriter;
			_longTextRepositoryBuilder = longTextRepositoryBuilder;
		}

		public void Export(ObjectExportInfo[] artifacts, object[] records, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			//TODO no way
			foreach (var artifact in artifacts)
			{
				_longTextRepositoryBuilder.AddLongTextForArtifact(artifact);
			}

			try
			{
				_filesDownloader.DownloadFilesForArtifacts(artifacts, volumePredictions, cancellationToken);
			}
			catch (OperationCanceledException ex)
			{
				return;
			}
			catch (Exception ex)
			{
				return;
			}

			foreach (var artifact in artifacts)
			{
				_imagesRollup.RollupImages(artifact);
			}

			IList<KeyValuePair<string, string>> entries = _imageLoadFileMetadataBuilder.CreateLoadFileEntries(artifacts);
			_imageLoadFileWriter.Write(entries, artifacts, cancellationToken);

			IDictionary<int, ILoadFileEntry> loadFileEntries = _loadFileMetadataBuilder.AddLines(artifacts);
			_loadFileWriter.Write(loadFileEntries, artifacts, cancellationToken);
		}
	}
}