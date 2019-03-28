using System.Text;
using System.Threading;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers
{
	public class FileEncodingConverter : IFileEncodingConverter
	{
		private readonly IFileEncodingRewrite _encodingRewrite;
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		public FileEncodingConverter(IFileEncodingRewrite encodingRewrite, IFileHelper fileHelper, ILog logger)
		{
			_encodingRewrite = encodingRewrite;
			_fileHelper = fileHelper;
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