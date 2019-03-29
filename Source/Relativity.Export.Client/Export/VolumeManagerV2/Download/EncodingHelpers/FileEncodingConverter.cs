using System.Text;
using System.Threading;
using Relativity.Import.Export.Io;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers
{
	public class FileEncodingConverter : IFileEncodingConverter
	{
		private readonly IFileEncodingRewrite _encodingRewrite;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		public FileEncodingConverter(IFileEncodingRewrite encodingRewrite, IFile fileWrapper, ILog logger)
		{
			_encodingRewrite = encodingRewrite;
			_fileWrapper = fileWrapper;
			_logger = logger;
		}

		public void Convert(string filePath, Encoding sourceEncoding, Encoding destinationEncoding, CancellationToken cancellationToken)
		{
			string tmpFilePath = $"{filePath}.tmp";
			_logger.LogVerbose("Converting file {filePath} from {srcEnc} to {dstEnc}. Using temporary file {tmpFile}.", filePath, sourceEncoding, destinationEncoding, tmpFilePath);
			try
			{
				_encodingRewrite.RewriteFile(filePath, tmpFilePath, sourceEncoding, destinationEncoding, cancellationToken);

				_logger.LogVerbose("Removing source file {filePath}.", filePath);
				_fileWrapper.Delete(filePath);
				_logger.LogVerbose("Moving temporary file from {tmpFile} to {dstFile}.", tmpFilePath, filePath);
				_fileWrapper.Move(tmpFilePath, filePath);
			}
			finally
			{
				if (_fileWrapper.Exists(tmpFilePath))
				{
					_logger.LogError("Error occurred during encoding conversion. Removing temporary file {tmpFile}.", tmpFilePath);
					_fileWrapper.Delete(tmpFilePath);
				}
			}
		}
	}
}