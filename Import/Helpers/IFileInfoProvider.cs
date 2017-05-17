
namespace kCura.WinEDDS.Core.Import.Helpers
{
	public interface IFileInfoProvider
	{
		OI.FileID.FileIDData GetFileId(string fileName);

		long GetFileSize(FileMetadata fileMetadata);
	}
}
