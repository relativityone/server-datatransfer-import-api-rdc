using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class Downloader : IDownloader
	{
		private List<ExportRequest> _fileExportRequests;
		private List<LongTextExportRequest> _longTextExportRequests;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly IPhysicalFilesDownloader _physicalFilesDownloader;
		private readonly SafeIncrement _safeIncrement;
		private readonly IErrorFileWriter _errorFileWriter;

		private readonly IExportTapiBridgeFactory _exportTapiBridgeFactory;

		private readonly ILog _logger;

		public Downloader(NativeRepository nativeRepository, ImageRepository imageRepository,
			LongTextRepository longTextRepository, IPhysicalFilesDownloader physicalFilesDownloader, SafeIncrement safeIncrement, 
			IExportTapiBridgeFactory exportTapiBridgeFactory, IErrorFileWriter errorFileWriter, ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_physicalFilesDownloader = physicalFilesDownloader;
			_safeIncrement = safeIncrement;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_logger = logger;
			_errorFileWriter = errorFileWriter;

			//todo: Adrian - remove below line when validation is confirmed to be working on environment with properly signed certificates.
			ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
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
			DownloadRequests(cancellationToken).ConfigureAwait(false).GetAwaiter().GetResult();
		}

		private void RetrieveExportRequests()
		{
			_fileExportRequests = new List<ExportRequest>();

			IList<ExportRequest> nativeExportRequests = _nativeRepository.GetExportRequests();
			_fileExportRequests.AddRange(nativeExportRequests);

			IList<ExportRequest> imageExportRequests = _imageRepository.GetExportRequests();
			_fileExportRequests.AddRange(imageExportRequests);

			IList<LongTextExportRequest> longTextExportRequests = _longTextRepository.GetExportRequests();
			_longTextExportRequests = new List<LongTextExportRequest>();
			_longTextExportRequests.AddRange(longTextExportRequests);
		}

		private async Task DownloadRequests(CancellationToken cancellationToken)
		{
			IDownloadTapiBridge longTextDownloader = null;
			try
			{
				Task filesDownloadTask = _physicalFilesDownloader.DownloadFilesAsync(_fileExportRequests, cancellationToken);

				longTextDownloader = _exportTapiBridgeFactory.CreateForLongText(cancellationToken);
				DownloadLongTexts(longTextDownloader, cancellationToken);

				_logger.LogVerbose("Waiting for long text transfer to finish.");
				longTextDownloader.WaitForTransferJob();
				_logger.LogVerbose("Long text transfer finished.");

				_logger.LogVerbose("Waiting for files transfer to finish.");
				await filesDownloadTask;
				_logger.LogVerbose("Files transfer finished.");
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
					longTextDownloader?.Dispose();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to dispose DownloadTapiBridge for long text.");
				}
			}
		}

		private void DownloadLongTexts(IDownloadTapiBridge longTextDownloader, CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating TAPI bridge for long text export. Adding {count} requests to it.", _longTextExportRequests.Count);

			foreach (LongTextExportRequest textExportRequest in _longTextExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				try
				{
					_logger.LogVerbose("Adding export request for downloading long text {fieldId} to {destination}.", textExportRequest.FieldArtifactId, textExportRequest.DestinationLocation);
					TransferPath path = textExportRequest.CreateTransferPath(_safeIncrement.GetNext());
					textExportRequest.FileName = longTextDownloader.QueueDownload(path);
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