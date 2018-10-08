using System;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class LongTextParallelBatchValidator : IBatchValidator
	{
		private readonly ILongTextRepository _longTextRepository;

		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public LongTextParallelBatchValidator(ILongTextRepository longTextRepository, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_longTextRepository = longTextRepository;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			ParallelQuery<Action> validationResults =
				_longTextRepository.GetLongTexts().AsParallel().AsOrdered()
					.Where(x => (x.ExportRequest != null || x.HasBeenDownloaded) && !x.RequireDeletion)
					.Select(x => GetErrorActionForArtifact(x, cancellationToken));

			foreach (Action validationAction in validationResults.Where(action => action != null))
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				validationAction();
			}
		}

		public Action GetErrorActionForArtifact(LongText longText, CancellationToken cancellationToken)
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return null;
			}

			if (!_fileHelper.Exists(longText.Location))
			{
				return () =>
				{
					_logger.LogError("File {file} for LongText {fieldId} for artifact {artifactId} missing.", longText.Location, longText.FieldArtifactId, longText.ArtifactId);
					_status.WriteError($"File {longText.Location} for LongText {longText.FieldArtifactId} for artifact {longText.ArtifactId} missing.");
				};
			}
			else if (_fileHelper.GetFileSize(longText.Location) == 0)
			{
				return () =>
				{
					_logger.LogWarning("File {file} for LongText {fieldId} for artifact {artifactId} is empty.", longText.Location, longText.FieldArtifactId, longText.ArtifactId);
					_status.WriteWarning($"File {longText.Location} for LongText {longText.FieldArtifactId} for artifact {longText.ArtifactId} is empty.");
				};
			}

			return null;
		}
	}
}