using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class BatchExporter : IBatchExporter
	{
		private readonly Metadata.Images.ImageLoadFile _imageLoadFile;
		private readonly ImageLoadFileWriter _imageLoadFileWriter;
		private readonly FilesDownloader _filesDownloader;
		private readonly IImagesRollup _imagesRollup;

		public BatchExporter(FilesDownloader filesDownloader, IImagesRollup imagesRollup, Metadata.Images.ImageLoadFile _imageLoadFile, ImageLoadFileWriter imageLoadFileWriter)
		{
			_filesDownloader = filesDownloader;
			_imagesRollup = imagesRollup;
			this._imageLoadFile = _imageLoadFile;
			_imageLoadFileWriter = imageLoadFileWriter;
		}

		public void Export(ObjectExportInfo[] artifacts, object[] records, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
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

			IList<KeyValuePair<string, string>> entries = _imageLoadFile.CreateLoadFileEntries(artifacts);
			_imageLoadFileWriter.Write(entries, artifacts, cancellationToken);
		}
	}
}