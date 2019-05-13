namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System;
	using System.IO;
	using System.Text;

	using kCura.WinEDDS.Exceptions;

	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	public class StreamFactory : IStreamFactory
	{
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		public StreamFactory(IFile fileWrapper, ILog logger)
		{
			_fileWrapper = fileWrapper;
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
					fileStream = _fileWrapper.ReopenAndTruncate(path, lastStreamWriterPosition);
				}
				else
				{
					fileStream = _fileWrapper.Create(path, false);
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