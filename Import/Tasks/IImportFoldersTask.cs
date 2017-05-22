namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportFoldersTask
	{
		void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext);
	}
}