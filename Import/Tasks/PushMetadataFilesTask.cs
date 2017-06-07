using System;
using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks.Helpers;

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

		public PushMetadataFilesTask(IMetadataFilesServerExecution metadataFilesServerExecution, IFileUploaderFactory fileUploaderFactory,
			IServerErrorManager serverErrorManager, IImportMetadata importMetadata, ICancellationProvider cancellationProvider, IImportExceptionHandlerExec importExceptionHandlerExec)
		{
			_metadataFilesServerExecution = metadataFilesServerExecution;
			_fileUploaderFactory = fileUploaderFactory;
			_serverErrorManager = serverErrorManager;
			_importMetadata = importMetadata;
			_cancellationProvider = cancellationProvider;
			_importExceptionHandlerExec = importExceptionHandlerExec;
		}

		public void PushMetadataFiles(ImportBatchContext importBatchContext)
		{
			_cancellationProvider.ThrowIfCancellationRequested();
			foreach (var metadataFilesInfo in importBatchContext.MetadataFilesInfo)
			{
				_importExceptionHandlerExec.TryCatchExec(() =>
				{
					var uploadResult = UploadFiles(metadataFilesInfo);

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
		}

		private IDictionary<FileMetadata, UploadResult> UploadFiles(MetadataFilesInfo metadataFilesInfo)
		{
			var fileUploader = _fileUploaderFactory.CreateBcpFileUploader();

			fileUploader.UploadFile(metadataFilesInfo.NativeFilePath);
			fileUploader.UploadFile(metadataFilesInfo.DataGridFilePath);
			fileUploader.UploadFile(metadataFilesInfo.CodeFilePath);
			fileUploader.UploadFile(metadataFilesInfo.ObjectFilePath);

			return fileUploader.WaitForUploadToComplete();
		}
	}
}