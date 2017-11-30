using System;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2
{
	public class BatchExporter : IBatchExporter
	{
		private readonly FilesDownloader _filesDownloader;
		private readonly IImagesRollup _imagesRollup;

		public BatchExporter(FilesDownloader filesDownloader, IImagesRollup imagesRollup)
		{
			_filesDownloader = filesDownloader;
			_imagesRollup = imagesRollup;
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
		}
	}
}