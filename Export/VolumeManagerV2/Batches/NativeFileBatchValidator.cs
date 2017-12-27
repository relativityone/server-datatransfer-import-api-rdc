using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class NativeFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public NativeFileBatchValidator(IErrorFileWriter errorFileWriter, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			for (int i = 0; i < artifacts.Length; i++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
				ValidateNativesForArtifact(artifacts[i], predictions[i]);
			}
		}

		private void ValidateNativesForArtifact(ObjectExportInfo artifact, VolumePredictions prediction)
		{
			if (string.IsNullOrWhiteSpace(artifact.NativeTempLocation))
			{
				return;
			}
			if (!_fileHelper.Exists(artifact.NativeTempLocation) || _fileHelper.GetFileSize(artifact.NativeTempLocation) == 0)
			{
				_logger.LogError("Native file {file} missing or empty for artifact {artifactId}.", artifact.NativeTempLocation, artifact.ArtifactID);
				string errorMessage = _fileHelper.Exists(artifact.NativeTempLocation) ? "Empty file." : "File missing.";
				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Native, artifact.IdentifierValue, artifact.NativeTempLocation, errorMessage);
			}
			else if (_fileHelper.GetFileSize(artifact.NativeTempLocation) != prediction.NativeFilesSize)
			{
				long actualFileSize = _fileHelper.GetFileSize(artifact.NativeTempLocation);
				_logger.LogWarning("Native file {file} size {actualSize} is different from expected {expectedSize} for artifact {artifactId}.", artifact.NativeTempLocation, actualFileSize,
					prediction.NativeFilesSize, artifact.ArtifactID);
				_status.WriteWarning(
					$"Native file {artifact.NativeTempLocation} size {actualFileSize} is different from expected {prediction.NativeFilesSize} for artifact {artifact.ArtifactID}.");
			}
		}
	}
}