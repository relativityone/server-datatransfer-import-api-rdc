using System.IO;
using System.Text;
using System.Threading;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers
{
	public class FileEncodingRewrite : IFileEncodingRewrite
	{
		private const int _BUFFER_SIZE = 4096;

		public void RewriteFile(string filePath, string tmpFilePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken)
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
							return;
						}

						writer.Write(buf, 0, count);
					}
				}
			}
		}
	}
}