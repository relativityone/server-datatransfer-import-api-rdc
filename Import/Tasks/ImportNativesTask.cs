using System;
using kCura.WinEDDS.Api;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesTask : IImportNativesTask
	{
		private readonly LoadFile _settings;
		private readonly IFileUploader _fileUploader;
		private readonly IFileHelper _fileHelper;
		private readonly ITransferConfig _transferConfig;

		public ImportNativesTask(LoadFile settings, IFileUploaderFactory fileUploaderFactory, IFileHelper fileHelper, ITransferConfig transferConfig)
		{
			_settings = settings;
			_fileHelper = fileHelper;
			_transferConfig = transferConfig;

			_fileUploader = fileUploaderFactory.CreateNativeFileUploader();
		}

		public void Execute(ArtifactFieldCollection artifactFieldCollection)
		{
			// This task reffers to document type native import
			if (CanExecute(artifactFieldCollection))
			{
				try
				{
					// TODO: Need to verify if we need special care for not existing source (native) file
					// If filename <> String.Empty AndAlso Not fileExists Then lineStatus += Relativity.MassImport.ImportStatus.FileSpecifiedDne 'Throw New InvalidFilenameException(filename)
					string fileName = GetFileName(artifactFieldCollection);
					if (FileExists(fileName))
					{
						OI.FileID.FileIDData fileId = GetFileId(fileName);
						CopyFile(fileName);
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

		/// <summary>
		/// It can be overridden to support IInjectableFieldCollection redord type
		/// </summary>
		/// <param name="fileName"></param>
		/// <returns></returns>
		protected virtual OI.FileID.FileIDData GetFileId(string fileName)
		{
			if (_transferConfig.DisableNativeValidation)
			{
				return null;
			}
			return OI.FileID.Manager.Instance.GetFileIDDataByFilePath(fileName);
		}

		private bool FileExists(string fileName)
		{
			return _fileHelper.Exists(fileName);
		}

		private bool CanExecute(ArtifactFieldCollection artifactFieldCollection)
		{
			return artifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.File).Length > 0
					&& GetFileName(artifactFieldCollection) != null
					&& _settings.ArtifactTypeID == (int)ArtifactType.Document;
		}

		private string GetFileName(ArtifactFieldCollection artifactFieldCollection)
		{
			return artifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.File)[0].Value.ToString();
		}
	}
}
