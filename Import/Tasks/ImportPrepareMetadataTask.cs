using System;
using System.Linq;
using kCura.Utility.Extensions;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportPrepareMetadataTask : IImportPrepareMetadataTask
	{
		private readonly ImportContext _importContext;
		private readonly IImportMetadata _importMetadata;

		public ImportPrepareMetadataTask(ImportContext importContext, IImportMetadata importMetadata)
		{
			_importContext = importContext;
			_importMetadata = importMetadata;
		}

		public void Execute(FileMetadata fileMetadata)
		{
			string recordId = _importMetadata.PrepareFieldsAndExtractIdentityValue(fileMetadata);
			string dataGridId = GetDataGridId(fileMetadata);

			ValidateRecordId(recordId);
			ProcessDocumentMetadata(recordId, dataGridId, fileMetadata);
		}

		private void ProcessDocumentMetadata(string recordId, string dataGridId, FileMetadata fileMetadata)
		{
			var doc = new MetaDocument(fileMetadata.FileGuid, recordId, IndexFileInDb(fileMetadata),
				fileMetadata.FileName, fileMetadata.FullFilePath, fileMetadata.UploadFile,
				fileMetadata.LineNumber, _importContext.ParentFolderId, fileMetadata.ArtifactFieldCollection,
				fileMetadata.FileIdData, fileMetadata.LineStatus, fileMetadata.DestinationDirectory, _importContext.FolderPath, dataGridId);

			_importMetadata.ProcessDocumentMetadata(doc);
		}

		private bool IndexFileInDb(FileMetadata fileMetadata)
		{
			return fileMetadata.FileExists && fileMetadata.UploadFile &&
					(!fileMetadata.FileGuid.IsNullOrEmpty() || !_importContext.Settings.LoadFile.CopyFilesToDocumentRepository);
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
