using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
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
		private readonly LabelManager _labelManager;

		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;

		private readonly ILog _logger;

		public FilesDownloader(IFileExportRequestBuilder nativeExportRequestBuilder, IFileExportRequestBuilder imageExportRequestBuilder,
			LongTextExportRequestBuilder longTextExportRequestBuilder, ExportTapiBridgeFactory exportTapiBridgeFactory, IDirectoryManager directoryManager, ILog logger,
			LabelManager labelManager)
		{
			_nativeExportRequestBuilder = nativeExportRequestBuilder;
			_imageExportRequestBuilder = imageExportRequestBuilder;
			_longTextExportRequestBuilder = longTextExportRequestBuilder;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_directoryManager = directoryManager;
			_logger = logger;
			_labelManager = labelManager;
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

			DownloadRequests(cancellationToken);
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
				artifacts[i].DestinationVolume = _labelManager.GetCurrentVolumeLabel();

				IEnumerable<FileExportRequest> nativeExportRequests = _nativeExportRequestBuilder.Create(artifacts[i]);
				_fileExportRequests.AddRange(nativeExportRequests);

				IEnumerable<FileExportRequest> imageExportRequests = _imageExportRequestBuilder.Create(artifacts[i]);
				_fileExportRequests.AddRange(imageExportRequests);

				IEnumerable<LongTextExportRequest> longTextExportRequestsForArtifact = _longTextExportRequestBuilder.Create(artifacts[i]);
				_longTextExportRequests.AddRange(longTextExportRequestsForArtifact);
			}
		}

		private void DownloadRequests(CancellationToken cancellationToken)
		{
			IDownloadTapiBridge filesDownloader = null;
			IDownloadTapiBridge longTextDownloader = null;
			try
			{
				filesDownloader = DownloadFiles(cancellationToken);
				longTextDownloader = DownloadLongTexts(cancellationToken);

				_logger.LogVerbose("Waiting for file transfer to finish.");
				filesDownloader.WaitForTransferJob();
				_logger.LogVerbose("File transfer finished.");

				_logger.LogVerbose("Waiting for long text transfer to finish.");
				longTextDownloader.WaitForTransferJob();
				_logger.LogVerbose("Long text transfer finished.");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during transfer.");
				throw;
			}
			finally
			{
				try
				{
					filesDownloader?.Dispose();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to dispose DownloadTapiBridge for files.");
				}
				try
				{
					longTextDownloader?.Dispose();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to dispose DownloadTapiBridge for long text.");
				}
			}
		}

		private IDownloadTapiBridge DownloadFiles(CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating TAPI bridge for file export. Adding {count} requests to it.", _fileExportRequests.Count);
			IDownloadTapiBridge tapiBridge = _exportTapiBridgeFactory.Create(cancellationToken);

			int order = 1;
			foreach (var fileExportRequest in _fileExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return tapiBridge;
				}
				try
				{
					_logger.LogVerbose("Adding export request for downloading file {source} to {destination}.", fileExportRequest.SourceLocation, fileExportRequest.DestinationLocation);
					tapiBridge.AddPath(fileExportRequest.CreateTransferPath(order++));
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding file export request to TAPI bridge. Skipping.");
					throw;
				}
			}

			return tapiBridge;
		}

		private IDownloadTapiBridge DownloadLongTexts(CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating TAPI bridge for long text export. Adding {count} requests to it.", _longTextExportRequests.Count);
			IDownloadTapiBridge tapiBridge = _exportTapiBridgeFactory.CreateForLongText(cancellationToken);

			int order = 1;
			foreach (var textExportRequest in _longTextExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return tapiBridge;
				}
				try
				{
					_logger.LogVerbose("Adding export request for downloading long text {fieldId} to {destination}.", textExportRequest.FieldArtifactId, textExportRequest.DestinationLocation);
					tapiBridge.AddPath(textExportRequest.CreateTransferPath(order++));
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding long text export request to TAPI bridge. Skipping.");
					throw;
				}
			}

			return tapiBridge;
		}
	}
}