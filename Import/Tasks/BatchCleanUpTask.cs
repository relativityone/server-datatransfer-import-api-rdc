namespace kCura.WinEDDS.Core.Import.Tasks
{
	public class BatchCleanUpTask : IBatchCleanUpTask
	{
		private readonly ImportBatchContext _batchContext;

		private readonly IFileHelper _fileHelper;

		public BatchCleanUpTask(ImportBatchContext batchContext, IFileHelper fileHelper)
		{
			_batchContext = batchContext;
			_fileHelper = fileHelper;
		}

		public void Execute()
		{
			foreach (var metadataFilesInfo in _batchContext.MetadataFilesInfo)
			{
				_fileHelper.Delete(metadataFilesInfo.NativeFilePath.FullFilePath);
				_fileHelper.Delete(metadataFilesInfo.CodeFilePath.FullFilePath);
				_fileHelper.Delete(metadataFilesInfo.ObjectFilePath.FullFilePath);
				_fileHelper.Delete(metadataFilesInfo.DataGridFilePath.FullFilePath);
			}
		}
	}
}