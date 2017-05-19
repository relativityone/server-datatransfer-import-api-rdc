
using kCura.WinEDDS.Api;
using kCura.WinEDDS.Core.Import.Tasks;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchJob : IImportBatchJob
	{
		private readonly IImportNativesTask _importNativesTask;
		private readonly IImportFoldersTask _importFoldersTask;
		private readonly IImportPrepareMetadataTask _importCreateMetadataTask;
		private readonly IImportMetadataTask _importMetadataTask;

		public ImportBatchJob(IImportNativesTask importNativesTask, IImportFoldersTask importFoldersTask, 
			IImportPrepareMetadataTask importCreateMetadataTask,
			IImportMetadataTask importMetadataTask)
		{
			_importNativesTask = importNativesTask;
			_importFoldersTask = importFoldersTask;
			_importCreateMetadataTask = importCreateMetadataTask;
			_importMetadataTask = importMetadataTask;
		}

		public void Run(ImportBatchContext batchContext)
		{
			// TODO:
			foreach (FileMetadata fileMetadata in batchContext.FileMetaDataHolder)
			{
				UploadNatives(fileMetadata);
				CreateFolderStructure();
				MetadataFilesInfo metadataDoc = CreateMetadata(fileMetadata);
				UploadMetadata(metadataDoc);
			}
		}

		private void UploadMetadata(MetadataFilesInfo metadataDoc)
		{
			_importMetadataTask.Execute(metadataDoc);
		}

		private MetadataFilesInfo CreateMetadata(FileMetadata fileMetadata)
		{
			return _importCreateMetadataTask.Execute(fileMetadata);
		}

		private void CreateFolderStructure()
		{
			_importFoldersTask.Execute();
		}

		private void UploadNatives(FileMetadata fileMetaData)
		{
			_importNativesTask.Execute(fileMetaData);
		}
	}
}
