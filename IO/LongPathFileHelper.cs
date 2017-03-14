using System.IO;
using ZetaLongPaths;

namespace kCura.WinEDDS.Core.IO
{
	public class LongPathFileHelper : IFileHelper
	{
		public FileStream Create(string filePath)
		{
			var fileInfo = new ZlpFileInfo(filePath);
			return fileInfo.OpenCreate();
		}

		public void Delete(string filePath)
		{
			ZlpIOHelper.DeleteFile(filePath);
		}

		public void Copy(string sourceFilePath, string destinationFilePath)
		{
			Copy(sourceFilePath, destinationFilePath, false);
		}

		public void Copy(string sourceFilePath, string destinationFilePath, bool overwrite)
		{
			ZlpIOHelper.CopyFile(sourceFilePath, destinationFilePath, overwrite);
		}

		public bool Exists(string filePath)
		{
			return ZlpIOHelper.FileExists(filePath);
		}
	}
}