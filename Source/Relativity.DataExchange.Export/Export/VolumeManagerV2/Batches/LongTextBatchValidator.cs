namespace Relativity.DataExchange.Export.VolumeManagerV2.Batches
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	public class LongTextBatchValidator : IBatchValidator
	{
		private readonly ILongTextRepository _longTextRepository;

		private readonly IFile _fileWrapper;
		private readonly IStatus _status;
		private readonly ILog _logger;

		public LongTextBatchValidator(ILongTextRepository longTextRepository, IFile fileWrapper, IStatus status, ILog logger)
		{
			_longTextRepository = longTextRepository;
			_fileWrapper = fileWrapper;
			_status = status;
			_logger = logger;
		}

		public void ValidateExportedBatch(ObjectExportInfo[] artifacts, CancellationToken cancellationToken)
		{
			IEnumerable<LongText> downloadedFiles = _longTextRepository.GetLongTexts().Where(x =>
				x.ExportRequest != null && !x.RequireDeletion && !string.IsNullOrWhiteSpace(x.Location));
			foreach (LongText longText in downloadedFiles)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				if (!_fileWrapper.Exists(longText.Location))
				{
					_logger.LogError("File {file} for LongText {fieldId} for artifact {artifactId} missing.", longText.Location.Secure(), longText.FieldArtifactId, longText.ArtifactId);
					_status.WriteError($"File {longText.Location} for LongText {longText.FieldArtifactId} for artifact {longText.ArtifactId} missing.");
				}
				else if (_fileWrapper.GetFileSize(longText.Location) == 0)
				{
					_logger.LogWarning("File {file} for LongText {fieldId} for artifact {artifactId} is empty.", longText.Location.Secure(), longText.FieldArtifactId, longText.ArtifactId);
					_status.WriteWarning($"File {longText.Location} for LongText {longText.FieldArtifactId} for artifact {longText.ArtifactId} is empty.");
				}
			}
		}
	}
}