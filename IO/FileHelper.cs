﻿using System.IO;

namespace kCura.WinEDDS.Core.IO
{
	public class FileHelper : IFileHelper
	{
		public FileStream Create(string filePath)
		{
			return File.Create(filePath);
		}

		public FileStream Create(string filePath, bool append)
		{
			FileMode mode = append ? FileMode.Append : FileMode.Create;
			return new FileStream(filePath, mode, FileAccess.ReadWrite, FileShare.None);
		}

		public void Delete(string filePath)
		{
			File.Delete(filePath);
		}

		public void Copy(string sourceFilePath, string destinationFilePath)
		{
			Copy(sourceFilePath, destinationFilePath, false);
		}

		public void Copy(string sourceFilePath, string destinationFilePath, bool overwrite)
		{
			File.Copy(sourceFilePath, destinationFilePath, overwrite);
		}

		public bool Exists(string filePath)
		{
			return File.Exists(filePath);
		}
	}
}