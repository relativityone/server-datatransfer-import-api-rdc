using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.TapiHelpers;
using Relativity.Logging;
using Relativity.Transfer;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class PhysicalFilesDownloader : IPhysicalFilesDownloader
	{
		private readonly IFileshareCredentialsService _credentialsService;
		private readonly IExportTapiBridgeFactory _exportTapiBridgeFactory;
		private readonly IExportConfig _exportConfig;
		private readonly ILog _logger;
		private readonly SafeIncrement _safeIncrement;

		public PhysicalFilesDownloader(IFileshareCredentialsService credentialsService, IExportTapiBridgeFactory exportTapiBridgeFactory, IExportConfig exportConfig, SafeIncrement safeIncrement, ILog logger)
		{
			_credentialsService = credentialsService;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_exportConfig = exportConfig;
			_safeIncrement = safeIncrement;
			_logger = logger;
		}

		public async Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken batchCancellationToken)
		{
			var taskCancellationTokenSource = new DownloadCancellationTokenSource(batchCancellationToken);

			ConcurrentQueue<ExportRequestsWithCredentials> queue = CreateTransferQueue(requests);
			_logger.LogVerbose("Adding {filesToExportCount} requests for files through {tapiBridgeCount} TAPI bridges.", requests.Count, queue.Count);

			IEnumerable<Task> tasks = Enumerable.Repeat(Task.Run(() => CreateJobTask(queue, taskCancellationTokenSource), taskCancellationTokenSource.Token), _exportConfig.MaxNumberOfFileExportTasks);
			await AwaitAllTasks(tasks);
		}

		private ConcurrentQueue<ExportRequestsWithCredentials> CreateTransferQueue(List<ExportRequest> requests)
		{
			ILookup<AsperaCredential, ExportRequest> result = requests.ToLookup(r => _credentialsService.GetCredentialsForFileshare(r.SourceLocation));

			return new ConcurrentQueue<ExportRequestsWithCredentials>(result.Select(r => new ExportRequestsWithCredentials(r.Key, r)));
		}

		private async Task AwaitAllTasks(IEnumerable<Task> tasks)
		{
			foreach (Task task in tasks)
			{
				await task;
			}
		}

		private void CreateJobTask(ConcurrentQueue<ExportRequestsWithCredentials> queue, DownloadCancellationTokenSource downloadCancellationTokenSourceSource)
		{
			ExportRequestsWithCredentials exportRequestWithCredentials;

			while (queue.TryDequeue(out exportRequestWithCredentials))
			{
				try
				{
					using (IDownloadTapiBridge bridge = _exportTapiBridgeFactory.CreateForFiles(exportRequestWithCredentials.Credentials, downloadCancellationTokenSourceSource.Token))
					{
						DownloadFiles(bridge, exportRequestWithCredentials.Requests, downloadCancellationTokenSourceSource.Token);
						bridge.WaitForTransferJob();
					}
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
