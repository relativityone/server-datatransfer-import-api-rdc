namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	public class PhysicalFilesDownloader : IPhysicalFilesDownloader
	{
		private readonly IFileShareSettingsService _settingsService;
		private readonly IFileTapiBridgePool _fileTapiBridgePool;
		private readonly ILog _logger;
		private readonly SafeIncrement _safeIncrement;

		public PhysicalFilesDownloader(
			IFileShareSettingsService settingsService,
			IFileTapiBridgePool fileTapiBridgePool,
			SafeIncrement safeIncrement,
			ILog logger)
		{
			_settingsService = settingsService;
			_fileTapiBridgePool = fileTapiBridgePool;
			_safeIncrement = safeIncrement;
			_logger = logger;
		}

		public async Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken token)
		{
			var taskCancellationTokenSource = new DownloadCancellationTokenSource(token);
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
						() => this.CreateJobTask(settings, taskCancellationTokenSource),
						taskCancellationTokenSource.Token));
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
			DownloadCancellationTokenSource tokenSource)
		{
			try
			{
				IDownloadTapiBridge bridge = _fileTapiBridgePool.Request(settings.FileshareSettings, tokenSource.Token);
				this.DownloadFiles(bridge, settings.Requests, tokenSource.Token);
				bridge.WaitForTransfers();
			}
			catch (TaskCanceledException)
			{
				if (!tokenSource.IsBatchCancelled())
				{
					throw;
				}
			}
			catch (Exception)
			{
				tokenSource.Cancel();
				throw;
			}
		}

		private void DownloadFiles(
			IDownloadTapiBridge bridge,
			IEnumerable<ExportRequest> fileExportRequests,
			CancellationToken cancellationToken)
		{
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
						fileExportRequest.DestinationLocation);
					fileExportRequest.FileName =
						bridge.QueueDownload(fileExportRequest.CreateTransferPath(_safeIncrement.GetNext()));
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