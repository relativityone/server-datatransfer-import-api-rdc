using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Validation
{
	public class FilesOverwriteValidator
	{
		private readonly IUserNotification _interactionManager;
		private readonly IFileHelper _fileHelper;
		private readonly ILog _logger;

		public FilesOverwriteValidator(IUserNotification interactionManager, IFileHelper fileHelper, ILog logger)
		{
			_interactionManager = interactionManager;
			_fileHelper = fileHelper;
			_logger = logger;
		}

		public bool ValidateLoadFilesOverwriting(bool overwrite, bool exportImages, LoadFileDestinationPath loadFileDestinationPath,
			ImageLoadFileDestinationPath imageLoadFileDestinationPath)
		{
			return ValidateLoadFileOverwriting(overwrite, loadFileDestinationPath) && ValidateImageLoadFileOverwriting(overwrite, exportImages, imageLoadFileDestinationPath);
		}

		private bool ValidateLoadFileOverwriting(bool overwrite, LoadFileDestinationPath loadFileDestinationPath)
		{
			if (!overwrite && _fileHelper.Exists(loadFileDestinationPath.Path))
			{
				_interactionManager.Alert(CreateErrorMessage(loadFileDestinationPath.Path));
				_logger.LogError("Load file exists. Canceling the export.");
				return false;
			}

			_logger.LogVerbose("Load file doesn't exist or overwrite selected.");
			return true;
		}

		private bool ValidateImageLoadFileOverwriting(bool overwrite, bool exportImages, ImageLoadFileDestinationPath imageLoadFileDestinationPath)
		{
			if (!overwrite && exportImages && _fileHelper.Exists(imageLoadFileDestinationPath.Path))
			{
				_interactionManager.Alert(CreateErrorMessage(imageLoadFileDestinationPath.Path));
				_logger.LogError("Image load file exists. Canceling the export.");
				return false;
			}

			_logger.LogVerbose("Image load file doesn't exist, overwrite selected or exporting images skipped.");
			return true;
		}

		private string CreateErrorMessage(string filePath)
		{
			return $"Overwrite not selected and file '{filePath}' exists.";
		}
	}
}