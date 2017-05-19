using System.IO;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import.Helpers;
using kCura.WinEDDS.Core.Import.Status;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportNativesAnalyzer  : IImportNativesAnalyzer
	{
		private readonly IFileInfoProvider _fileInfoProvider;
		private readonly ITransferConfig _transferConfig;
		private readonly IFileHelper _fileHelper;
		private readonly IImportStatusManager _importStatusManager;

		public ImportNativesAnalyzer(IFileInfoProvider fileInfoProvider, ITransferConfig transferConfig, IFileHelper fileHelper,
			IImportStatusManager importStatusManager)
		{
			_fileInfoProvider = fileInfoProvider;
			_transferConfig = transferConfig;
			_fileHelper = fileHelper;
			_importStatusManager = importStatusManager;
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
			bool fileExists = FileExists(fileMetadata.FullFilePath);
			if (!fileExists)
			{
				fileMetadata.LineStatus += (int)Relativity.MassImport.ImportStatus.FileSpecifiedDne;
				fileMetadata.FileExists = false;
			}
			if (fileMetadata.FileExists && FileContentIsEmpty(fileMetadata))
			{
				if (!_transferConfig.CreateErrorForEmptyNativeFile)
				{
					_importStatusManager.ReiseWarningImportEvent(this, $"The file {fileMetadata.FullFilePath} is empty; only metadata will be loaded for this record.", 0);
					fileMetadata.FileExists = false;
					fileMetadata.FullFilePath = string.Empty;
				}
				else
				{
					fileMetadata.LineStatus += (int)Relativity.MassImport.ImportStatus.EmptyFile;
				}
			}
			fileMetadata.FileName = Path.GetFileName(fileMetadata.FullFilePath);
			return fileMetadata;
		}

		private OI.FileID.FileIDData ExtractFileIdData(FileMetadata fileMetadata)
		{
			if (fileMetadata.FileExists && !_transferConfig.DisableNativeValidation)
			{
				return _fileInfoProvider.GetFileId(fileMetadata.FullFilePath);
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

		private string GetFullFilePath(ArtifactFieldCollection artifactFieldCollection)
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
			fileMetadata.FullFilePath = GetFullFilePath(fileMetadata.ArtifactFieldCollection);
			fileMetadata.FileExists = true;
		}
	}
}
