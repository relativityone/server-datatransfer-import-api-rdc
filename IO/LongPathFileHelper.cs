using System.IO;
using ZetaLongPaths;
using ZetaLongPaths.Native;

namespace kCura.WinEDDS.Core.IO
{
	public class LongPathFileHelper : IFileHelper
	{
		public FileStream Create(string filePath)
		{
			var fileInfo = new ZlpFileInfo(filePath);
			return fileInfo.OpenCreate();
		}

		public FileStream Create(string filePath, bool append)
		{
			CreationDisposition disposition = append ? CreationDisposition.OpenExisting : CreationDisposition.CreateAlways;
			ZetaLongPaths.Native.FileAccess fileAccess = ZetaLongPaths.Native.FileAccess.GenericRead | ZetaLongPaths.Native.FileAccess.GenericWrite;

			var fileHandle = ZlpIOHelper.CreateFileHandle(filePath, disposition, fileAccess, ZetaLongPaths.Native.FileShare.None);
			FileStream fileStream = new FileStream(fileHandle, System.IO.FileAccess.ReadWrite);

			if (append)
			{
				fileStream.Seek(0L, SeekOrigin.End);
			}

			return fileStream;
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