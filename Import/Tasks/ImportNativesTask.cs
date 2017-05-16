using System;
using kCura.WinEDDS.Api;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesTask : IImportNativesTask
	{
		private readonly LoadFile _settings;
		private readonly IFileUploader _fileUploader;
		private readonly IImportNativesAnalyzer _importNativesAnalyzer;

		public ImportNativesTask(LoadFile settings, IFileUploaderFactory fileUploaderFactory, IImportNativesAnalyzer importNativesAnalyzer)
		{
			_settings = settings;
			_importNativesAnalyzer = importNativesAnalyzer;
			_fileUploader = fileUploaderFactory.CreateNativeFileUploader();
		}

		public void Execute(ArtifactFieldCollection artifactFieldCollection)
		{
			// This task reffers to document type native import
			if (CanExecute(artifactFieldCollection))
			{
				try
				{
					FileMetadata fileMetadata = _importNativesAnalyzer.Process(artifactFieldCollection);
					if (fileMetadata.FileExists)
					{
						CopyFile(fileMetadata.FileName);
					}
				}
				catch (Exception ex)
				{
					//TODO
					throw;
				}
			}
		}

		private void CopyFile(string sourceFileName)
		{
			if (_settings.CopyFilesToDocumentRepository)
			{
				string destinationFileName = Guid.NewGuid().ToString();
				_fileUploader.UploadFile(sourceFileName, destinationFileName);
			}
		}

		private bool CanExecute(ArtifactFieldCollection artifactFieldCollection)
		{
			return artifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.File).Length > 0
					&& _settings.ArtifactTypeID == (int)ArtifactType.Document;
		}
	}
}
