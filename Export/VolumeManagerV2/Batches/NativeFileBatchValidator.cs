using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class NativeFileBatchValidator : IBatchValidator
	{
		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public NativeFileBatchValidator(IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions)
		{
			for (int i = 0; i < artifacts.Length; i++)
			{
				ValidateNativesForArtifact(artifacts[i], predictions[i]);
			}
		}

		private void ValidateNativesForArtifact(ObjectExportInfo artifact, VolumePredictions prediction)
		{
			if (!_fileHelper.Exists(artifact.NativeTempLocation))
			{
				_logger.LogWarning("Native file {file} missing for artifact {artifactId}.", artifact.NativeTempLocation, artifact.ArtifactID);
				_status.WriteWarning($"Native file {artifact.NativeTempLocation} missing for artifact {artifact.ArtifactID}.");
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