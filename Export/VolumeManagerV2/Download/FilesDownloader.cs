using System;
using System.Collections.Generic;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FilesDownloader
	{
		private List<ExportRequest> _nativeFileExportRequests;
		private List<ExportRequest> _imageFileExprotRequests;
		private List<LongTextExportRequest> _longTextExportRequests;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly ErrorFileWriter _errorFileWriter;

		private readonly ExportTapiBridgeFactory _exportTapiBridgeFactory;

		private readonly ILog _logger;

		public FilesDownloader(NativeRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository, ExportTapiBridgeFactory exportTapiBridgeFactory,
			ErrorFileWriter errorFileWriter, ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_logger = logger;
			_errorFileWriter = errorFileWriter;
		}

		public void DownloadFilesForArtifacts(ObjectExportInfo[] artifacts, VolumePredictions[] volumePredictions, CancellationToken cancellationToken)
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
			_imageFileExprotRequests = new List<ExportRequest>();
			_imageFileExprotRequests.AddRange(imageExportRequests);

			IList<LongTextExportRequest> longTextExportRequests = _longTextRepository.GetExportRequests();
			_longTextExportRequests = new List<LongTextExportRequest>();
			_longTextExportRequests.AddRange(longTextExportRequests);
		}

		private void DownloadRequests(CancellationToken cancellationToken)
		{
			//TODO REL-187625 we need three tapi bridges until I figure out how to identify files in TAPI without file name (which can be duplicated in case of native being an image)
			IDownloadTapiBridge nativeFilesDownloader = null;
			IDownloadTapiBridge imageFilesDownloader = null;
			IDownloadTapiBridge longTextDownloader = null;
			try
			{
				nativeFilesDownloader = DownloadNativeFiles(cancellationToken);
				imageFilesDownloader = DownloadImageFiles(cancellationToken);
				longTextDownloader = DownloadLongTexts(cancellationToken);

				_logger.LogVerbose("Waiting for native files transfer to finish.");
				nativeFilesDownloader.WaitForTransferJob();
				_logger.LogVerbose("Native files transfer finished.");

				_logger.LogVerbose("Waiting for image files transfer to finish.");
				imageFilesDownloader.WaitForTransferJob();
				_logger.LogVerbose("Image files transfer finished.");

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
				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Generic, string.Empty, string.Empty, $"Fatal exception occurred during transfer. Failed to download files for batch {ex.Message}");
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
					nativeFilesDownloader?.Dispose();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to dispose DownloadTapiBridge for native files.");
				}
				try
				{
					imageFilesDownloader?.Dispose();
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Failed to dispose DownloadTapiBridge for image files.");
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

		private IDownloadTapiBridge DownloadNativeFiles(CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating TAPI bridge for native file export. Adding {count} requests to it.", _nativeFileExportRequests.Count);
			IDownloadTapiBridge tapiBridge = _exportTapiBridgeFactory.CreateForNatives(cancellationToken);

			foreach (var fileExportRequest in _nativeFileExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return tapiBridge;
				}
				try
				{
					_logger.LogVerbose("Adding export request for downloading native file for artifact {artifactId} to {destination}.", fileExportRequest.ArtifactId,
						fileExportRequest.DestinationLocation);
					fileExportRequest.UniqueId = tapiBridge.AddPath(fileExportRequest.CreateTransferPath());
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding native file export request to TAPI bridge. Skipping.");
					throw;
				}
			}

			return tapiBridge;
		}

		private IDownloadTapiBridge DownloadImageFiles(CancellationToken cancellationToken)
		{
			_logger.LogVerbose("Creating TAPI bridge for image file export. Adding {count} requests to it.", _imageFileExprotRequests.Count);
			IDownloadTapiBridge tapiBridge = _exportTapiBridgeFactory.CreateForImages(cancellationToken);

			foreach (var fileExportRequest in _imageFileExprotRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return tapiBridge;
				}
				try
				{
					_logger.LogVerbose("Adding export request for downloading image file for artifact {artifactId} to {destination}.", fileExportRequest.ArtifactId,
						fileExportRequest.DestinationLocation);
					fileExportRequest.UniqueId = tapiBridge.AddPath(fileExportRequest.CreateTransferPath());
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding image file export request to TAPI bridge. Skipping.");
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
					textExportRequest.UniqueId = tapiBridge.AddPath(textExportRequest.CreateTransferPath());
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