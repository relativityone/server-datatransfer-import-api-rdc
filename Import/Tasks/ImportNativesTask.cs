using System;
using kCura.WinEDDS.Core.Import.Factories;
using kCura.WinEDDS.Core.Import.Helpers;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesTask : IImportNativesTask
	{
		private readonly IFileUploader _fileUploader;
		private readonly IImportNativesAnalyzer _importNativesAnalyzer;
		private readonly IRepositoryFilePathHelper _repositoryFilePathHelper;

		public ImportNativesTask(IFileUploaderFactory fileUploaderFactory, IImportNativesAnalyzer importNativesAnalyzer,
			IRepositoryFilePathHelper repositoryFilePathHelper)
		{
			_importNativesAnalyzer = importNativesAnalyzer;
			_repositoryFilePathHelper = repositoryFilePathHelper;
			_fileUploader = fileUploaderFactory.CreateNativeFileUploader();
		}

		public void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			// This task reffers to document type native import
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