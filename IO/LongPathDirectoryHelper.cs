using ZetaLongPaths;

namespace kCura.WinEDDS.Core.IO
{
	public class LongPathDirectoryHelper : IDirectoryHelper
	{
		private object _lockObject = new object();

		public bool Exists(string directoryPath)
		{
			return ZlpIOHelper.DirectoryExists(directoryPath);
		}

		public void CreateDirectory(string directoryPath)
		{
			lock (_lockObject)
			{
				if (!Exists(directoryPath))
				{
					ZlpIOHelper.CreateDirectory(directoryPath);
				}
			}
		}
	}
}