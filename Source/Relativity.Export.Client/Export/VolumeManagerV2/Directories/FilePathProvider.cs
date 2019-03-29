using System.IO;
using Relativity.Import.Export.Io;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories
{
	public abstract class FilePathProvider : IFilePathProvider
	{
		private readonly ExportFile _exportSettings;
		private readonly IDirectory _directoryHelper;
		private readonly ILog _logger;

		protected ILabelManagerForArtifact LabelManagerForArtifact { get; }

		protected FilePathProvider(ILabelManagerForArtifact labelManagerForArtifact, ExportFile exportSettings, IDirectory directoryHelper, ILog logger)
		{
			LabelManagerForArtifact = labelManagerForArtifact;
			_exportSettings = exportSettings;
			_directoryHelper = directoryHelper;
			_logger = logger;
		}

		public string GetPathForFile(string fileName, int objectExportInfoArtifactId)
		{
			string volumeLabel = LabelManagerForArtifact.GetVolumeLabel(objectExportInfoArtifactId);
			string subdirectoryLabel = GetSubdirectoryLabel(objectExportInfoArtifactId);

			string destinationDirectory = Path.Combine(_exportSettings.FolderPath, volumeLabel, subdirectoryLabel);

			if (!_directoryHelper.Exists(destinationDirectory))
			{
				_logger.LogInformation("Creating directory {directory}.", destinationDirectory);
				_directoryHelper.CreateDirectory(destinationDirectory);
			}

			return Path.Combine(destinationDirectory, fileName);
		}

		protected abstract string GetSubdirectoryLabel(int objectExportInfoArtifactId);
	}
}