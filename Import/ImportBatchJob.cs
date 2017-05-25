using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import.Status;
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
		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec;
		private readonly IImportStatusManager _importStatusManager;

		public ImportBatchJob(IImportNativesTask importNativesTask, IImportFoldersTask importFoldersTask,
			IImportPrepareMetadataTask importCreateMetadataTask,
			IPushMetadataFilesTask pushMetadataFilesTask, IImportMetadata importMetadata,
			IImportExceptionHandlerExec importExceptionHandlerExec, IImportStatusManager importStatusManager)
		{
			_importNativesTask = importNativesTask;
			_importFoldersTask = importFoldersTask;
			_importCreateMetadataTask = importCreateMetadataTask;
			_pushMetadataFilesTask = pushMetadataFilesTask;
			_importMetadata = importMetadata;
			_importExceptionHandlerExec = importExceptionHandlerExec;
			_importStatusManager = importStatusManager;
		}

		public void Run(ImportBatchContext batchContext)
		{
			InitializeBatch(batchContext);

			IDictionary<FileMetadata, UploadResult> result = UploadNatives(batchContext)
				.OrderBy(item => item.Key.LineNumber)
				.ToDictionary(keyValSelector => keyValSelector.Key, keyValSelector => keyValSelector.Value);

			foreach (var keyValuePair in result.Where(x => x.Value.Success))
			{
				_importExceptionHandlerExec.TryCatchExec(() =>
				{
					CreateFolderStructure(keyValuePair.Key, batchContext);
					CreateMetadata(keyValuePair.Key, batchContext);

					
				});
			}
			_importExceptionHandlerExec.TryCatchExec(() =>
			{
				CompleteMetadataProcess();
				UploadMetadata(batchContext);
			});
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

		private void CreateFolderStructure(FileMetadata fileMetadata, ImportBatchContext importBatchContext)
		{
			_importFoldersTask.Execute(fileMetadata, importBatchContext);
		}

		private IDictionary<FileMetadata, UploadResult> UploadNatives(ImportBatchContext importBatchContext)
		{
			return _importNativesTask.Execute(importBatchContext);
		}
	}
}