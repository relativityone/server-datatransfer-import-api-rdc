using System.IO;

namespace kCura.WinEDDS.Core.IO
{
	public interface IFileHelper
	{
		FileStream Create(string filePath);
		void Delete(string filePath);
		void Copy(string sourceFilePath, string destinationFilePath);
		void Copy(string sourceFilePath, string destinationFilePath, bool overwrite);
		bool Exists(string filePath);
		FileStream Create(string filePath, bool append);
	}
}