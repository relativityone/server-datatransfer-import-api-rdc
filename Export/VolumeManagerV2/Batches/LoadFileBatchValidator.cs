using System.Threading;
using Castle.Core;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Paths;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class LoadFileBatchValidator : IBatchValidator
	{
		private readonly IDestinationPath _destinationPath;

		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public LoadFileBatchValidator(LoadFileDestinationPath destinationPath, IFileHelper fileHelper, IStatus status, ILog logger) : this((IDestinationPath) destinationPath, fileHelper,
			status, logger)
		{
		}

		/// <summary>
		/// For testing
		/// </summary>
		/// <param name="destinationPath"></param>
		/// <param name="fileHelper"></param>
		/// <param name="status"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public LoadFileBatchValidator(IDestinationPath destinationPath, IFileHelper fileHelper, IStatus status, ILog logger)
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
				_logger.LogError("Load file {file} is missing.", _destinationPath.Path);
				_status.WriteError($"Load file {_destinationPath.Path} is missing.");
			}
			else if (_fileHelper.GetFileSize(_destinationPath.Path) == 0)
			{
				_logger.LogError("Load file {file} is empty.", _destinationPath.Path);
				_status.WriteError($"Load file {_destinationPath.Path} is empty.");
			}
		}
	}
}