namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Globalization;
	using System.Threading;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.DataExchange.Resources;
	using Relativity.Logging;

	public class NativeFileBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFile _fileWrapper;
		private readonly ILog _logger;

		public NativeFileBatchValidator(
			IErrorFileWriter errorFileWriter,
			IFile fileWrapper,
			ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileWrapper = fileWrapper;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
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

			bool fileExists = _fileWrapper.Exists(artifact.NativeTempLocation);
			if (!fileExists || _fileWrapper.GetFileSize(artifact.NativeTempLocation) == 0)
			{
				string errorMessage = string.Format(
					CultureInfo.CurrentCulture,
					fileExists ? ExportStrings.FileValidationZeroByteFile : ExportStrings.FileValidationFileMissing,
					artifact.ArtifactID);
				if (string.IsNullOrWhiteSpace(artifact.NativeSourceLocation))
				{
					errorMessage = string.Format(
						CultureInfo.CurrentCulture,
						ExportStrings.FileValidationEmptyRemoteSourcePath,
						artifact.ArtifactID);
					_logger.LogError(
						"Native file {File} remote source path is null or whitespace for native artifact {ArtifactId} and indicates a problem with the artifact data.",
						artifact.NativeTempLocation,
						artifact.ArtifactID);
				}
				else
				{
					this._logger.LogError(
						fileExists
							? "Native file {File} contains zero bytes for artifact {ArtifactId}."
							: "Native file {File} is missing for artifact {ArtifactId}.",
						artifact.NativeTempLocation,
						artifact.ArtifactID);
				}

				_errorFileWriter.Write(
					ErrorFileWriter.ExportFileType.Native,
					artifact,
					artifact.NativeTempLocation,
					errorMessage);
			}
		}
	}
}