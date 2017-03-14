using System.IO;

namespace kCura.WinEDDS.Core.IO
{
	public class FileStreamFactory : IFileStreamFactory
	{
		private readonly IFileHelper _fileHelper;

		public FileStreamFactory(IFileHelper fileHelper)
		{
			_fileHelper = fileHelper;
		}

		public FileStream Create(string filePath)
		{
			return _fileHelper.Create(filePath);
		}

		public FileStream Create(string filePath, bool append)
		{
			return _fileHelper.Create(filePath, append);
		}
	}
}