namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;

	using Castle.Core;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Paths;
	using Relativity.Logging;

	public class LoadFileBatchValidator : IBatchValidator
	{
		private readonly IDestinationPath _destinationPath;

		private readonly IFile _fileWrapper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public LoadFileBatchValidator(LoadFileDestinationPath destinationPath, IFile fileWrapper, IStatus status, ILog logger) : this((IDestinationPath) destinationPath, fileWrapper,
			status, logger)
		{
		}

		/// <summary>
		///     For testing
		/// </summary>
		/// <param name="destinationPath"></param>
		/// <param name="fileWrapper"></param>
		/// <param name="status"></param>
		/// <param name="logger"></param>
		[DoNotSelect]
		public LoadFileBatchValidator(IDestinationPath destinationPath, IFile fileWrapper, IStatus status, ILog logger)
		{
			_destinationPath = destinationPath;
			_fileWrapper = fileWrapper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			if (!_fileWrapper.Exists(_destinationPath.Path))
			{
				_logger.LogError("Load file {file} is missing.", _destinationPath.Path);
				_status.WriteError($"Load file {_destinationPath.Path} is missing.");
			}
			else if (_fileWrapper.GetFileSize(_destinationPath.Path) == 0)
			{
				_logger.LogError("Load file {file} is empty.", _destinationPath.Path);
				_status.WriteError($"Load file {_destinationPath.Path} is empty.");
			}
		}
	}
}