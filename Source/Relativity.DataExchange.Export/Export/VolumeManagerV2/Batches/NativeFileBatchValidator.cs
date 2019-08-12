namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Resources;
	using Relativity.Logging;

	public class NativeFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public NativeFileBatchValidator(
			IErrorFileWriter errorFileWriter,
			IFile fileWrapper,
			IStatus status,
			ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileWrapper = fileWrapper;
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

			bool fileExists = _fileWrapper.Exists(artifact.NativeTempLocation);
			if (!fileExists || _fileWrapper.GetFileSize(artifact.NativeTempLocation) == 0)
			{
				string errorMessage = fileExists ? ExportStrings.FileValidationZeroByteFile : ExportStrings.FileValidationFileMissing;
				if (string.IsNullOrWhiteSpace(artifact.NativeSourceLocation))
				{
					errorMessage = ExportStrings.FileValidationEmptyRemoteSourcePath;
					_logger.LogError(
						"Native file {File} remote source path is empty for native artifact {ArtifactId} and suggests a back-end database issue.",
						artifact.NativeTempLocation,
						artifact.ArtifactID);
				}
				else
				{
					_logger.LogError(
						"Native file {File} missing or empty for artifact {ArtifactId}.",
						artifact.NativeTempLocation,
						artifact.ArtifactID);
				}

				_errorFileWriter.Write(
					ErrorFileWriter.ExportFileType.Native,
					artifact.IdentifierValue,
					artifact.NativeTempLocation,
					errorMessage);
			}
		}
	}
}