using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Status;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesTask : IImportNativesTask
	{
		private readonly IFileUploaderFactory _fileUploaderFactory;
		private readonly IImportNativesAnalyzer _importNativesAnalyzer;
		private readonly IRepositoryFilePathHelper _repositoryFilePathHelper;
		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec;
		private readonly IUploadErrors _uploadErrors;
		private readonly ICancellationProvider _cancellationProvider;

		private IFileUploader _fileUploader;

		public ImportNativesTask(IFileUploaderFactory fileUploaderFactory, IImportNativesAnalyzer importNativesAnalyzer,
			IRepositoryFilePathHelper repositoryFilePathHelper, IImportExceptionHandlerExec importExceptionHandlerExec, IUploadErrors uploadErrors,
			ICancellationProvider cancellationProvider)
		{
			_importNativesAnalyzer = importNativesAnalyzer;
			_repositoryFilePathHelper = repositoryFilePathHelper;
			_importExceptionHandlerExec = importExceptionHandlerExec;
			_uploadErrors = uploadErrors;
			_cancellationProvider = cancellationProvider;
			_fileUploaderFactory = fileUploaderFactory;
		}

		public IDictionary<FileMetadata, UploadResult> Execute(ImportBatchContext importBatchContext)
		{
			_fileUploader = _fileUploaderFactory.CreateNativeFileUploader();
			foreach (var fileMetadata in importBatchContext.FileMetaDataHolder)
			{
				Upload(fileMetadata, importBatchContext);
			}
			var uploadResult = _fileUploader.WaitForUploadToComplete();

			if (_cancellationProvider.GetToken().IsCancellationRequested)
			{
				_cancellationProvider.GetToken().ThrowIfCancellationRequested();
			}

			_uploadErrors.HandleUploadErrors(uploadResult);
			return uploadResult;
		}

		private void Upload(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			// This task reffers to document type native import

			_importExceptionHandlerExec.TryCatchExec(() =>
			{
				fileMetadata.UploadFile = ExtractUploadCheck(fileMetadata);
				if (CanExecute(fileMetadata, importBatchContext))
				{
					FileMetadata processedFileMetadata = _importNativesAnalyzer.Process(fileMetadata);
					if (processedFileMetadata.FileExists)
					{
						processedFileMetadata.FileGuid = Guid.NewGuid().ToString();
						CopyFile(processedFileMetadata, importBatchContext);
					}
				}
			});
		}

		private void CopyFile(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			if (importBatchContext.ImportContext.Settings.LoadFile.CopyFilesToDocumentRepository)
			{
				fileMetadata.DestinationDirectory = _repositoryFilePathHelper.GetNextDestinationDirectory();
				_fileUploader.UploadFile(fileMetadata);
			}
		}

		private bool ExtractUploadCheck(FileMetadata fileMetadata)
		{
			return fileMetadata.ArtifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.File).Length > 0;
		}

		private bool CanExecute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			return fileMetadata.UploadFile && importBatchContext.ImportContext.Settings.LoadFile.ArtifactTypeID == (int) ArtifactType.Document;
		}
	}
}