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
		private readonly IExportConfig _exportConfig;
		private readonly ILog _logger;
		private readonly SafeIncrement _safeIncrement;

		public PhysicalFilesDownloader(
			IFileShareSettingsService settingsService,
			IFileTapiBridgePool fileTapiBridgePool,
			IExportConfig exportConfig,
			SafeIncrement safeIncrement,
			ILog logger)
		{
			_settingsService = settingsService;
			_fileTapiBridgePool = fileTapiBridgePool;
			_exportConfig = exportConfig;
			_safeIncrement = safeIncrement;
			_logger = logger;
		}

		public async Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken token)
		{
			var taskCancellationTokenSource = new DownloadCancellationTokenSource(token);
			await _settingsService.ReadFileSharesAsync(token);

			ConcurrentQueue<ExportRequestsWithFileshareSettings> queue = this.CreateTransferQueue(requests);
			_logger.LogVerbose(
				"Adding {filesToExportCount} requests for files through {tapiBridgeCount} TAPI bridges.",
				requests.Count,
				queue.Count);

			var tasks = new List<Task>();

			for (var i = 0; i < _exportConfig.MaxNumberOfFileExportTasks; i++)
			{
				tasks.Add(Task.Run(() => this.CreateJobTask(queue, taskCancellationTokenSource), taskCancellationTokenSource.Token));
			}

			await Task.WhenAll(tasks).ConfigureAwait(false);
		}

		private ConcurrentQueue<ExportRequestsWithFileshareSettings> CreateTransferQueue(List<ExportRequest> requests)
		{
			ILookup<IRelativityFileShareSettings, ExportRequest> result = requests.ToLookup(
				r => _settingsService.GetSettingsForFileShare(r.ArtifactId, r.SourceLocation));
			ConcurrentQueue<ExportRequestsWithFileshareSettings> results =
				new ConcurrentQueue<ExportRequestsWithFileshareSettings>(
					result.Select(r => new ExportRequestsWithFileshareSettings(r.Key, r)));
			return results;
		}

		private void CreateJobTask(
			ConcurrentQueue<ExportRequestsWithFileshareSettings> queue,
			DownloadCancellationTokenSource downloadCancellationTokenSourceSource)
		{
			ExportRequestsWithFileshareSettings requests;
			while (queue.TryDequeue(out requests))
			{
				try
				{
					IDownloadTapiBridge bridge = _fileTapiBridgePool.Request(
						requests.FileshareSettings,
						downloadCancellationTokenSourceSource.Token);
					this.DownloadFiles(bridge, requests.Requests, downloadCancellationTokenSourceSource.Token);
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