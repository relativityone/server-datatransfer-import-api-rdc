using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import.Status;

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

		private int _counter;

		public PrepareMetadataFilesTask(ImportBatchContext batchContext, IImportExceptionHandlerExec importExceptionHandlerExec, ICancellationProvider cancellationProvider,
			IImportFoldersTask importFoldersTask, IImportPrepareMetadataTask importCreateMetadataTask, IImportMetadata importMetadata, ITransferConfig transferConfig)
		{
			_importExceptionHandlerExec = importExceptionHandlerExec;
			_cancellationProvider = cancellationProvider;
			_importFoldersTask = importFoldersTask;
			_importCreateMetadataTask = importCreateMetadataTask;
			_importMetadata = importMetadata;
			_transferConfig = transferConfig;
			_batchContext = batchContext;
		}

		public void Execute(IDictionary<FileMetadata, UploadResult> uploadResults)
		{
			CreateNewMetadataFiles();
			foreach (var keyValuePair in uploadResults.Where(x => x.Value.Success))
			{
				_importExceptionHandlerExec.TryCatchExec(() =>
				{
					_cancellationProvider.ThrowIfCancellationRequested();
					if (MetadataFilesExceededMaxSize())
					{
						CloseMetadataFiles();
						CreateNewMetadataFiles();
					}
					CreateFolderStructure(keyValuePair.Key);
					CreateMetadata(keyValuePair.Key);
					_counter++;
				});
			}
			CloseMetadataFiles();
		}

		private void CreateNewMetadataFiles()
		{
			_counter = 0;
			var metadataFilesInfo = _importMetadata.InitNewMetadataProcess();
			_batchContext.MetadataFilesInfo.Add(metadataFilesInfo);
		}

		private void CloseMetadataFiles()
		{
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