using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Statistics;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks.Helpers;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class PushMetadataFilesTask : IPushMetadataFilesTask
	{
		private readonly IMetadataFilesServerExecution _metadataFilesServerExecution;
		private readonly IFileUploaderFactory _fileUploaderFactory;
		private readonly IServerErrorManager _serverErrorManager;
		private readonly IImportMetadata _importMetadata;
		private readonly ICancellationProvider _cancellationProvider;
		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec;
		private readonly IMetadataStatisticsHandler _metadataStatisticsHandler;
		private readonly ILog _log;

		public PushMetadataFilesTask(IMetadataFilesServerExecution metadataFilesServerExecution, IFileUploaderFactory fileUploaderFactory,
			IServerErrorManager serverErrorManager, IImportMetadata importMetadata, ICancellationProvider cancellationProvider, IImportExceptionHandlerExec importExceptionHandlerExec,
			IMetadataStatisticsHandler metadataStatisticsHandler, ILog log)
		{
			_metadataFilesServerExecution = metadataFilesServerExecution;
			_fileUploaderFactory = fileUploaderFactory;
			_serverErrorManager = serverErrorManager;
			_importMetadata = importMetadata;
			_cancellationProvider = cancellationProvider;
			_importExceptionHandlerExec = importExceptionHandlerExec;
			_metadataStatisticsHandler = metadataStatisticsHandler;
			_log = log;
		}

		public void PushMetadataFiles(ImportBatchContext importBatchContext)
		{
			_log.LogInformation("Push metadata process started");
			_cancellationProvider.ThrowIfCancellationRequested();
			foreach (var metadataFilesInfo in importBatchContext.MetadataFilesInfo)
			{

				_importExceptionHandlerExec.TryCatchExecNonFatal(() =>
				{
					_metadataStatisticsHandler.RaiseUploadingMetadataFileEvent(importBatchContext.MetadataFilesInfo.Count, importBatchContext.MetadataFilesInfo.IndexOf(metadataFilesInfo) + 1);
					var uploadResult = UploadFiles(metadataFilesInfo);

					int errorsCount = uploadResult.Values.Count(item => !item.Success);
					_log.LogInformation($"Metadata uploaded completed with {errorsCount} errors");

					// Here we need to check cancellation operation was requested as uploadResults variable will may not contain any results in that case
					_cancellationProvider.ThrowIfCancellationRequested();
					if (uploadResult.Any(x => !x.Value.Success))
					{
						throw new Exception(
							$"Failed to upload metadata files for batch with messages: {string.Join(",", uploadResult.Values.Where(x => !x.Success).Select(x => x.ErrorMessage).ToList())}");
					}

					_importMetadata.BatchSizeHistoryList.Add(metadataFilesInfo.BatchSize);
					_metadataFilesServerExecution.Import(metadataFilesInfo);

					_serverErrorManager.ManageErrors(importBatchContext.ImportContext);
				});
			}
			_log.LogInformation("Push metadata process completed");
		}

		private IDictionary<FileMetadata, UploadResult> UploadFiles(MetadataFilesInfo metadataFilesInfo)
		{
			_log.LogDebug("Creating bcp file uploader");
			var fileUploader = _fileUploaderFactory.CreateBcpFileUploader();

			_log.LogDebug($"Uploading native metadata from file: {metadataFilesInfo.NativeFilePath.FileName} ");
			fileUploader.UploadFile(metadataFilesInfo.NativeFilePath);
			_log.LogDebug($"Uploading data grid metadata from file: {metadataFilesInfo.DataGridFilePath.FileName} ");
			fileUploader.UploadFile(metadataFilesInfo.DataGridFilePath);
			_log.LogDebug($"Uploading code metadata from file: {metadataFilesInfo.CodeFilePath.FileName} ");
			fileUploader.UploadFile(metadataFilesInfo.CodeFilePath);
			_log.LogDebug($"Uploading object metadata from file: {metadataFilesInfo.ObjectFilePath.FileName} ");
			fileUploader.UploadFile(metadataFilesInfo.ObjectFilePath);

			_log.LogDebug("Waiting on bcp File upload to complete...");
			return fileUploader.WaitForUploadToComplete();
		}
	}
}