namespace Relativity.DataExchange.Export.VolumeManagerV2.Download
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers;
	using Relativity.Logging;

	public class PhysicalFilesDownloader : IPhysicalFilesDownloader
	{
		private readonly IFileShareSettingsService _settingsService;
		private readonly IFileTapiBridgePool _fileTapiBridgePool;
		private readonly ILog _logger;
		private readonly SafeIncrement _safeIncrement;

		public PhysicalFilesDownloader(IFileShareSettingsService settingsService, IFileTapiBridgePool fileTapiBridgePool, SafeIncrement safeIncrement, ILog logger)
		{
			_settingsService = settingsService;
			_fileTapiBridgePool = fileTapiBridgePool;
			_safeIncrement = safeIncrement;
			_logger = logger;
		}

		public async Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken batchCancellationToken)
		{
			var taskCancellationTokenSource = new DownloadCancellationTokenSource(batchCancellationToken);

			var exportRequestsFileshareSettingsList = GetExportRequestFileShareSettings(requests);
			_logger.LogVerbose("Adding {filesToExportCount} requests for files through {tapiBridgeCount} TAPI bridges.", requests.Count,
				exportRequestsFileshareSettingsList.Count);

			var tasks = new List<Task>();

			foreach(ExportRequestsWithFileshareSettings exportRequestsWithFileshareSettings in exportRequestsFileshareSettingsList)
			{
				tasks.Add(Task.Run(() => CreateJobTask(exportRequestsWithFileshareSettings, taskCancellationTokenSource), taskCancellationTokenSource.Token));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		private List<ExportRequestsWithFileshareSettings> GetExportRequestFileShareSettings(List<ExportRequest> requests)
		{
			ILookup<IRelativityFileShareSettings, ExportRequest> result = requests.ToLookup(r => _settingsService.GetSettingsForFileshare(r.SourceLocation));

			return new List<ExportRequestsWithFileshareSettings>(result.Select(r => new ExportRequestsWithFileshareSettings(r.Key, r)));
		}

		private void CreateJobTask(ExportRequestsWithFileshareSettings exportRequestWithFileshareSettings, DownloadCancellationTokenSource downloadCancellationTokenSourceSource)
		{
			IDownloadTapiBridge bridge = null;
			try
			{
				bridge = _fileTapiBridgePool.Request(exportRequestWithFileshareSettings.FileshareSettings,
					downloadCancellationTokenSourceSource.Token);

				DownloadFiles(bridge, exportRequestWithFileshareSettings.Requests,
					downloadCancellationTokenSourceSource.Token);
					bridge.WaitForTransfers();
			}
			catch (TaskCanceledException)
			{
				if (!downloadCancellationTokenSourceSource.IsBatchCancelled())
				{
					throw;
				}
			}
			catch (Exception)
			{
				downloadCancellationTokenSourceSource.Cancel();
				throw;
			}
			finally
			{
				if (bridge != null)
				{
					_fileTapiBridgePool.Release(bridge);
				}
			}
		}

		private void DownloadFiles(IDownloadTapiBridge filesDownloader, IEnumerable<ExportRequest> fileExportRequests, CancellationToken cancellationToken)
		{
			foreach (ExportRequest fileExportRequest in fileExportRequests)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				try
				{
					_logger.LogVerbose("Adding export request for downloading file for artifact {artifactId} to {destination}.", fileExportRequest.ArtifactId,
						fileExportRequest.DestinationLocation);
					fileExportRequest.FileName = filesDownloader.QueueDownload(fileExportRequest.CreateTransferPath(_safeIncrement.GetNext()));
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