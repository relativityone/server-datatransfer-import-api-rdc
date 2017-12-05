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
		private List<FileExportRequest> _fileExportRequests;
		private List<LongTextExportRequest> _longTextExportRequests;

		private readonly IFileExportRequestBuilder _nativeExportRequestBuilder;
		private readonly IFileExportRequestBuilder _imageExportRequestBuilder;
		private readonly LongTextExportRequestBuilder _longTextExportRequestBuilder;
		private readonly IDirectoryManager _directoryManager;

		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;

		private readonly ILog _logger;

		#region TEMP

		//TODO replace with new TAPI client
		private readonly FileDownloader _fileDownloader;

		//TODO remove this after replacing FileDownloader
		private readonly ExportFile _exportFile;

		#endregion

		public FilesDownloader(IFileExportRequestBuilder nativeExportRequestBuilder, IFileExportRequestBuilder imageExportRequestBuilder,
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

			//TODO depending on how slow this is we could consider adding requests to TAPI when we create them
			CreateExportRequests(artifacts, volumePredictions, cancellationToken);

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			DownloadFiles(cancellationToken);
			DownloadLongTexts(cancellationToken);
		}

		private void CreateExportRequests(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			_fileExportRequests = new List<FileExportRequest>();
			_longTextExportRequests = new List<LongTextExportRequest>();

			for (int i = 0; i < artifacts.Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				_logger.LogVerbose("Creating export requests for artifact {artifactId}.", artifacts[i].ArtifactID);

				_directoryManager.MoveNext(volumePredictions[i]);

				IEnumerable<FileExportRequest> nativeExportRequests = _nativeExportRequestBuilder.Create(artifacts[i]);
				_fileExportRequests.AddRange(nativeExportRequests);

				IEnumerable<FileExportRequest> imageExportRequests = _imageExportRequestBuilder.Create(artifacts[i]);
				_fileExportRequests.AddRange(imageExportRequests);

				IEnumerable<LongTextExportRequest> longTextExportRequestsForArtifact = _longTextExportRequestBuilder.Create(artifacts[i]);
				_longTextExportRequests.AddRange(longTextExportRequestsForArtifact);
			}
		}

		private void DownloadFiles(CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating TAPI bridge for export. Adding {count} requests to it.", _fileExportRequests.Count);
			using (TapiBridge tapiBridge = _exportTapiBridgeFactory.Create(cancellationToken))
			{
				int order = 1;
				foreach (var fileExportRequest in _fileExportRequests)
				{
					try
					{
						_logger.LogVerbose("Adding export request for downloading file {source} to {destination}.", fileExportRequest.SourceLocation, fileExportRequest.DestinationLocation);
						tapiBridge.AddPath(fileExportRequest.SourceLocation, fileExportRequest.DestinationLocation, order++);
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

		private void DownloadLongTexts(CancellationToken cancellationToken)
		{
			//TODO replace with new TAPI client
			foreach (var textExportRequest in _longTextExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				if (textExportRequest.FullText)
				{
					_logger.LogVerbose("Downloading Full Text for artifact {artifactId}. Field {fieldId}.", textExportRequest.ArtifactId, textExportRequest.FieldArtifactId);
					_fileDownloader.DownloadFullTextFile(textExportRequest.DestinationLocation, textExportRequest.ArtifactId, _exportFile.CaseInfo.ArtifactID.ToString());
				}
				else
				{
					_logger.LogVerbose("Downloading Long Text for artifact {artifactId}. Field {fieldId}.", textExportRequest.ArtifactId, textExportRequest.FieldArtifactId);
					_fileDownloader.DownloadLongTextFile(textExportRequest.DestinationLocation, textExportRequest.ArtifactId, textExportRequest.FieldArtifactId,
						_exportFile.CaseInfo.ArtifactID.ToString());
				}
			}
		}
	}
}