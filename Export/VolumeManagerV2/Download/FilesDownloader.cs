using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FilesDownloader
	{
		private List<ExportRequest> _fileExportRequests;
		private List<LongTextExportRequest> _longTextExportRequests;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;

		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;

		private readonly ILog _logger;

		public FilesDownloader(NativeRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository, ExportTapiBridgeFactory exportTapiBridgeFactory,
			ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_logger = logger;
		}

		public void DownloadFilesForArtifacts(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			RetrieveExportRequests();

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			DownloadRequests(cancellationToken);
		}

		private void RetrieveExportRequests()
		{
			IList<ExportRequest> nativeExportRequests = _nativeRepository.GetExportRequests();
			IList<ExportRequest> imageExportRequests = _imageRepository.GetExportRequests();

			_fileExportRequests = new List<ExportRequest>();
			_fileExportRequests.AddRange(nativeExportRequests);
			_fileExportRequests.AddRange(imageExportRequests);

			IList<LongTextExportRequest> longTextExportRequests = _longTextRepository.GetExportRequests();

			_longTextExportRequests = new List<LongTextExportRequest>();
			_longTextExportRequests.AddRange(longTextExportRequests);
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
			catch (OperationCanceledException ex)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_logger.LogWarning(ex, "Operation canceled during transfer.");
					return;
				}
				throw;
			}
			catch (TransferException ex)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					//this is needed, because TAPI in Web mode throws TransferException after canceling
					_logger.LogWarning(ex, "TransferException occurred during transfer, but cancellation has been requested.");
					return;
				}
				throw;
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

			foreach (var fileExportRequest in _fileExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return tapiBridge;
				}
				try
				{
					_logger.LogVerbose("Adding export request for downloading file for artifact {artifactId} to {destination}.", fileExportRequest.ArtifactId,
						fileExportRequest.DestinationLocation);
					tapiBridge.AddPath(fileExportRequest.CreateTransferPath());
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

			foreach (var textExportRequest in _longTextExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return tapiBridge;
				}
				try
				{
					_logger.LogVerbose("Adding export request for downloading long text {fieldId} to {destination}.", textExportRequest.FieldArtifactId, textExportRequest.DestinationLocation);
					tapiBridge.AddPath(textExportRequest.CreateTransferPath());
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