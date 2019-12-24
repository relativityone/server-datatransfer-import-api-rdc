namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Transfer;
	using Relativity.Logging;

	internal class LongTextObjectManagerDownloader : ILongTextDownloader, IDisposable
	{
		private readonly SafeIncrement _safeIncrement;
		private readonly IMetadataProcessingStatistics _metadataProcessingStatistics;
		private readonly IAppSettings _appSettings;
		private readonly ILog _logger;
		private readonly IDownloadProgressManager _downloadProgressManager;
		private readonly ILongTextRepository _longTextRepository;
		private readonly ILongTextStreamService _longTextStreamService;
		private readonly IStatus _status;
		private readonly ExportFile _exportSettings;
		private bool disposed ;

		/// <summary>
		/// Initializes a new instance of the <see cref="LongTextObjectManagerDownloader"/> class. This is used exclusively for testing.
		/// </summary>
		/// <param name="metadataProcessingStatistics">
		/// The statistics manager that keeps track of long text total bytes and duration..
		/// </param>
		/// <param name="exportSettings">
		/// The export settings that contain connection and workspace parameters.
		/// </param>
		/// <param name="safeIncrement">
		/// The object used to assign a unique order.
		/// </param>
		/// <param name="longTextRepository">
		/// The long text repository.
		/// </param>
		/// <param name="downloadProgressManager">
		/// The download progress manager.
		/// </param>
		/// <param name="status">
		/// The status.
		/// </param>
		/// <param name="logger">
		/// The logger instance.
		/// </param>
		/// <param name="appSettings">
		/// The application settings.
		/// </param>
		/// <param name="longTextStreamService">
		/// The service that provides a streaming API to retrieve long text data.
		/// </param>
		public LongTextObjectManagerDownloader(
			IMetadataProcessingStatistics metadataProcessingStatistics,
			ExportFile exportSettings,
			SafeIncrement safeIncrement,
			ILongTextRepository longTextRepository,
			IDownloadProgressManager downloadProgressManager,
			IStatus status,
			ILog logger,
			IAppSettings appSettings,
			ILongTextStreamService longTextStreamService)
		{
			_safeIncrement = safeIncrement.ThrowIfNull(nameof(safeIncrement));
			_metadataProcessingStatistics = metadataProcessingStatistics.ThrowIfNull(nameof(metadataProcessingStatistics));
			_exportSettings = exportSettings.ThrowIfNull(nameof(exportSettings));
			_longTextRepository = longTextRepository.ThrowIfNull(nameof(longTextRepository));
			_downloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_status = status.ThrowIfNull(nameof(status));
			_appSettings = appSettings.ThrowIfNull(nameof(appSettings));
			_logger = logger.ThrowIfNull(nameof(logger));
			_longTextStreamService = longTextStreamService.ThrowIfNull(nameof(longTextStreamService));
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public async Task DownloadAsync(List<LongTextExportRequest> longTextExportRequests, CancellationToken cancellationToken)
		{
			longTextExportRequests.ThrowIfNull(nameof(longTextExportRequests));
			Stopwatch sw = Stopwatch.StartNew();
			long originalTicks = _metadataProcessingStatistics.MetadataTime;
			int exportLongThreadCount = _exportSettings.CaseInfo.EnableDataGrid
				                            ? _appSettings.ExportLongTextDataGridThreadCount
				                            : _appSettings.ExportLongTextSqlThreadCount;
			if (exportLongThreadCount < 1)
			{
				throw new ArgumentException("The settings dictate the number of threads to be used is 0, this is not a valid option.");
			}

			_logger.LogVerbose(
				"Preparing to download {TotalLongTextRequests} long text field data using {ExportLongThreadCount} threads.",
				longTextExportRequests.Count,
				exportLongThreadCount);
			using (SemaphoreSlim throttler = new SemaphoreSlim(exportLongThreadCount))
			{
				List<Task> tasks = new List<Task>();
				foreach (LongTextExportRequest longTextExportRequest in longTextExportRequests)
				{
					await throttler.WaitAsync(cancellationToken).ConfigureAwait(false);
					tasks.Add(
						Task.Run(
							async () =>
							{
								try
								{
									await this.DownloadLongTextFileAsync(
										longTextExportRequest,
										sw,
										originalTicks,
										cancellationToken).ConfigureAwait(false);
								}
								catch (ArgumentException e)
								{
									_logger.LogWarning(
										e,
										"There was a problem with the request details that prevented retrieving the long text data from artifact {ArtifactId}.",
										longTextExportRequest.ArtifactId);
									_downloadProgressManager.MarkArtifactAsError(longTextExportRequest.ArtifactId, e.Message);
								}
								catch (Exception e)
								{
									_logger.LogError(
										e,
										"There was an unexpected problem retrieving the long text data from artifact {ArtifactId}.",
										longTextExportRequest.ArtifactId);
									throw;
								}
								finally
								{
									throttler.Release();
								}
							},
							cancellationToken));
				}

				await Task.WhenAll(tasks).ConfigureAwait(false);
				_logger.LogVerbose(
					"Completed download of {TotalLongTextRequests} long text field data using {ExportLongThreadCount} threads.",
					longTextExportRequests.Count,
					exportLongThreadCount);
			}
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					_longTextStreamService?.Dispose();
				}

				this.disposed = true;
			}
		}

		private async Task DownloadLongTextFileAsync(
			LongTextExportRequest request,
			Stopwatch stopwatch,
			long originalTicks,
			CancellationToken cancellationToken)
		{
			if (request.FieldArtifactId < 1 || request.ArtifactId < 1)
			{
				string errorPart = request.ArtifactId < 1 ? "an" : "a field";
				throw new ArgumentException(
					$"The long text request requires {errorPart} artifact identifier greater than zero.",
					nameof(request));
			}

			if (request.ArtifactId < 1)
			{
				throw new ArgumentException(
					"The long text request requires an artifact identifier greater than zero.",
					nameof(request));
			}

			if (string.IsNullOrWhiteSpace(request.DestinationLocation))
			{
				throw new ArgumentException(
					"The long text request requires the destination location file to be non-empty.",
					nameof(request));
			}

			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}

			request.Order = _safeIncrement.GetNext();
			_logger.LogVerbose(
				"Adding export request for downloading long text {FieldArtifactId} to {DestinationLocation}.",
				request.FieldArtifactId,
				request.DestinationLocation);
			LongText longText = _longTextRepository.GetLongText(
				request.ArtifactId,
				request.FieldArtifactId);
			if (longText == null)
			{
				string message =
					$"Failed to retrieve the long text object for artifact {request.ArtifactId} and field {request.FieldArtifactId}";
				_logger.LogWarning(message);
				_downloadProgressManager.MarkArtifactAsError(request.ArtifactId, message);
				return;
			}

			request.FileName = System.IO.Path.GetFileName(request.DestinationLocation);
			IProgress<LongTextStreamProgressEventArgs> progress = new Progress<LongTextStreamProgressEventArgs>(
				args =>
					{
						if (args.TotalBytesWritten > 0)
						{
							_status.WriteStatusLine(EventType2.Status, args.ToString(), false);
						}
					});
			LongTextStreamResult result = await _longTextStreamService.SaveLongTextStreamAsync(
				                              new LongTextStreamRequest
					                              {
						                              SourceEncoding = longText.SourceEncoding,
						                              SourceFieldArtifactId = request.FieldArtifactId,
						                              SourceObjectArtifactId = request.ArtifactId,
						                              SourceTotalBytes = longText.Length,
						                              TargetFile = request.DestinationLocation,
						                              TargetEncoding = longText.DestinationEncoding,
						                              WorkspaceId = _exportSettings.CaseArtifactID
					                              },
				                              cancellationToken,
				                              progress).ConfigureAwait(false);
			_metadataProcessingStatistics.UpdateStatistics(
				result.FileName,
				result.Successful,
				result.Length,
				originalTicks + stopwatch.ElapsedTicks);
			_logger.LogVerbose(
				"Long text progress event for {FileName} with completion status {Status} and duration {Duration} ({LineNumber}).",
				request.FileName,
				result.Issue == null,
				result.Duration,
				request.Order);
			_downloadProgressManager.MarkLongTextAsCompleted(
				request.DestinationLocation,
				request.Order,
				result.Successful);
			if (result.Issue != null)
			{
				_status.WriteWarningWithoutDocCount(
					$"There was a non-fatal issue retrieving the {request.ArtifactId} long text data from the {request.FieldArtifactId} field. Details: {result.Issue.Message}");
			}
		}
	}
}