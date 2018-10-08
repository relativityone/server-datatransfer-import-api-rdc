using System.Collections.Generic;
using System.Linq;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Batches
{
	public class LongTextBatchValidator : IBatchValidator
	{
		private readonly ILongTextRepository _longTextRepository;

		private readonly IFileHelper _fileHelper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public LongTextBatchValidator(ILongTextRepository longTextRepository, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_longTextRepository = longTextRepository;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, VolumePredictions[] predictions, CancellationToken cancellationToken)
		{
			IEnumerable<LongText> downloadedFiles = _longTextRepository.GetLongTexts().Where(x => x.ExportRequest != null && !x.RequireDeletion);
			foreach (LongText longText in downloadedFiles)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				if (!_fileHelper.Exists(longText.Location))
				{
					_logger.LogError("File {file} for LongText {fieldId} for artifact {artifactId} missing.", longText.Location, longText.FieldArtifactId, longText.ArtifactId);
					_status.WriteError($"File {longText.Location} for LongText {longText.FieldArtifactId} for artifact {longText.ArtifactId} missing.");
				}
				else if (_fileHelper.GetFileSize(longText.Location) == 0)
				{
					_logger.LogWarning("File {file} for LongText {fieldId} for artifact {artifactId} is empty.", longText.Location, longText.FieldArtifactId, longText.ArtifactId);
					_status.WriteWarning($"File {longText.Location} for LongText {longText.FieldArtifactId} for artifact {longText.ArtifactId} is empty.");
				}
			}
		}
	}
}