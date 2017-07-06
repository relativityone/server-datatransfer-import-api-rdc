using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import.Status;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class PrepareMetadataFilesTask : IPrepareMetadataFilesTask
	{
		private readonly ImportBatchContext _batchContext;

		private readonly IImportExceptionHandlerExec _importExceptionHandlerExec;
		private readonly ICancellationProvider _cancellationProvider;
		private readonly IImportFoldersTask _importFoldersTask;
		private readonly IImportPrepareMetadataTask _importCreateMetadataTask;
		private readonly IImportMetadata _importMetadata;
		private readonly ITransferConfig _transferConfig;
		private readonly ILog _log;

		private int _counter;

		public PrepareMetadataFilesTask(ImportBatchContext batchContext, IImportExceptionHandlerExec importExceptionHandlerExec, ICancellationProvider cancellationProvider,
			IImportFoldersTask importFoldersTask, IImportPrepareMetadataTask importCreateMetadataTask, IImportMetadata importMetadata, ITransferConfig transferConfig,
			ILog log)
		{
			_importExceptionHandlerExec = importExceptionHandlerExec;
			_cancellationProvider = cancellationProvider;
			_importFoldersTask = importFoldersTask;
			_importCreateMetadataTask = importCreateMetadataTask;
			_importMetadata = importMetadata;
			_transferConfig = transferConfig;
			_log = log;
			_batchContext = batchContext;
		}

		public void Execute(IDictionary<FileMetadata, UploadResult> uploadResults)
		{
			_log.LogInformation("Metadata creation started");
			CreateNewMetadataFiles();
			foreach (var keyValuePair in uploadResults.Where(x => x.Value.Success))
			{
				_importExceptionHandlerExec.TryCatchExecNonFatal(() =>
				{
					_cancellationProvider.ThrowIfCancellationRequested();
					if (MetadataFilesExceededMaxSize())
					{
						_log.LogInformation($"Metadata file chunk ready for upload. Reached batch size or {_transferConfig.ImportBatchMaxVolume} metadata file size limit (Mb)");
						CloseMetadataFiles();
						CreateNewMetadataFiles();
					}
					CreateFolderStructure(keyValuePair.Key);
					CreateMetadata(keyValuePair.Key);
					_counter++;
				});
			}
			CloseMetadataFiles();
			_log.LogInformation("Metadata creation completed");
		}

		private void CreateNewMetadataFiles()
		{
			_log.LogDebug("Creating new metadata temp files");
			_counter = 0;
			var metadataFilesInfo = _importMetadata.InitNewMetadataProcess();
			_batchContext.MetadataFilesInfo.Add(metadataFilesInfo);
		}

		private void CloseMetadataFiles()
		{
			_log.LogDebug($"Closing metadata temp files. Total items: {_counter}");
			_batchContext.MetadataFilesInfo.Last().BatchSize = _counter;
			_importMetadata.EndMetadataProcess();
		}

		private bool MetadataFilesExceededMaxSize()
		{
			return _importMetadata.CurrentMetadataFileStreamLength() > _transferConfig.ImportBatchMaxVolume;
		}

		private void CreateMetadata(FileMetadata fileMetadata)
		{
			_importCreateMetadataTask.Execute(fileMetadata, _batchContext);
		}

		private void CreateFolderStructure(FileMetadata fileMetadata)
		{
			_importFoldersTask.Execute(fileMetadata, _batchContext);
		}
	}
}