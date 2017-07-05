using kCura.Utility;
using kCura.WinEDDS.Core.Import.Managers;
using Relativity;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportRdoFoldersTask : IImportFoldersTask
	{
		private readonly IObjectManager _objectManager;

		public ImportRdoFoldersTask(IObjectManager objectManager)
		{
			_objectManager = objectManager;
		}

		public void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			fileMetadata.FolderPath = string.Empty;

			if (importBatchContext.ImportContext.Settings.LoadFile.CreateFolderStructure)
			{
				HandleCreatingFolderStructure(fileMetadata, importBatchContext);
			}
			else
			{
				HandleNotCreatingFolderStructure(fileMetadata, importBatchContext);
			}
		}

		private void HandleCreatingFolderStructure(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			var parentArtifact = NullableTypesHelper.ToEmptyStringOrValue(
				NullableTypesHelper.DBNullString(fileMetadata.ArtifactFieldCollection.get_FieldList(FieldCategory.ParentArtifact)[0].Value));

			if (parentArtifact == "")
			{
				throw new LoadFileBase.ParentObjectReferenceRequiredException(fileMetadata.LineNumber, importBatchContext.ImportContext.Settings.DestinationFolderColumnIndex);
			}

			fileMetadata.ParentFolderId = RetrieveParentArtifactId(fileMetadata, importBatchContext, parentArtifact);
		}

		private int RetrieveParentArtifactId(FileMetadata fileMetadata, ImportBatchContext importBatchContext, string parentArtifact)
		{
			var parentObjectTable = _objectManager.RetrieveArtifactIdOfMappedParentObject(importBatchContext.ImportContext.Settings.LoadFile.CaseInfo.ArtifactID, parentArtifact,
				importBatchContext.ImportContext.Settings.LoadFile.ArtifactTypeID).Tables[0];
			if (parentObjectTable.Rows.Count > 1)
			{
				throw new LoadFileBase.DuplicateObjectReferenceException(fileMetadata.LineNumber, importBatchContext.ImportContext.Settings.DestinationFolderColumnIndex, "Parent Info");
			}
			if (parentObjectTable.Rows.Count == 0)
			{
				throw new LoadFileBase.NonExistentParentException(fileMetadata.LineNumber, importBatchContext.ImportContext.Settings.DestinationFolderColumnIndex, "Parent Info");
			}
			return (int) parentObjectTable.Rows[0]["ArtifactID"];
		}

		private static void HandleNotCreatingFolderStructure(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			if (importBatchContext.ImportContext.Settings.ParentArtifactTypeID == ArtifactType.Case)
			{
				fileMetadata.ParentFolderId = importBatchContext.ImportContext.Settings.FolderId;
			}
			else
			{
				fileMetadata.ParentFolderId = -1;
			}
		}
	}
}