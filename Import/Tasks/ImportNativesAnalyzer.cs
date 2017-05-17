using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import.Helpers;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesAnalyzer  : IImportNativesAnalyzer
	{
		private readonly IFileInfoProvider _fileInfoProvider;
		private readonly ITransferConfig _transferConfig;
		private readonly IFileHelper _fileHelper;

		public ImportNativesAnalyzer(IFileInfoProvider fileInfoProvider, ITransferConfig transferConfig, IFileHelper fileHelper)
		{
			_fileInfoProvider = fileInfoProvider;
			_transferConfig = transferConfig;
			_fileHelper = fileHelper;
		}

		public FileMetadata Process(FileMetadata fileMetadata)
		{
			SetDefaults(fileMetadata);

			if (_transferConfig.DisableNativeLocationValidation)
			{
				return fileMetadata;
			}
			fileMetadata = ExtractFileExistsAndName(fileMetadata);
			fileMetadata.FileIdData = ExtractFileIdData(fileMetadata);
			return fileMetadata;
		}

		private FileMetadata ExtractFileExistsAndName(FileMetadata fileMetadata)
		{
			bool fileExists = FileExists(fileMetadata.FileName);
			if (!fileExists)
			{
				// TODO:
				// RDC: lineStatus += Relativity.MassImport.ImportProcessStatus.FileSpecifiedDne 'Throw New InvalidFilenameException(filename)
				//return false;

				fileMetadata.FileExists = false;
			}
			if (fileMetadata.FileExists && FileContentIsEmpty(fileMetadata))
			{
				if (!_transferConfig.CreateErrorForEmptyNativeFile)
				{
					// TODO:
					// RDC: WriteWarning("The file " & filename & " is empty; only metadata will be loaded for this record.")
					fileMetadata.FileExists = false;
					fileMetadata.FileName = string.Empty;
				}
				else
				{
					// TODO:
					// RDC: lineStatus += Relativity.MassImport.ImportProcessStatus.EmptyFile 'Throw New EmptyNativeFileException(filename)
				}
			}
			return fileMetadata;
		}

		private OI.FileID.FileIDData ExtractFileIdData(FileMetadata fileMetadata)
		{
			if (fileMetadata.FileExists && !_transferConfig.DisableNativeValidation)
			{
				return _fileInfoProvider.GetFileId(fileMetadata.FileName);
			}
			return null;
		}

		private bool FileContentIsEmpty(FileMetadata fileMetadata)
		{
			return _fileInfoProvider.GetFileSize(fileMetadata) == 0;
		}

		private bool FileExists(string fileName)
		{
			return _fileHelper.Exists(fileName);
		}

		private string GetFileName(ArtifactFieldCollection artifactFieldCollection)
		{
			string fileName = artifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.File)[0].Value.ToString().Trim();
			if (fileName.Length > 1 && fileName.StartsWith("\\") && fileName[1] != '\\')
			{
				fileName = $".{fileName}";
			}
			return fileName;
		}

		private void SetDefaults(FileMetadata fileMetadata)
		{
			fileMetadata.FileName = GetFileName(fileMetadata.ArtifactFieldCollection);
			fileMetadata.FileExists = true;
		}
	}
}
