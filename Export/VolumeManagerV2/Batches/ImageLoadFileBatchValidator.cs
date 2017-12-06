using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class ImageLoadFileBatchValidator : IBatchValidator
	{
		private readonly ImageLoadFileDestinationPath _destinationPath;

		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImageLoadFileBatchValidator(ImageLoadFileDestinationPath destinationPath, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_destinationPath = destinationPath;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions)
		{
			if (!_fileHelper.Exists(_destinationPath.Path))
			{
				_logger.LogWarning("Image load file {file} is missing.", _destinationPath.Path);
				_status.WriteWarning($"Image load file {_destinationPath.Path} is missing.");
			}
			else if (_fileHelper.GetFileSize(_destinationPath.Path) == 0)
			{
				_logger.LogWarning("Image load file {file} is empty.", _destinationPath.Path);
				_status.WriteWarning($"Image load file {_destinationPath.Path} is empty.");
			}
		}
	}
}