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
		private readonly ICancellationProvider _cancellationProvider;

		public PushMetadataFilesTask(IMetadataFilesServerExecution metadataFilesServerExecution, IFileUploaderFactory fileUploaderFactory, 
			IServerErrorManager serverErrorManager, ICancellationProvider cancellationProvider)
		{
			_metadataFilesServerExecution = metadataFilesServerExecution;
			_fileUploaderFactory = fileUploaderFactory;
			_serverErrorManager = serverErrorManager;
			_cancellationProvider = cancellationProvider;
		}

		public void PushMetadataFiles(ImportBatchContext importBatchContext)
		{
			var uploadResult = UploadFiles(importBatchContext);

			// Here we need to check cancellation operation was requested as uploadResults variable will may not contain any results in that case
			_cancellationProvider.ThrowIfCancellationRequested();
			if (uploadResult.Any(x => !x.Value.Success))
			{
				throw new Exception();
			}

			_metadataFilesServerExecution.Import(importBatchContext.MetadataFilesInfo);

			_serverErrorManager.ManageErrors(importBatchContext.ImportContext);
		}

		private IDictionary<FileMetadata, UploadResult> UploadFiles(ImportBatchContext importBatchContext)
		{
			var fileUploader = _fileUploaderFactory.CreateBcpFileUploader();

			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.NativeFilePath);
			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.DataGridFilePath);
			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.CodeFilePath);
			fileUploader.UploadFile(importBatchContext.MetadataFilesInfo.ObjectFilePath);

			return fileUploader.WaitForUploadToComplete();
		}
	}
}