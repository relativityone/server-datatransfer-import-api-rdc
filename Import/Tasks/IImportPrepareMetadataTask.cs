

namespace kCura.WinEDDS.Core.Import.Tasks
{
	public interface IImportPrepareMetadataTask
	{
		MetadataFilesInfo Execute(FileMetadata fileMetadata);
	}
}
