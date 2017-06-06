using System;
using kCura.WinEDDS.Core.Import.Status;

namespace kCura.WinEDDS.Core.Import.Statistics
{
	public class StatisticsManager : IStatisticsManager
	{
		private readonly IImportStatusManager _importStatusManager;
		private readonly IImportMetadata _importMetadata;
		private readonly ITransferConfig _transferConfig;

		private readonly int _startLineNumber;
		private int _filesTransferred;

		private readonly object _lock = new object();

		public StatisticsManager(LoadFile loadFile, IImportStatusManager importStatusManager, IImportMetadata importMetadata, ITransferConfig transferConfig,
			INativeFilesStatisticsHandler nativeStatistics, IMetadataFilesStatisticsHandler metadataStatistics, IMetadataStatisticsHandler metadataStatisticsHandler,
			IBulkImportStatisticsHandler bulkImportStatisticsHandler, IServerErrorStatisticsHandler serverErrorStatisticsHandler, IJobFinishStatisticsHandler jobFinishStatisticsHandler)
		{
			_importStatusManager = importStatusManager;
			_importMetadata = importMetadata;
			_transferConfig = transferConfig;

			_startLineNumber = (int) loadFile.StartLineNumber;

			_importMetadata.Statistics.BatchSize = transferConfig.ImportBatchSize;

			nativeStatistics.TransferRateChanged += OnNativeFilesTransferRateChanged;
			nativeStatistics.FilesTransferred += OnFilesTransferred;
			metadataStatistics.TransferRateChanged += OnMetadataFilesTransferRateChanged;
			metadataStatisticsHandler.FileMetadataProcessed += OnFileMetadataProcessed;
			bulkImportStatisticsHandler.BulkImportCompleted += OnBulkImportCompleted;
			bulkImportStatisticsHandler.IoWarningOccurred += OnIoWarningOccurred;
			serverErrorStatisticsHandler.RetrievingServerErrors += OnRetrievingServerErrors;
			serverErrorStatisticsHandler.RetrievingServerErrorStatusUpdated += OnRetrievingServerErrorStatusUpdated;
			jobFinishStatisticsHandler.JobFinished += OnJobFinished;
		}

		private void OnJobFinished(object sender, string s)
		{
			_importStatusManager.RaiseCustomStatusUpdateEvent(this, StatusUpdateType.End, s, _filesTransferred);
		}

		private void OnRetrievingServerErrors(object sender, EventArgs eventArgs)
		{
			RaiseUpdateEvent("Retrieving errors from server");
		}

		private void OnRetrievingServerErrorStatusUpdated(object sender, string s)
		{
			RaiseUpdateEvent(s);
		}

		private void OnBulkImportCompleted(object sender, BulkImportCompletedEventArgs bulkImportCompletedEventArgs)
		{
			_importMetadata.Statistics.ProcessRunResults(bulkImportCompletedEventArgs.Results);
			lock (_lock)
			{
				_importMetadata.Statistics.SqlTime += bulkImportCompletedEventArgs.Time;
			}
			_importMetadata.Statistics.DocCount = _importMetadata.Statistics.DocumentsCreated + _importMetadata.Statistics.DocumentsUpdated;
			RaiseUpdateEvent("Metadata uploaded");
		}

		private void OnIoWarningOccurred(object sender, Exception exception)
		{
			_importStatusManager.RaiseIoWarningEvent(sender, _transferConfig.IoErrorWaitTimeInSeconds, _filesTransferred, exception);
		}

		private void OnFilesTransferred(object sender, FilesTransferredEventArgs filesTransferredEventArgs)
		{
			lock (_lock)
			{
				_filesTransferred = Math.Max(_filesTransferred, filesTransferredEventArgs.FilesTransferred + _startLineNumber);
			}
			RaiseUpdateEvent("Uploading files");
		}

		private void OnMetadataFilesTransferRateChanged(object sender, TransferRateEventArgs transferRateEventArgs)
		{
			_importMetadata.Statistics.MetadataBytes = transferRateEventArgs.TransferredBytes;
			_importMetadata.Statistics.MetadataTime = transferRateEventArgs.TransferTime;

			RaiseUpdateEvent("Uploading metadata");
		}

		private void OnNativeFilesTransferRateChanged(object sender, TransferRateEventArgs transferRateEventArgs)
		{
			_importMetadata.Statistics.FileBytes = transferRateEventArgs.TransferredBytes;
			_importMetadata.Statistics.FileTime = transferRateEventArgs.TransferTime;

			RaiseUpdateEvent("Uploading files");
		}

		private void RaiseUpdateEvent(string message)
		{
			if (_filesTransferred > 0)
			{
				_importStatusManager.RaiseCustomStatusUpdateEvent(this, StatusUpdateType.Progress, message, _filesTransferred);
			}
		}

		private void OnFileMetadataProcessed(object sender, FileMetadataEventArgs fileMetadataEventArgs)
		{
			lock (_lock)
			{
				_filesTransferred = Math.Max(_filesTransferred, fileMetadataEventArgs.LineNumber);
			}
			RaiseUpdateEvent($"Processing file metadata: Item '{fileMetadataEventArgs.RecordId}' processed [line {fileMetadataEventArgs.LineNumber}].");
		}
	}
}