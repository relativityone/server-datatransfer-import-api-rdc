using System.IO;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	/// <summary>
	/// TODO probably can extract some code from here and from NativeExportRequestBuilder
	/// </summary>
	public class ImageExportRequestBuilder
	{
		private readonly ExportFile _exportSettings;
		private readonly LabelManager _labelManager;
		private readonly IFileHelper _fileHelper;
		private readonly IDirectoryHelper _directoryHelper;
		private readonly IStatus _status;

		public ImageExportRequestBuilder(ExportFile exportSettings, LabelManager labelManager, IFileHelper fileHelper, IDirectoryHelper directoryHelper, IStatus status)
		{
			_exportSettings = exportSettings;
			_labelManager = labelManager;
			_fileHelper = fileHelper;
			_directoryHelper = directoryHelper;
			_status = status;
		}

		public ImageExportRequestBuilder(ExportFile exportSettings)
		{
			_exportSettings = exportSettings;
		}

		public ExportRequest Create(ImageExportInfo image)
		{
			if (!ExportingImagesFiles())
			{
				//TODO OMG!
				return null;
			}

			string destinationLocation = GetExportDestinationLocation(image);

			if (!CanExport(image, destinationLocation))
			{
				//TODO OMG!
				return null;
			}

			image.TempLocation = destinationLocation;

			return new ExportRequest(image, destinationLocation);
		}

		private bool ExportingImagesFiles()
		{
			return _exportSettings.ExportImages && _exportSettings.VolumeInfo.CopyImageFilesFromRepository;
		}

		private string GetExportDestinationLocation(ImageExportInfo image)
		{
			string volumeLabel = _labelManager.GetCurrentVolumeLabel();
			string subdirectoryLabel = _labelManager.GetCurrentImageSubdirectoryLabel();

			string destinationDirectory = Path.Combine(_exportSettings.FolderPath, volumeLabel, subdirectoryLabel);

			if (!_directoryHelper.Exists(destinationDirectory))
			{
				_directoryHelper.CreateDirectory(destinationDirectory);
			}
			return Path.Combine(destinationDirectory, image.FileName);
		}

		private bool CanExport(ImageExportInfo image, string destinationLocation)
		{
			if (_fileHelper.Exists(destinationLocation))
			{
				if (_exportSettings.Overwrite)
				{
					_fileHelper.Delete(destinationLocation);
					_status.WriteStatusLine(EventType.Status, $"Overwriting image for {image.BatesNumber}.", false);
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