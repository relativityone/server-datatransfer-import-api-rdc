using System.IO;
using kCura.Utility;
using kCura.WinEDDS.Importers;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportDocumentFoldersTask : IImportFoldersTask
	{
		private const int _UNKNOWN_PARENT_FOLDER_ID = -9;

		private readonly ITransferConfig _transferConfig;
		private readonly ILog _log;
		private readonly IFolderCache _folderCache;

		public ImportDocumentFoldersTask(IFolderCache folderCache, ITransferConfig transferConfig, ILog log)
		{
			_folderCache = folderCache;
			_transferConfig = transferConfig;
			_log = log;
		}

		public void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			if (importBatchContext.ImportContext.Settings.LoadFile.CreateFolderStructure)
			{
				_log.LogDebug($"Document folder processing task started for record at line {fileMetadata.LineNumber}");
				CreateFolderStructure(fileMetadata);
				_log.LogDebug($"Document folder processing task completed for record at line {fileMetadata.LineNumber}");
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
					throw new PathTooLongException(string.Format(LogMessages.PathTooLongMessage, parentFolderPath));
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