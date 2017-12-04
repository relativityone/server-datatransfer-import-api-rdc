using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.TApi;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FilesDownloader
	{
		private readonly IExportRequestBuilder _nativeExportRequestBuilder;
		private readonly IExportRequestBuilder _imageExportRequestBuilder;
		private readonly LongTextExportRequestBuilder _longTextExportRequestBuilder;
		private readonly IDirectoryManager _directoryManager;

		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;

		//TODO replace with new TAPI client
		private readonly FileDownloader _fileDownloader;

		//TODO remove this after replacing FileDownloader
		private readonly ExportFile _exportFile;

		private readonly ILog _logger;

		public FilesDownloader(IExportRequestBuilder nativeExportRequestBuilder, IExportRequestBuilder imageExportRequestBuilder,
			LongTextExportRequestBuilder longTextExportRequestBuilder, ExportTapiBridgeFactory exportTapiBridgeFactory, IDirectoryManager directoryManager, ILog logger,
			FileDownloader fileDownloader, ExportFile exportFile)
		{
			_nativeExportRequestBuilder = nativeExportRequestBuilder;
			_imageExportRequestBuilder = imageExportRequestBuilder;
			_longTextExportRequestBuilder = longTextExportRequestBuilder;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_directoryManager = directoryManager;
			_logger = logger;
			_fileDownloader = fileDownloader;
			_exportFile = exportFile;
		}

		public void DownloadFilesForArtifacts(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			List<ExportRequest> exportRequests = CreateExportRequests(artifacts, volumePredictions);

			if (exportRequests.Count == 0 || cancellationToken.IsCancellationRequested)
			{
				return;
			}

			//TODO we can add request when creating them, no need to create list first
			DownloadFiles(cancellationToken, exportRequests);
		}

		private List<ExportRequest> CreateExportRequests(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions)
		{
			List<ExportRequest> exportRequests = new List<ExportRequest>();

			for (int i = 0; i < artifacts.Length; i++)
			{
				_directoryManager.MoveNext(volumePredictions[i]);

				IEnumerable<ExportRequest> nativeExportRequests = _nativeExportRequestBuilder.Create(artifacts[i]);
				exportRequests.AddRange(nativeExportRequests);

				IEnumerable<ExportRequest> imageExportRequests = _imageExportRequestBuilder.Create(artifacts[i]);
				exportRequests.AddRange(imageExportRequests);

				BuildTextRequests(artifacts[i]);
			}

			return exportRequests;
		}

		private void BuildTextRequests(ObjectExportInfo artifact)
		{
			IEnumerable<TextExportRequest> longTextExportRequests = _longTextExportRequestBuilder.Create(artifact);

			//TODO replace with new TAPI client
			foreach (var textExportRequest in longTextExportRequests)
			{
				if (textExportRequest.FullText)
				{
					_fileDownloader.DownloadFullTextFile(textExportRequest.DestinationLocation, textExportRequest.ArtifactId, _exportFile.CaseInfo.ArtifactID.ToString());
				}
				else
				{
					_fileDownloader.DownloadLongTextFile(textExportRequest.DestinationLocation, textExportRequest.ArtifactId, textExportRequest.FieldArtifactId,
						_exportFile.CaseInfo.ArtifactID.ToString());
				}
			}
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
					catch (Exception ex)
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