using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import.Status;
using kCura.WinEDDS.Core.Import.Tasks;

namespace kCura.WinEDDS.Core.Import
{
	public class ImportBatchJob : IImportBatchJob
	{
		private readonly IImportNativesTask _importNativesTask;
		private readonly IPushMetadataFilesTask _pushMetadataFilesTask;
		private readonly IPrepareMetadataFilesTask _prepareMetadataFilesTask;
		private readonly IBatchCleanUpTask _batchCleanUpTask;
		private readonly ICancellationProvider _cancellationProvider;

		public ImportBatchJob(IImportNativesTask importNativesTask, IPushMetadataFilesTask pushMetadataFilesTask, IPrepareMetadataFilesTask prepareMetadataFilesTask,
			IBatchCleanUpTask batchCleanUpTask, ICancellationProvider cancellationProvider)
		{
			_importNativesTask = importNativesTask;
			_pushMetadataFilesTask = pushMetadataFilesTask;
			_cancellationProvider = cancellationProvider;
			_batchCleanUpTask = batchCleanUpTask;
			_prepareMetadataFilesTask = prepareMetadataFilesTask;
		}

		public void Run(ImportBatchContext batchContext)
		{
			_cancellationProvider.ThrowIfCancellationRequested();

			IDictionary<FileMetadata, UploadResult> result = UploadNatives(batchContext)
				.OrderBy(item => item.Key.LineNumber)
				.ToDictionary(keyValSelector => keyValSelector.Key, keyValSelector => keyValSelector.Value);

			PrepareMetadata(result);

			UploadMetadata(batchContext);

			CleanUp();
		}

		private void PrepareMetadata(IDictionary<FileMetadata, UploadResult> result)
		{
			_prepareMetadataFilesTask.Execute(result);
		}

		private IDictionary<FileMetadata, UploadResult> UploadNatives(ImportBatchContext importBatchContext)
		{
			return _importNativesTask.Execute(importBatchContext);
		}

		private void UploadMetadata(ImportBatchContext batchContext)
		{
			_pushMetadataFilesTask.PushMetadataFiles(batchContext);
		}

		private void CleanUp()
		{
			_batchCleanUpTask.Execute();
		}
	}
}