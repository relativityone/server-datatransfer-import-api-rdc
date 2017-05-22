using System.IO;
using kCura.Utility;
using kCura.WinEDDS.Importers;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportDocumentFoldersTask : IImportFoldersTask
	{
		private const int _UNKNOWN_PARENT_FOLDER_ID = -9;

		private readonly ITransferConfig _transferConfig;
		private readonly IFolderCache _folderCache;

		public ImportDocumentFoldersTask(IFolderCache folderCache, ITransferConfig transferConfig)
		{
			_folderCache = folderCache;
			_transferConfig = transferConfig;
		}

		public void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			if (importBatchContext.ImportContext.Settings.LoadFile.CreateFolderStructure)
			{
				CreateFolderStructure(fileMetadata);
			}
			else
			{
				fileMetadata.ParentFolderId = importBatchContext.ImportContext.Settings.FolderId;
				fileMetadata.FolderPath = string.Empty;
			}
		}

		private void CreateFolderStructure(FileMetadata fileMetadata)
		{
			int parentFolderId;
			string folderPath = string.Empty;

			if (_transferConfig.CreateFoldersInWebAPI)
			{
				var parentFolderPath = GetParentArtifact(fileMetadata);
				if (PathIsTooLarge(parentFolderPath))
				{
					throw new PathTooLongException("Error occurred when importing the document. The folder name is longer than 255 characters.");
				}
				folderPath = parentFolderPath;
				parentFolderId = _UNKNOWN_PARENT_FOLDER_ID;
			}
			else
			{
				parentFolderId = _folderCache.get_FolderId(GetParentArtifact(fileMetadata));
			}
			fileMetadata.FolderPath = folderPath;
			fileMetadata.ParentFolderId = parentFolderId;
		}

		private string GetParentArtifact(FileMetadata fileMetadata)
		{
			var parentArtifact =
				NullableTypesHelper.ToEmptyStringOrValue(NullableTypesHelper.DBNullString(fileMetadata.ArtifactFieldCollection.get_FieldList(FieldCategory.ParentArtifact)[0].Value));
			return CleanDestinationFolderPath(parentArtifact);
		}

		private string CleanDestinationFolderPath(string path)
		{
			//TODO refactor!
			return BulkLoadFileImporter.CleanDestinationFolderPath(path);
		}

		private bool PathIsTooLarge(string parentFolderPath)
		{
			//TODO refactor!
			return BulkLoadFileImporter.InnerRelativityFolderPathsAreTooLarge(parentFolderPath);
		}
	}
}