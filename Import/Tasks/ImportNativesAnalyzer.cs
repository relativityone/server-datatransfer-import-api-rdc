using kCura.Utility.Extensions;
using kCura.WinEDDS.Api;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesAnalyzer  : IImportNativesAnalyzer
	{
		private readonly ITransferConfig _transferConfig;
		private readonly IFileHelper _fileHelper;

		public ImportNativesAnalyzer(ITransferConfig transferConfig, IFileHelper fileHelper)
		{
			_transferConfig = transferConfig;
			_fileHelper = fileHelper;
		}

		public FileMetadata Process(ArtifactFieldCollection artifactFieldCollection)
		{
			FileMetadata fileMetadata = GetDefaults(artifactFieldCollection);

			if (_transferConfig.DisableNativeLocationValidation)
			{
				return fileMetadata;
			}
			fileMetadata.FileExists = ExtractFileExists(fileMetadata.FileName);
			fileMetadata.FileName = ExtractFileName(fileMetadata);
			fileMetadata.FileIdData = ExtractFileIdData(fileMetadata.FileName);
			return fileMetadata;
		}

		private bool ExtractFileExists(string fileName)
		{
			bool emptyFileName = fileName.Trim().IsNullOrEmpty();
			if (emptyFileName)
			{
				return false;
			}
			bool fileExists = FileExists(fileName);

			if (!fileExists)
			{
				// TODO:
				// RDC: lineStatus += Relativity.MassImport.ImportStatus.FileSpecifiedDne 'Throw New InvalidFilenameException(filename)
				return false;
			}
			if (FileContentIsEmpty(fileName))
			{
				if (_transferConfig.CreateErrorForEmptyNativeFile)
				{
					// TODO:
					// RDC: lineStatus += Relativity.MassImport.ImportStatus.EmptyFile 'Throw New EmptyNativeFileException(filename)
					return true;
				}
				return false;
			}
			return true;
		}

		private string ExtractFileName(FileMetadata fileMetadata)
		{
			if (!fileMetadata.FileExists && FileContentIsEmpty(fileMetadata.FileName))
			{
				return string.Empty;
			}
			return fileMetadata.FileName;
		}

		private OI.FileID.FileIDData ExtractFileIdData(string fileName)
		{
			return _transferConfig.DisableNativeValidation ? null : OI.FileID.Manager.Instance.GetFileIDDataByFilePath(fileName);
		}

		private bool FileContentIsEmpty(string fileName)
		{
			// TODO
			return false;
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
				fileName = ".{fileName}";
			}
			return fileName;
		}

		private FileMetadata GetDefaults(ArtifactFieldCollection artifactFieldCollection)
		{
			return new FileMetadata()
			{
				FileName = GetFileName(artifactFieldCollection)
			};
		}
	}
}
