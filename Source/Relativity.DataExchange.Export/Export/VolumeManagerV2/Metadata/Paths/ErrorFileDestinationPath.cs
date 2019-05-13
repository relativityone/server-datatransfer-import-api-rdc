﻿namespace Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths
{
	using System.Text;

	using Relativity.Logging;

	using kCura.WinEDDS.Exceptions;
	using kCura.WinEDDS;

	public class ErrorFileDestinationPath : IDestinationPath, IErrorFile
	{
		private string _errorFilePath;
		private readonly ExportFile _exportSettings;
		private readonly ILog _logger;

		public ErrorFileDestinationPath(ExportFile exportSettings, ILog logger)
		{
			_exportSettings = exportSettings;
			_logger = logger;
		}

		public string Path
		{
			get
			{
				if (string.IsNullOrEmpty(_errorFilePath))
				{
					_errorFilePath = TempFileBuilder.GetTempFileName(TempFileConstants.ErrorsFileNameSuffix);
					_logger.LogVerbose("Creating new path {path} for error file.", _errorFilePath);
				}

				return _errorFilePath;
			}
		}

		public Encoding Encoding => _exportSettings.LoadFileEncoding;
		public FileWriteException.DestinationFile DestinationFileType => FileWriteException.DestinationFile.Errors;

		public bool IsErrorFileCreated()
		{
			return !string.IsNullOrEmpty(_errorFilePath);
		}

		string IErrorFile.Path()
		{
			return Path;
		}
	}
}