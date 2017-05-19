

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportPrepareMetadataTask
	{
		void Execute(FileMetadata fileMetadata, ImportBatchContext importBatchContext);
	}
}
