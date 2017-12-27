using System.IO;
using System.Text;
using System.Threading;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class FileEncodingConverter
	{
		private const int _BUFFER_SIZE = 4096;

		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		public FileEncodingConverter(IFileHelper fileHelper, ILog logger)
		{
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public void Convert(string filePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken)
		{
			string tmpFilePath = $"{filePath}.tmp";
			_logger.LogVerbose("Converting file {filePath} from {srcEnc} to {dstEnc}. Using temporary file {tmpFile}.", filePath, sourceEncoding, destinationEncoding, tmpFilePath);
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

				_logger.LogVerbose("Removing source file {filePath}.", filePath);
				_fileHelper.Delete(filePath);
				_logger.LogVerbose("Moving temporary file from {tmpFile} to {dstFile}.", tmpFilePath, filePath);
				_fileHelper.Move(tmpFilePath, filePath);
			}
			finally
			{
				if (_fileHelper.Exists(tmpFilePath))
				{
					_logger.LogError("Error occurred during encoding conversion. Removing temporary file {tmpFile}.", tmpFilePath);
					_fileHelper.Delete(tmpFilePath);
				}
			}
		}
	}
}