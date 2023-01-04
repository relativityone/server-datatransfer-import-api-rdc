namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.DataExchange.Export.VolumeManagerV2.Statistics;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	public class PhysicalFilesDownloader : IPhysicalFilesDownloader
	{
		private readonly IFileShareSettingsService _settingsService;
		private readonly IFileTapiBridgePool _fileTapiBridgePool;
		private readonly IDownloadProgressManager _downloadProgressManager;
		private readonly ILog _logger;
		private readonly SafeIncrement _safeIncrement;

		public PhysicalFilesDownloader(
			IFileShareSettingsService settingsService,
			IFileTapiBridgePool fileTapiBridgePool,
			IDownloadProgressManager downloadProgressManager,
			SafeIncrement safeIncrement,
			ILog logger)
		{
			_settingsService = settingsService.ThrowIfNull(nameof(settingsService));
			_fileTapiBridgePool = fileTapiBridgePool.ThrowIfNull(nameof(fileTapiBridgePool));
			_downloadProgressManager = downloadProgressManager.ThrowIfNull(nameof(downloadProgressManager));
			_safeIncrement = safeIncrement.ThrowIfNull(nameof(safeIncrement));
			_logger = logger.ThrowIfNull(nameof(logger));
		}

		public async Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken token)
		{
			await _settingsService.ReadFileSharesAsync(token).ConfigureAwait(false);

			List<ExportRequestsWithFileshareSettings> exportRequestsFileShareSettingsList =
				this.GetExportRequestFileShareSettings(requests);
			_logger.LogVerbose(
				"Adding {FilesToExportCount} requests for files through {ExportRequestsWithFileShareSettingsCount} file share setting objects.",
				requests.Count,
				exportRequestsFileShareSettingsList.Count);
			var tasks = new List<Task>();
			foreach (ExportRequestsWithFileshareSettings settings in exportRequestsFileShareSettingsList)
			{
				tasks.Add(
					Task.Run(
						() => this.CreateJobTask(settings, token),
						token));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		private List<ExportRequestsWithFileshareSettings> GetExportRequestFileShareSettings(List<ExportRequest> requests)
		{
			ILookup<IRelativityFileShareSettings, ExportRequest> result = requests.ToLookup(
				r => _settingsService.GetSettingsForFileShare(r.ArtifactId, r.SourceLocation));
			return new List<ExportRequestsWithFileshareSettings>(
				result.Select(r => new ExportRequestsWithFileshareSettings(r.Key, r)));
		}

		private void CreateJobTask(
			ExportRequestsWithFileshareSettings settings,
			CancellationToken token)
		{
			IDownloadTapiBridge bridge = _fileTapiBridgePool.Request(settings.FileshareSettings, token);
			this.DownloadFiles(bridge, settings.Requests, token);
			bridge.WaitForTransfers();
		}

		private void DownloadFiles(
			IDownloadTapiBridge bridge,
			IEnumerable<ExportRequest> fileExportRequests,
			CancellationToken cancellationToken)
		{
			_logger.LogDebug("Starting requesting files download...");
			foreach (ExportRequest fileExportRequest in fileExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				try
				{
					_logger.LogVerbose(
						"Adding export request for downloading file for artifact {artifactId} to {destination}.",
						fileExportRequest.ArtifactId,
						fileExportRequest.DestinationLocation.Secure());
					fileExportRequest.FileName =
						bridge.QueueDownload(fileExportRequest.CreateTransferPath(_safeIncrement.GetNext()));
				}
				catch (ArgumentException ex)
				{
					_logger.LogWarning(
						ex,
						"There was a problem downloading artifact {ArtifactId}.",
						fileExportRequest.ArtifactId);
					_downloadProgressManager.MarkArtifactAsError(fileExportRequest.ArtifactId, ex.Message);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex, "Error occurred during adding file export request to TAPI bridge. Skipping.");
					throw;
				}
			}
		}
	}
}