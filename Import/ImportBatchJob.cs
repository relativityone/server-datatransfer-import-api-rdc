using kCura.WinEDDS.Core.Import.Tasks;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchJob : IImportBatchJob
	{
		private readonly IImportNativesTask _importNativesTask;
		private readonly IImportFoldersTask _importFoldersTask;
		private readonly IImportPrepareMetadataTask _importCreateMetadataTask;
		private readonly IPushMetadataFilesTask _pushMetadataFilesTask;
		private readonly IImportMetadata _importMetadata;

		public ImportBatchJob(IImportNativesTask importNativesTask, IImportFoldersTask importFoldersTask, 
			IImportPrepareMetadataTask importCreateMetadataTask,
			IPushMetadataFilesTask pushMetadataFilesTask, IImportMetadata importMetadata)
		{
			_importNativesTask = importNativesTask;
			_importFoldersTask = importFoldersTask;
			_importCreateMetadataTask = importCreateMetadataTask;
			_pushMetadataFilesTask = pushMetadataFilesTask;
			_importMetadata = importMetadata;
		}

		public void Run(ImportBatchContext batchContext)
		{
			InitializeBatch(batchContext);
			foreach (FileMetadata fileMetadata in batchContext.FileMetaDataHolder)
			{
				UploadNatives(fileMetadata);
				CreateFolderStructure(batchContext);
				CreateMetadata(fileMetadata, batchContext);
			}
			CompleteMetadataProcess();
			UploadMetadata(batchContext);
		}

		private void UploadMetadata(ImportBatchContext batchContext)
		{
			_pushMetadataFilesTask.PushMetadataFiles(batchContext);
		}

		private void CompleteMetadataProcess()
		{
			_importMetadata.SubmitMetadataProcess();
		}

		private void InitializeBatch(ImportBatchContext batchContext)
		{
			batchContext.MetadataFilesInfo = _importMetadata.InitMetadataProcess();
		}

		private void CreateMetadata(FileMetadata fileMetadata, ImportBatchContext batchContext)
		{
			_importCreateMetadataTask.Execute(fileMetadata, batchContext);
		}

		private void CreateFolderStructure(ImportBatchContext importBatchContext)
		{
			_importFoldersTask.Execute(importBatchContext);
		}

		private void UploadNatives(FileMetadata fileMetaData)
		{
			_importNativesTask.Execute(fileMetaData);
		}
	}
}
