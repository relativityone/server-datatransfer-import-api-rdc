using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class Downloader : IDownloader
	{
		private int _lineNumber;

		private List<ExportRequest> _nativeFileExportRequests;
		private List<ExportRequest> _imageFileExportRequests;
		private List<LongTextExportRequest> _longTextExportRequests;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly IErrorFileWriter _errorFileWriter;

		private readonly IExportTapiBridgeFactory _exportTapiBridgeFactory;

		private readonly ILog _logger;

		public Downloader(NativeRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository, IExportTapiBridgeFactory exportTapiBridgeFactory,
			IErrorFileWriter errorFileWriter, ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_logger = logger;
			_errorFileWriter = errorFileWriter;
		}

		public void DownloadFilesForArtifacts(CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_logger.LogVerbose("Retrieving all export requests from repositories.");
			RetrieveExportRequests();

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			_logger.LogVerbose("Attempting to download files.");
			DownloadRequests(cancellationToken);
		}

		private void RetrieveExportRequests()
		{
			IList<ExportRequest> nativeExportRequests = _nativeRepository.GetExportRequests();
			_nativeFileExportRequests = new List<ExportRequest>();
			_nativeFileExportRequests.AddRange(nativeExportRequests);

			IList<ExportRequest> imageExportRequests = _imageRepository.GetExportRequests();
			_imageFileExportRequests = new List<ExportRequest>();
			_imageFileExportRequests.AddRange(imageExportRequests);

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
				_lineNumber = 1;
				_logger.LogVerbose("Creating TAPI bridge for native and image files export.");
				filesDownloader = _exportTapiBridgeFactory.CreateForFiles(cancellationToken);
				DownloadNativeFiles(filesDownloader, cancellationToken);
				DownloadImageFiles(filesDownloader, cancellationToken);
				longTextDownloader = _exportTapiBridgeFactory.CreateForLongText(cancellationToken);
				DownloadLongTexts(longTextDownloader, cancellationToken);

				_logger.LogVerbose("Waiting for files transfer to finish.");
				filesDownloader.WaitForTransferJob();
				_logger.LogVerbose("Files transfer finished.");
				
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

				_logger.LogError(ex, "Operation canceled, but cancellation has NOT been requested.");
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

				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Generic, string.Empty, string.Empty,
					$"Fatal exception occurred during transfer. Failed to download files for batch {ex.Message}");
				_logger.LogError(ex, "TransferException occurred during transfer and cancellation has NOT been requested.");
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

		private void DownloadNativeFiles(IDownloadTapiBridge nativeFilesDownloader, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding {count} requests for native files to TAPI bridge.", _nativeFileExportRequests.Count);

			foreach (var fileExportRequest in _nativeFileExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				try
				{
					_logger.LogVerbose("Adding export request for downloading native file for artifact {artifactId} to {destination}.", fileExportRequest.ArtifactId,
						fileExportRequest.DestinationLocation);
					TransferPath path = fileExportRequest.CreateTransferPath(_lineNumber++);
					fileExportRequest.UniqueId = nativeFilesDownloader.AddPath(path);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding native file export request to TAPI bridge. Skipping.");
					throw;
				}
			}
		}

		private void DownloadImageFiles(IDownloadTapiBridge nativeFilesDownloader, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Adding {count} requests for image files to TAPI bridge.", _imageFileExportRequests.Count);

			foreach (var fileExportRequest in _imageFileExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				try
				{
					_logger.LogVerbose("Adding export request for downloading image file for artifact {artifactId} to {destination}.", fileExportRequest.ArtifactId,
						fileExportRequest.DestinationLocation);
					TransferPath path = fileExportRequest.CreateTransferPath(_lineNumber++);
					fileExportRequest.UniqueId = nativeFilesDownloader.AddPath(path);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding image file export request to TAPI bridge. Skipping.");
					throw;
				}
			}
		}

		private void DownloadLongTexts(IDownloadTapiBridge longTextDownloader, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating TAPI bridge for long text export. Adding {count} requests to it.", _longTextExportRequests.Count);

			foreach (var textExportRequest in _longTextExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				try
				{
					_logger.LogVerbose("Adding export request for downloading long text {fieldId} to {destination}.", textExportRequest.FieldArtifactId, textExportRequest.DestinationLocation);
					TransferPath path = textExportRequest.CreateTransferPath(_lineNumber++);
					textExportRequest.UniqueId = longTextDownloader.AddPath(path);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding long text export request to TAPI bridge. Skipping.");
					throw;
				}
			}
		}
	}
}