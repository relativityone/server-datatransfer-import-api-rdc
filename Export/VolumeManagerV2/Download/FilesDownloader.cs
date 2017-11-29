using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Requests;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FilesDownloader
	{
		private readonly NativeExportRequestBuilder _nativeExportRequestBuilder;
		private readonly ImageExportRequestBuilder _imageExportRequestBuilder;

		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;

		private readonly ILog _logger;

		public FilesDownloader(NativeExportRequestBuilder nativeExportRequestBuilder, ImageExportRequestBuilder imageExportRequestBuilder,
			ExportTapiBridgeFactory exportTapiBridgeFactory, ILog logger)
		{
			_nativeExportRequestBuilder = nativeExportRequestBuilder;
			_imageExportRequestBuilder = imageExportRequestBuilder;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_logger = logger;
		}

		public void DownloadFilesForArtifacts(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			List<ExportRequest> exportRequests = CreateExportRequests(artifacts);

			if (exportRequests.Count == 0 || cancellationToken.IsCancellationRequested)
			{
				return;
			}

			DownloadFiles(cancellationToken, exportRequests);
		}

		private List<ExportRequest> CreateExportRequests(ObjectExportInfo[] artifacts)
		{
			List<ExportRequest> exportRequests = new List<ExportRequest>();

			foreach (var artifact in artifacts)
			{
				ExportRequest nativeExportRequest = _nativeExportRequestBuilder.Create(artifact);
				exportRequests.Add(nativeExportRequest);

				foreach (var image in artifact.Images.Cast<ImageExportInfo>())
				{
					ExportRequest imageExportRequest = _imageExportRequestBuilder.Create(image);
					exportRequests.Add(imageExportRequest);
				}
			}

			//TODO yeah, I know...
			exportRequests.RemoveAll(x => x == null);
			return exportRequests;
		}

		private void DownloadFiles(CancellationToken cancellationToken, List<ExportRequest> exportRequests)
		{
			_logger.LogVerbose("Creating TAPI bridge for export. Adding {count} requests to it.", exportRequests.Count);
			using (TapiBridge tapiBridge = _exportTapiBridgeFactory.Create(cancellationToken))
			{
				int order = 1;
				foreach (var exportRequest in exportRequests)
				{
					try
					{
						_logger.LogVerbose("Adding export request for downloading file {source} to {destination}.", exportRequest.SourceLocation, exportRequest.DestinationLocation);
						tapiBridge.AddPath(exportRequest.SourceLocation, exportRequest.DestinationLocation, order++);
					}
					catch (System.Exception ex)
					{
						_logger.LogError(ex, "Error occurred during adding export request to TAPI bridge. Skipping.");
						throw;
					}
				}

				_logger.LogVerbose("Waiting for transfer to finish.");
				tapiBridge.WaitForTransferJob();
				_logger.LogVerbose("Transfer finished. Exiting file downloader.");
			}
		}
	}
}