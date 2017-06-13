using System.Linq;
using kCura.Utility.Extensions;
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import.Statistics;
using Relativity;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class ImportPrepareMetadataTask : IImportPrepareMetadataTask
	{
		private readonly IImportMetadata _importMetadata;
		private readonly IMetadataStatisticsHandler _metadataStatisticsHandler;
		private readonly ILog _log;

		public ImportPrepareMetadataTask(IImportMetadata importMetadata, IMetadataStatisticsHandler metadataStatisticsHandler, ILog log)
		{
			_importMetadata = importMetadata;
			_metadataStatisticsHandler = metadataStatisticsHandler;
			_log = log;
		}

		public void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			_log.LogDebug($"Creating metadata for record at line {fileMetadata.LineNumber}");

			string recordId = _importMetadata.PrepareFieldsAndExtractIdentityValue(fileMetadata);
			string dataGridId = GetDataGridId(fileMetadata);

			ValidateRecordId(recordId, fileMetadata);

			_log.LogDebug($"Extracted record identity: {recordId} , data grid id: {dataGridId}");
			ProcessDocumentMetadata(recordId, dataGridId, fileMetadata, importBatchContext.ImportContext);

			_metadataStatisticsHandler.RaiseFileMetadataProcessedEvent(recordId, fileMetadata.LineNumber);
			_log.LogDebug($"Metadata created for record at line {fileMetadata.LineNumber}");
		}

		private void ProcessDocumentMetadata(string recordId, string dataGridId, FileMetadata fileMetadata, ImportContext importContext)
		{
			var doc = new MetaDocument(fileMetadata.FileGuid, recordId, IndexFileInDb(fileMetadata, importContext),
				fileMetadata.FileName, fileMetadata.FullFilePath, fileMetadata.UploadFile,
				fileMetadata.LineNumber, fileMetadata.ParentFolderId, fileMetadata.ArtifactFieldCollection,
				fileMetadata.FileIdData, fileMetadata.LineStatus, fileMetadata.DestinationDirectory, fileMetadata.FolderPath, dataGridId);

			_importMetadata.ProcessDocumentMetadata(doc);

			_log.LogDebug($"Metadata processing complete for record at line {fileMetadata.LineNumber}");
		}

		private bool IndexFileInDb(FileMetadata fileMetadata, ImportContext importContext)
		{
			return fileMetadata.FileExists && fileMetadata.UploadFile &&
					(!fileMetadata.FileGuid.IsNullOrEmpty() || !importContext.Settings.LoadFile.CopyFilesToDocumentRepository);
		}

		private string GetDataGridId(FileMetadata fileMetadata)
		{
			ArtifactField dataGridField = fileMetadata.ArtifactFieldCollection.get_FieldList(FieldTypeHelper.FieldType.Varchar)
				.FirstOrDefault(item => item.DisplayName == BulkLoadFileImporter.DATA_GRID_ID_FIELD_NAME);

			return dataGridField != null ? dataGridField.ValueAsString : string.Empty;
		}

		private void ValidateRecordId(string identityValue, FileMetadata fileMetadata)
		{
			if (identityValue.IsNullOrEmpty())
			{
				_log.LogDebug($"Record identity at line {fileMetadata.LineNumber} is null or empty");
				throw new BulkLoadFileImporter.IdentityValueNotSetException();
			}
			if (_importMetadata.ProcessedDocIdentifiers[identityValue] != null)
			{
				_log.LogDebug($"Found duplicated record identity: {identityValue} at line {fileMetadata.LineNumber}");
				throw new LoadFileBase.IdentifierOverlapException(identityValue,
					_importMetadata.ProcessedDocIdentifiers[identityValue]);
			}
		}
	}
}