using System;
using System.IO;
using kCura.Utility;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exceptions;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers
{
	public class ErrorFileWriter : IErrorFileWriter
	{
		private StreamWriter _streamWriter;

		private readonly StreamFactory _streamFactory;
		private readonly ErrorFileDestinationPath _errorFileDestinationPath;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ErrorFileWriter(StreamFactory streamFactory, ErrorFileDestinationPath errorFileDestinationPath, IStatus status, ILog logger)
		{
			_streamFactory = streamFactory;
			_errorFileDestinationPath = errorFileDestinationPath;
			_logger = logger;
			_status = status;
		}

		public void Write(ExportFileType type, string recordIdentifier, string fileLocation, string errorText)
		{
			try
			{
				InitializeStream();

				WriteLine(type, recordIdentifier, fileLocation, errorText);
			}
			catch (IOException ex)
			{
				_logger.LogError(ex, "Failed to create or use Error file stream.");
				throw new FileWriteException(FileWriteException.DestinationFile.Errors, ex);
			}
			_status.WriteError($"{type} - Document [{recordIdentifier}] - File [{fileLocation}] - Error: {Environment.NewLine}{errorText}");
		}

		private void InitializeStream()
		{
			if (_streamWriter == null)
			{
				_logger.LogVerbose("Creating stream for error file in {destination}.", _errorFileDestinationPath.Path);
				_streamWriter = _streamFactory.Create(_streamWriter, 0, _errorFileDestinationPath.Path, _errorFileDestinationPath.Encoding, false);
				WriteHeader();
			}
		}

		private void WriteHeader()
		{
			string header = FormatErrorLine("File Type", "Document Identifier", "File Guid", "Error Description");
			_streamWriter.WriteLine(header);
		}

		private void WriteLine(ExportFileType type, string recordIdentifier, string fileLocation, string errorText)
		{
			string line = FormatErrorLine(type.ToString(), recordIdentifier, fileLocation, Strings.ToCsvCellContents(errorText));
			_logger.LogError(line);
			_streamWriter.WriteLine(line);
		}

		private string FormatErrorLine(string fileType, string documentIdentifier, string fileGuid, string errorDescription)
		{
			return $"\"{fileType}\",\"{documentIdentifier}\",\"{fileGuid}\",\"{errorDescription}\"";
		}

		public void Dispose()
		{
			_streamWriter?.Dispose();
		}

		public enum ExportFileType
		{
			Image,
			Native,
			Generic
		}
	}
}