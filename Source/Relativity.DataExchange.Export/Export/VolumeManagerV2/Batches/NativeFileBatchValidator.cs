namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Threading;
	using kCura.WinEDDS.Exporters;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.Logging;

	public class NativeFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		public NativeFileBatchValidator(IErrorFileWriter errorFileWriter, IFile fileWrapper, ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileWrapper = fileWrapper;
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
   
			 	ValidateNativesForArtifact(artifacts[i]);
			 }
		}

		private void ValidateNativesForArtifact(ObjectExportInfo artifact)
		{
			if (string.IsNullOrWhiteSpace(artifact.NativeTempLocation))
			{
				return;
			}

			bool nativeFileExists = _fileWrapper.Exists(artifact.NativeTempLocation);

			if (!nativeFileExists || _fileWrapper.GetFileSize(artifact.NativeTempLocation) == 0)
			{
				_logger.LogError("Native file {file} missing or empty for artifact {artifactId}.", artifact.NativeTempLocation, artifact.ArtifactID);
				string errorMessage = nativeFileExists ? "Empty file." : "File missing.";
				_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Native, artifact.IdentifierValue, artifact.NativeTempLocation, errorMessage);
			}
		}
	}
}