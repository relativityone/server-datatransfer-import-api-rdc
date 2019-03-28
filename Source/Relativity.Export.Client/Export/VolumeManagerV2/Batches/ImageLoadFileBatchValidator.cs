using System.Linq;
using System.Threading;
using Castle.Core;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class ImageLoadFileBatchValidator : IBatchValidator
	{
		private readonly IDestinationPath _destinationPath;

		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public ImageLoadFileBatchValidator(ImageLoadFileDestinationPath destinationPath, IFileHelper fileHelper, IStatus status, ILog logger) : this((IDestinationPath) destinationPath,
			fileHelper, status, logger)
		{
		}

		/// <summary>
		///     Used for testing
		/// </summary>
		/// <param name="destinationPath"></param>
		/// <param name="fileHelper"></param>
		/// <param name="status"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public ImageLoadFileBatchValidator(IDestinationPath destinationPath, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_destinationPath = destinationPath;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			if (!_fileHelper.Exists(_destinationPath.Path))
			{
				_logger.LogError("Image load file {file} is missing.", _destinationPath.Path);
				_status.WriteError($"Image load file {_destinationPath.Path} is missing.");
			}
			else if (_fileHelper.GetFileSize(_destinationPath.Path) == 0)
			{
				if (artifacts.Any(x => x.Images != null && x.Images.Count > 0))
				{
					_logger.LogError("Image load file {file} is empty.", _destinationPath.Path);
					_status.WriteError($"Image load file {_destinationPath.Path} is empty.");
				}
			}
		}
	}
}