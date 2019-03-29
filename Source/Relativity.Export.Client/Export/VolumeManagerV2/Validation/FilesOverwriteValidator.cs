namespace Relativity.Export.VolumeManagerV2.Validation
{
	using kCura.WinEDDS.Exporters;

	using Relativity.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.Import.Export.Io;
	using Relativity.Logging;

	public class FilesOverwriteValidator
	{
		private readonly IUserNotification _interactionManager;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		public FilesOverwriteValidator(IUserNotification interactionManager, IFile fileWrapper, ILog logger)
		{
			_interactionManager = interactionManager;
			_fileWrapper = fileWrapper;
			_logger = logger;
		}

		public bool ValidateLoadFilesOverwriting(bool overwrite, bool exportImages, LoadFileDestinationPath loadFileDestinationPath,
			ImageLoadFileDestinationPath imageLoadFileDestinationPath)
		{
			return ValidateLoadFileOverwriting(overwrite, loadFileDestinationPath) && ValidateImageLoadFileOverwriting(overwrite, exportImages, imageLoadFileDestinationPath);
		}

		private bool ValidateLoadFileOverwriting(bool overwrite, LoadFileDestinationPath loadFileDestinationPath)
		{
			if (!overwrite && _fileWrapper.Exists(loadFileDestinationPath.Path))
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
			if (!overwrite && exportImages && _fileWrapper.Exists(imageLoadFileDestinationPath.Path))
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