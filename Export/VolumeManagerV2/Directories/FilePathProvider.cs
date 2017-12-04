using System.IO;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public abstract class FilePathProvider : IFilePathProvider
	{
		private readonly ExportFile _exportSettings;
		private readonly IDirectoryHelper _directoryHelper;
		private readonly ILog _logger;

		protected LabelManager LabelManager { get; }

		protected FilePathProvider(LabelManager labelManager, ExportFile exportSettings, IDirectoryHelper directoryHelper, ILog logger)
		{
			LabelManager = labelManager;
			_exportSettings = exportSettings;
			_directoryHelper = directoryHelper;
			_logger = logger;
		}

		public string GetPathForFile(string fileName)
		{
			string volumeLabel = LabelManager.GetCurrentVolumeLabel();
			string subdirectoryLabel = GetSubdirectoryLabel();

			string destinationDirectory = Path.Combine(_exportSettings.FolderPath, volumeLabel, subdirectoryLabel);

			if (!_directoryHelper.Exists(destinationDirectory))
			{
				_logger.LogInformation("Creating directory {directory}.", destinationDirectory);
				_directoryHelper.CreateDirectory(destinationDirectory);
			}

			return Path.Combine(destinationDirectory, fileName);
		}

		protected abstract string GetSubdirectoryLabel();
	}
}