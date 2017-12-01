using System;
using System.IO;
using System.Text;
using kCura.WinEDDS.Exceptions;
using kCura.WinEDDS.IO;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class StreamFactory
	{
		private readonly IFileStreamFactory _fileStreamFactory;
		private readonly ILog _logger;

		public StreamFactory(IFileStreamFactory fileStreamFactory, ILog logger)
		{
			_fileStreamFactory = fileStreamFactory;
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
				StreamWriter newWriter = new StreamWriter(path, append, encoding);
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