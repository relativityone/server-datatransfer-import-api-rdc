namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportNativesTask
	{
		void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext);
	}
}