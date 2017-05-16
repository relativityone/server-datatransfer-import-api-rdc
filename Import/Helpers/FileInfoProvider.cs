

using kCura.OI.FileID;

namespace kCura.WinEDDS.Core.Import.Helpers
{
	public class FileInfoProvider : IFileInfoProvider
	{
		public FileIDData GetFileId(string fileName)
		{
			return Manager.Instance.GetFileIDDataByFilePath(fileName);
		}
	}
}
