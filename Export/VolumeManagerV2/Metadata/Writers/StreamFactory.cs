using System;
using System.IO;
using System.Text;
using kCura.WinEDDS.Exceptions;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class StreamFactory
	{
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		public StreamFactory(IFileHelper fileHelper, ILog logger)
		{
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public StreamWriter Create(StreamWriter currentStreamWriter, long lastStreamWriterPosition, string path, Encoding encoding, bool append)
		{
			if (currentStreamWriter != null)
			{
				try
				{
					currentStreamWriter.Dispose();
				}
				catch (Exception ex)
				{
					_logger.LogWarning(ex, "Failed to close broken stream.");
				}
			}
			try
			{
				_logger.LogVerbose("Creating new file stream {path}.", path);
				FileStream fileStream;
				if (append)
				{
					_logger.LogVerbose("Opening file in append mode with given position, so truncating file.");
					fileStream = _fileHelper.CreateAndTruncate(path, lastStreamWriterPosition);
				}
				else
				{
					fileStream = _fileHelper.Create(path, false);
				}
				StreamWriter newWriter = new StreamWriter(fileStream, encoding);
				newWriter.BaseStream.Position = lastStreamWriterPosition;
				return newWriter;
			}
			catch (IOException ex)
			{
				_logger.LogError(ex, "Failed to initialize stream {path} with encoding {encoding}.", path, encoding);
				throw new FileWriteException(FileWriteException.DestinationFile.Generic, ex);
			}
		}
	}
}