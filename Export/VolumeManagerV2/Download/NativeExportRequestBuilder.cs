using System.IO;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class NativeExportRequestBuilder
	{
		private readonly ExportFile _exportSettings;
		private readonly LabelManager _labelManager;
		private readonly IFileHelper _fileHelper;
		private readonly IDirectoryHelper _directoryHelper;
		private readonly IFileNameProvider _fileNameProvider;
		private readonly IStatus _status;

		public NativeExportRequestBuilder(ExportFile exportSettings, LabelManager labelManager, IFileHelper fileHelper, IDirectoryHelper directoryHelper,
			IFileNameProvider fileNameProvider, IStatus status)
		{
			_exportSettings = exportSettings;
			_labelManager = labelManager;
			_fileHelper = fileHelper;
			_directoryHelper = directoryHelper;
			_fileNameProvider = fileNameProvider;
			_status = status;
		}

		public ExportRequest Create(ObjectExportInfo artifact)
		{
			if (!ExportingNativeFiles())
			{
				//TODO
				return null;
			}

			string destinationLocation = GetExportDestinationLocation(artifact);

			if (!CanExport(destinationLocation))
			{
				//TODO
				return null;
			}

			artifact.NativeTempLocation = destinationLocation;

			return new ExportRequest(artifact, destinationLocation);
		}

		private bool ExportingNativeFiles()
		{
			return _exportSettings.ExportNative && _exportSettings.VolumeInfo.CopyNativeFilesFromRepository;
		}

		private string GetExportDestinationLocation(ObjectExportInfo artifact)
		{
			string fileName = _fileNameProvider.GetName(artifact);

			string volumeLabel = _labelManager.GetCurrentVolumeLabel();
			string subdirectoryLabel = _labelManager.GetCurrentNativeSubdirectoryLabel();

			string destinationDirectory = Path.Combine(_exportSettings.FolderPath, volumeLabel, subdirectoryLabel);

			if (!_directoryHelper.Exists(destinationDirectory))
			{
				_directoryHelper.CreateDirectory(destinationDirectory);
			}

			return Path.Combine(destinationDirectory, fileName);
		}

		private bool CanExport(string destinationLocation)
		{
			if (_fileHelper.Exists(destinationLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_fileHelper.Delete(destinationLocation);
					_status.WriteStatusLine(EventType.Status, $"Overwriting document {destinationLocation}.", false);
				}
				else
				{
					_status.WriteWarning($"{destinationLocation} already exists. Skipping file export.");
					return false;
				}
			}
			return true;
		}
	}
}