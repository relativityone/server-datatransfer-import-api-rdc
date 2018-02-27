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
		private const int _NUMBER_OF_TASKS = 2;

		private readonly IAsperaCredentialsService _credentialsService;
		private readonly IExportTapiBridgeFactory _exportTapiBridgeFactory;
		private readonly ILog _logger;
		private readonly SafeIncrement _safeIncrement;

		public PhysicalFilesDownloader(IAsperaCredentialsService credentialsService, IExportTapiBridgeFactory exportTapiBridgeFactory, SafeIncrement safeIncrement, ILog logger)
		{
			_credentialsService = credentialsService;
			_exportTapiBridgeFactory = exportTapiBridgeFactory;
			_safeIncrement = safeIncrement;
			_logger = logger;
		}

		public async Task DownloadFilesAsync(List<ExportRequest> requests, CancellationToken batchCancellationToken)
		{
			var taskCancellationTokenSource = Create(batchCancellationToken);

			ConcurrentQueue<ExportRequestsWithCredentials> queue = CreateTransferQueue(requests);
			_logger.LogVerbose("Adding {filesToExportCount} requests for files through {tapiBridgeCount} TAPI bridges.", requests.Count, queue.Count);

			IEnumerable<Task> tasks =
				Enumerable.Repeat(
					Task.Run(() => CreateJobTask(queue, taskCancellationTokenSource, batchCancellationToken), batchCancellationToken),
					_NUMBER_OF_TASKS);
			await AwaitAllTasks(tasks);
		}

		private static CancellationTokenSource Create(CancellationToken batchCancellationToken)
		{
			var taskCancellationTokenSource = new CancellationTokenSource();

			batchCancellationToken.Register(() => taskCancellationTokenSource.Cancel());
			return taskCancellationTokenSource;
		}

		private ConcurrentQueue<ExportRequestsWithCredentials> CreateTransferQueue(List<ExportRequest> requests)
		{
			ILookup<Credential, ExportRequest> result = requests.ToLookup(
				r => _credentialsService.GetAsperaCredentialsForFileshare(new Uri(r.SourceLocation)));

			var queue = new ConcurrentQueue<ExportRequestsWithCredentials>(result.Select(r => new ExportRequestsWithCredentials(r.Key, r)));
			return queue;
		}

		private async Task AwaitAllTasks(IEnumerable<Task> tasks)
		{
			foreach (Task task in tasks)
			{
				await task;
			}
		}

		private void CreateJobTask(ConcurrentQueue<ExportRequestsWithCredentials> queue, CancellationTokenSource taskCancellationTokenSource, CancellationToken batchCancellationToken)
		{
			IDownloadTapiBridge bridge = null;
			ExportRequestsWithCredentials exportRequestWithCredentials;

			while (queue.TryDequeue(out exportRequestWithCredentials))
			{
				try
				{
					bridge = _exportTapiBridgeFactory.CreateForFiles(exportRequestWithCredentials.Credentials, taskCancellationTokenSource.Token);
					DownloadFiles(bridge, exportRequestWithCredentials.Requests, taskCancellationTokenSource.Token);
					bridge.WaitForTransferJob();
				}
				catch (TaskCanceledException)
				{
					if (!batchCancellationToken.IsCancellationRequested)
					{
						throw;
					}
				}
				catch (Exception)
				{
					taskCancellationTokenSource.Cancel();
					throw;
				}
				finally
				{
					bridge?.Dispose();
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
