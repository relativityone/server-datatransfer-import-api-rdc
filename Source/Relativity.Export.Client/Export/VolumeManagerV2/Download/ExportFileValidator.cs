namespace Relativity.Export.VolumeManagerV2.Download
{
	using Relativity.Export.VolumeManagerV2.Repository;
	using Relativity.Import.Export.Io;
	using Relativity.Import.Export.Process;
	using Relativity.Logging;

	using kCura.WinEDDS;

	public class ExportFileValidator : IExportFileValidator
	{
		private readonly ExportFile _exportSettings;
		private readonly IStatus _status;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;
		private readonly IExportRequestRepository _exportRequestRepository;

		public ExportFileValidator(ExportFile exportSettings, IExportRequestRepository exportRequestRepository, IStatus status, IFile fileWrapper, ILog logger)
		{
			_exportSettings = exportSettings;
			_exportRequestRepository = exportRequestRepository;
			_status = status;
			_fileWrapper = fileWrapper;
			_logger = logger;
		}

		public bool CanExport(string destinationLocation, string warningUserMessage)
		{
			if (_fileWrapper.Exists(destinationLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_logger.LogVerbose($"Overwriting document {destinationLocation}. Removing already existing file.");
					_fileWrapper.Delete(destinationLocation);
					_status.WriteStatusLine(EventType.Status, warningUserMessage, false);
					return true;
				}
				else
				{
					_logger.LogWarning($"{destinationLocation} already exists. Skipping file export.");
					_status.WriteWarning($"{destinationLocation} already exists. Skipping file export.");
					return false;
				}
			}

			if (_exportRequestRepository.AnyRequestForLocation(destinationLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_logger.LogVerbose($"Document {destinationLocation} already exists in current export requests for batch. Skipping file.");
					_status.WriteStatusLine(EventType.Status, warningUserMessage, false);
				}
				else
				{
					_logger.LogWarning($"{destinationLocation} already exists. Skipping file export.");
					_status.WriteWarning($"{destinationLocation} already exists. Skipping file export.");
				}

				return false;
			}

			return true;
		}
	}
}