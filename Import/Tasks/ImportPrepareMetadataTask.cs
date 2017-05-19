using System;
using System.Linq;
using kCura.Utility.Extensions;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportPrepareMetadataTask : IImportPrepareMetadataTask
	{
		private readonly IImportMetadata _importMetadata;

		public ImportPrepareMetadataTask(IImportMetadata importMetadata)
		{
			_importMetadata = importMetadata;
		}

		public void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			string recordId = _importMetadata.PrepareFieldsAndExtractIdentityValue(fileMetadata);
			string dataGridId = GetDataGridId(fileMetadata);

			ValidateRecordId(recordId);
			ProcessDocumentMetadata(recordId, dataGridId, fileMetadata, importBatchContext.ImportContext);
		}

		private void ProcessDocumentMetadata(string recordId, string dataGridId, FileMetadata fileMetadata, ImportContext importContext)
		{
			var doc = new MetaDocument(fileMetadata.FileGuid, recordId, IndexFileInDb(fileMetadata, importContext),
				fileMetadata.FileName, fileMetadata.FullFilePath, fileMetadata.UploadFile,
				fileMetadata.LineNumber, importContext.ParentFolderId, fileMetadata.ArtifactFieldCollection,
				fileMetadata.FileIdData, fileMetadata.LineStatus, fileMetadata.DestinationDirectory, importContext.FolderPath, dataGridId);

			_importMetadata.ProcessDocumentMetadata(doc);
		}

		private bool IndexFileInDb(FileMetadata fileMetadata, ImportContext importContext)
		{
			return fileMetadata.FileExists && fileMetadata.UploadFile &&
					(!fileMetadata.FileGuid.IsNullOrEmpty() || !importContext.Settings.LoadFile.CopyFilesToDocumentRepository);
		}

		private string GetDataGridId(FileMetadata fileMetadata)
		{
			ArtifactField dataGridField = fileMetadata.ArtifactFieldCollection.get_FieldList(Relativity.FieldTypeHelper.FieldType.Varchar)
				.FirstOrDefault(item => item.DisplayName == BulkLoadFileImporter.DATA_GRID_ID_FIELD_NAME);
			
			return dataGridField != null ? dataGridField.ValueAsString : string.Empty;
		}

		private void ValidateRecordId(string identityValue)
		{
			if (identityValue.IsNullOrEmpty())
			{
				throw new BulkLoadFileImporter.IdentityValueNotSetException();
			}
			if (_importMetadata.ProcessedDocIdentifiers[identityValue] != null)
			{
				throw new LoadFileBase.IdentifierOverlapException(identityValue,
					_importMetadata.ProcessedDocIdentifiers[identityValue]);
			}
		}
	}
}
