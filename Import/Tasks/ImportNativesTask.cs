using System;
using System.Collections.Generic;
using System.Linq;
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
			IDictionary<FileMetadata, UploadResult> uploadResults;
			if (!CopyFilesToRepository(importBatchContext))
			{
				uploadResults = importBatchContext.FileMetaDataHolder.ToDictionary(x => x, y => new UploadResult
				{
					Success = true
				});
			}
			else
			{
				uploadResults = UploadFiles(importBatchContext);
			}

			if (_cancellationProvider.GetToken().IsCancellationRequested)
			{
				_cancellationProvider.GetToken().ThrowIfCancellationRequested();
			}

			_uploadErrors.HandleUploadErrors(uploadResults);
			return uploadResults;
		}

		private bool CopyFilesToRepository(ImportBatchContext importBatchContext)
		{
			return importBatchContext.ImportContext.Settings.LoadFile.CopyFilesToDocumentRepository;
		}

		private IDictionary<FileMetadata, UploadResult> UploadFiles(ImportBatchContext importBatchContext)
		{
			_fileUploader = _fileUploaderFactory.CreateNativeFileUploader();
			foreach (var fileMetadata in importBatchContext.FileMetaDataHolder)
			{
				Upload(fileMetadata, importBatchContext);
			}
			var uploadResults = _fileUploader.WaitForUploadToComplete();

			AddSkippedFilesToResult(importBatchContext, uploadResults);

			return uploadResults;
		}

		private void AddSkippedFilesToResult(ImportBatchContext importBatchContext, IDictionary<FileMetadata, UploadResult> uploadResults)
		{
			foreach (var fileMetadata in importBatchContext.FileMetaDataHolder.Where(x => !x.UploadFile))
			{
				uploadResults.Add(fileMetadata, new UploadResult
				{
					Success = true
				});
			}
		}

		private void Upload(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			// This task reffers to document type native import

			_importExceptionHandlerExec.TryCatchExec(() =>
			{
				fileMetadata.FileGuid = Guid.NewGuid().ToString();
				fileMetadata.UploadFile = ExtractUploadCheck(fileMetadata);
				if (CanExecute(fileMetadata, importBatchContext))
				{
					FileMetadata processedFileMetadata = _importNativesAnalyzer.Process(fileMetadata);
					if (processedFileMetadata.FileExists)
					{
						UploadFile(processedFileMetadata);
					}
				}
			});
		}

		private bool ExtractUploadCheck(FileMetadata fileMetadata)
		{
			return fileMetadata.ArtifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.File).Length > 0;
		}

		private bool CanExecute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			return fileMetadata.UploadFile && importBatchContext.ImportContext.Settings.LoadFile.ArtifactTypeID == (int) ArtifactType.Document;
		}

		private void UploadFile(FileMetadata fileMetadata)
		{
			fileMetadata.DestinationDirectory = _repositoryFilePathHelper.GetNextDestinationDirectory();
			_fileUploader.UploadFile(fileMetadata);
		}
	}
}