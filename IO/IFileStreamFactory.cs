using System.IO;

namespace kCura.WinEDDS.Core.IO
{
	public interface IFileStreamFactory
	{
		FileStream Create(string filePath);
	}
}