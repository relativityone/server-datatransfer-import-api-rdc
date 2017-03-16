using ZetaLongPaths;

namespace kCura.WinEDDS.Core.IO
{
	public class LongPathDirectoryHelper : IDirectoryHelper
	{
		public bool Exists(string directoryPath)
		{
			return ZlpIOHelper.DirectoryExists(directoryPath);
		}

		public void CreateDirectory(string directoryPath)
		{
			ZlpIOHelper.CreateDirectory(directoryPath);
		}
	}
}