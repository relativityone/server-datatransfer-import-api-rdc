using System;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import.Helpers;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesTask : IImportNativesTask
	{
		private readonly LoadFile _settings;
		private readonly IFileUploader _fileUploader;
		private readonly IImportNativesAnalyzer _importNativesAnalyzer;
		private readonly IRepositoryFilePathHelper _repositoryFilePathHelper;

		public ImportNativesTask(LoadFile settings, IFileUploaderFactory fileUploaderFactory, IImportNativesAnalyzer importNativesAnalyzer,
			IRepositoryFilePathHelper repositoryFilePathHelper)
		{
			_settings = settings;
			_importNativesAnalyzer = importNativesAnalyzer;
			_repositoryFilePathHelper = repositoryFilePathHelper;
			_fileUploader = fileUploaderFactory.CreateNativeFileUploader();
		}

		public void Execute(FileMetadata fileMetadata)
		{
			// This task reffers to document type native import
			fileMetadata.UploadFile = ExtractUploadCheck(fileMetadata);
			if (CanExecute(fileMetadata))
			{
				FileMetadata processedFileMetadata = _importNativesAnalyzer.Process(fileMetadata);
				if (processedFileMetadata.FileExists)
				{
					processedFileMetadata.FileGuid = Guid.NewGuid().ToString();
					CopyFile(processedFileMetadata);
				}
			}
		}

		private void CopyFile(FileMetadata fileMetadata)
		{
			if (_settings.CopyFilesToDocumentRepository)
			{
				fileMetadata.DestinationDirectory = _repositoryFilePathHelper.GetNextDestinationDirectory();
				_fileUploader.UploadFile(fileMetadata);
			}
		}

		private bool ExtractUploadCheck(FileMetadata fileMetadata)
		{
			return fileMetadata.ArtifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.File).Length > 0;
		}

		private bool CanExecute(FileMetadata fileMetadata)
		{
			return fileMetadata.UploadFile && _settings.ArtifactTypeID == (int)ArtifactType.Document;
		}
	}
}
