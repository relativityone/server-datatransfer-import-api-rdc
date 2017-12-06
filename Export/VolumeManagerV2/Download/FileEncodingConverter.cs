using System.IO;
using System.Text;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FileEncodingConverter
	{
		private const int _BUFFER_SIZE = 4096;

		private readonly IFileHelper _fileHelper;

		public FileEncodingConverter(IFileHelper fileHelper)
		{
			_fileHelper = fileHelper;
		}

		public void Convert(string filePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken)
		{
			string tmpFilePath = $"{filePath}.tmp";
			try
			{
				using (StreamReader reader = new StreamReader(filePath, sourceEncoding))
				{
					using (StreamWriter writer = new StreamWriter(tmpFilePath, false, destinationEncoding))
					{
						char[] buf = new char[_BUFFER_SIZE];
						while (true)
						{
							if (cancellationToken.IsCancellationRequested)
							{
								return;
							}
							int count = reader.Read(buf, 0, buf.Length);
							if (count == 0)
							{
								break;
							}

							writer.Write(buf, 0, count);
						}
					}
				}
				_fileHelper.Delete(filePath);
				_fileHelper.Move(tmpFilePath, filePath);
			}
			finally
			{
				if (_fileHelper.Exists(tmpFilePath))
				{
					_fileHelper.Delete(tmpFilePath);
				}
			}
		}
	}
}