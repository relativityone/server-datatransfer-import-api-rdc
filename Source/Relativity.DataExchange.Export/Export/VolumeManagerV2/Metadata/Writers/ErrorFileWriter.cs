namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers
{
	using System;
	using System.IO;

	using Castle.Core;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exceptions;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.Logging;

	public class ErrorFileWriter : IErrorFileWriter
	{
		private StreamWriter _streamWriter;

		private readonly IStreamFactory _streamFactory;
		private readonly IDestinationPath _errorFileDestinationPath;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ErrorFileWriter(IStreamFactory streamFactory, ErrorFileDestinationPath errorFileDestinationPath, IStatus status, ILog logger) : this(streamFactory,
			(IDestinationPath) errorFileDestinationPath, status, logger)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="streamFactory"></param>
		/// <param name="errorFileDestinationPath"></param>
		/// <param name="status"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public ErrorFileWriter(IStreamFactory streamFactory, IDestinationPath errorFileDestinationPath, IStatus status, ILog logger)
		{
			_streamFactory = streamFactory;
			_errorFileDestinationPath = errorFileDestinationPath;
			_logger = logger;
			_status = status;
		}

		public void Write(ExportFileType type, ObjectExportInfo documentInfo, string fileLocation, string errorText)
		{
			string recordIdentifier = documentInfo?.IdentifierValue ?? string.Empty;
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

			this.UpdateStatus(documentInfo, type, recordIdentifier, fileLocation, errorText);
		}

		private void UpdateStatus(ObjectExportInfo documentInfo, ExportFileType type, string recordIdentifier, string fileLocation, string errorText)
		{
			string statusMessage =
				$"{type} - Document [{recordIdentifier}] - File [{fileLocation}] - Error: {Environment.NewLine}{errorText}";

			// Don't sent Error status update on the document if it was already triggered to prevent failed record count duplication issue
			if (documentInfo == null || !documentInfo.DocumentError)
			{
				this._status.WriteError(statusMessage);
			}
			else
			{
				this._status.WriteWarning(statusMessage);
			}
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
			string line = FormatErrorLine(type.ToString(), recordIdentifier, fileLocation, errorText.ToCsvCellContents());
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