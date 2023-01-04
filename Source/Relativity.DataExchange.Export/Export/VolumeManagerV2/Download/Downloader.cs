namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.Logging;
	using Relativity.Transfer;

	public class Downloader : IDownloader
	{
		private List<ExportRequest> _fileExportRequests;
		private List<LongTextExportRequest> _longTextExportRequests;

		private readonly IPhysicalFilesDownloader _physicalFilesDownloader;
		private readonly ILongTextDownloader _longTextDownloader;
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly ILog _logger;
		private readonly IExportRequestRetriever _exportRequestRetriever;

		public Downloader(
			IExportRequestRetriever exportRequestRetriever,
			IPhysicalFilesDownloader physicalFilesDownloader,
			ILongTextDownloader longTextDownloader,
			ILongTextFileDownloadSubscriber longTextFileDownloadSubscriber,
			IErrorFileWriter errorFileWriter,
			IFileDownloadSubscriber fileSubscriber,
			ILog logger)
		{
			_physicalFilesDownloader = physicalFilesDownloader;
			_longTextDownloader = longTextDownloader;
			_logger = logger;
			_exportRequestRetriever = exportRequestRetriever;
			_errorFileWriter = errorFileWriter;
			longTextFileDownloadSubscriber?.RegisterSubscriber(fileSubscriber);
			if (AppSettings.Instance.SuppressServerCertificateValidation)
			{
				ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => true;
			}
		}

		public async Task DownloadFilesForArtifactsAsync(CancellationToken cancellationToken)
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
			await DownloadRequests(cancellationToken).ConfigureAwait(false);
		}

		private void RetrieveExportRequests()
		{
			_fileExportRequests = _exportRequestRetriever.RetrieveFileExportRequests();
			_longTextExportRequests = _exportRequestRetriever.RetrieveLongTextExportRequests();
		}

		private async Task DownloadRequests(CancellationToken cancellationToken)
		{
			try
			{
				_logger.LogVerbose("Waiting for long text and physical file transfers to finish.");
				Task filesDownloadTask = _physicalFilesDownloader.DownloadFilesAsync(_fileExportRequests, cancellationToken);
				Task longTextDownloadTask = _longTextDownloader.DownloadAsync(_longTextExportRequests, cancellationToken);
				await Task.WhenAll(filesDownloadTask, longTextDownloadTask).ConfigureAwait(false);
				_logger.LogVerbose("Long text and physical file transfers have finished.");
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

				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Generic, null, string.Empty,
					$"Fatal exception occurred during transfer. Failed to download files for batch {ex.Message}");
				_logger.LogError(ex,
					"TransferException occurred during transfer and cancellation has NOT been requested.");
				throw;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred during transfer.");
				throw;
			}
		}
	}
}