using System;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class NativeFileParallelBatchValidator : IBatchValidator
	{
		private readonly IErrorFileWriter _errorFileWriter;
		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public NativeFileParallelBatchValidator(IErrorFileWriter errorFileWriter, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_errorFileWriter = errorFileWriter;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			ParallelQuery<Action> validationResults =
				Enumerable.Range(0, artifacts.Length).AsParallel().AsOrdered()
					.Select(i => GetErrorMessageForArtifact(artifacts[i], predictions[i], cancellationToken));

			foreach (var validationAction in validationResults.Where(result => result != null))
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				validationAction();
			}
		}

		private Action GetErrorMessageForArtifact(ObjectExportInfo artifact, VolumePredictions prediction, CancellationToken cancellationToken)
		{
			if (string.IsNullOrWhiteSpace(artifact.NativeTempLocation) || cancellationToken.IsCancellationRequested)
			{
				return null;
			}

			bool nativeFileExists = _fileHelper.Exists(artifact.NativeTempLocation);

			if (!nativeFileExists || _fileHelper.GetFileSize(artifact.NativeTempLocation) == 0)
			{
				return () =>
				{
					_logger.LogError("Native file {file} missing or empty for artifact {artifactId}.", artifact.NativeTempLocation, artifact.ArtifactID);
					string errorMessage = nativeFileExists ? "Empty file." : "File missing.";
					_errorFileWriter.Write(ErrorFileWriter.ExportFileType.Native, artifact.IdentifierValue, artifact.NativeTempLocation, errorMessage);
				};
			}

			return null;
		}
	}
}