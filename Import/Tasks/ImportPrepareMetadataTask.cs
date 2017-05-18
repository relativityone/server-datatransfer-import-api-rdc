
using System;
using System.Linq;
using kCura.Utility.Extensions;
using kCura.WinEDDS.Api;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportPrepareMetadataTask : IImportPrepareMetadataTask
	{
		private readonly IImportMetadata _importMetadata;
		private readonly IImporterSettings _importerSettings;

		public ImportPrepareMetadataTask(IImportMetadata importMetadata, IImporterSettings importerSettings)
		{
			_importMetadata = importMetadata;
			_importerSettings = importerSettings;
		}

		public MetadataFilesInfo Execute(FileMetadata fileMetadata)
		{
			var metadataFilesInfo = new MetadataFilesInfo();

			string recordId = _importMetadata.PrepareFieldsAndExtractIdentityValue(fileMetadata);
			ValidateRecordId(recordId);

			string dataGridId = GetDataGridId(fileMetadata);

			//var doc = new MetaDocument(fileMetadata.FileGuid, recordId, IndexFileInDb(fileMetadata),
			//	fileMetadata.FileName, fileMetadata.FileName, uploadFile, CurrentLineNumber, parentFolderID, record, oixFileIdData, lineStatus, destinationVolume, folderPath, dataGridID)
			
			return metadataFilesInfo;
		}

		//private bool IndexFileInDb(FileMetadata fileMetadata)
		//{
		//	return fileMetadata.FileExists && fileMetadata.UploadFile &&
		//			(!fileMetadata.FileGuid.IsNullOrEmpty() || !_importerSettings.Settings.CopyFilesToDocumentRepository);
		//}

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
