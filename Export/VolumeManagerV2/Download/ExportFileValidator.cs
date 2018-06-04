using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class ExportFileValidator : IExportFileValidator
	{
		private readonly ExportFile _exportSettings;
		private readonly IStatus _status;
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;
		private readonly IExportRequestRepository _exportRequestRepository;

		public ExportFileValidator(ExportFile exportSettings, IExportRequestRepository exportRequestRepository, IStatus status, IFileHelper fileHelper, ILog logger)
		{
			_exportSettings = exportSettings;
			_exportRequestRepository = exportRequestRepository;
			_status = status;
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public bool CanExport(string destinationLocation, string warningUserMessage)
		{
			if (_fileHelper.Exists(destinationLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_logger.LogVerbose($"Overwriting document {destinationLocation}. Removing already existing file.");
					_fileHelper.Delete(destinationLocation);
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

			if (_exportRequestRepository.GetExportRequests().Any(x => x.DestinationLocation == destinationLocation))
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