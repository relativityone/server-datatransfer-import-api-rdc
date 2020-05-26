namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System.Collections.Generic;
	using System.Linq;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	public class DownloadProgressManager : IDownloadProgress, IDownloadProgressManager
	{
		private readonly HashSet<int> _artifactsProcessed;
		private readonly FileRequestRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly FileRequestRepository _pdfRepository;
		private readonly IStatus _status;
		private readonly ILog _logger;
		private readonly object _syncObject = new object();

		public DownloadProgressManager(
			FileRequestRepository nativeRepository,
			ImageRepository imageRepository,
			LongTextRepository longTextRepository,
			FileRequestRepository pdfRepository,
			IStatus status,
			ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_pdfRepository = pdfRepository;
			_status = status;
			_logger = logger;
			_artifactsProcessed = new HashSet<int>();
		}

		public void FinalizeBatchProcessedCount()
		{
			lock (_syncObject)
			{
				_logger.LogVerbose("Finalizing the batch processed artifact count...");
				int finalizedArtifactCount = 0;
				foreach (FileRequest<ObjectExportInfo> native in _nativeRepository.GetFileRequests())
				{
					const bool Finalizing = true;
					int artifactId = native.Artifact.ArtifactID;
					if (this.UpdateProcessedCount(artifactId, Finalizing))
					{
						_logger.LogWarning(
							"The processed document count was incremented for artifact {ArtifactId} during finalization.",
							artifactId);
						finalizedArtifactCount++;
					}
				}

				_logger.LogVerbose(
					"Finalized the batch processed artifact count and incremented by {FinalizedArtifactCount}.",
					finalizedArtifactCount);
			}
		}

		public void MarkArtifactAsError(int artifactId, string message)
		{
			lock (_syncObject)
			{
				if (!_artifactsProcessed.Contains(artifactId))
				{
					_logger.LogVerbose(
						"Marking artifact {ArtifactId} as error. Message: {ErrorMessage}",
						artifactId,
						message);
					_artifactsProcessed.Add(artifactId);
				}

				this.PublishProcessedCount();
			}
		}

		public void MarkFileAsCompleted(string targetFile, int lineNumber, bool transferResult)
		{
			lock (_syncObject)
			{
				_logger.LogVerbose("Marking {TargetFile} {LineNumber} file as completed.", targetFile.Secure(), lineNumber);
				IList<FileRequest<ObjectExportInfo>> natives = _nativeRepository.GetFileRequestByDestinationLocation(targetFile);
				IList<FileRequest<ObjectExportInfo>> pdfs = this._pdfRepository.GetFileRequestByDestinationLocation(targetFile);
				if (natives.Count > 0)
				{
					this.MarkAsCompleted(natives, transferResult);
				}
				else if (pdfs.Count > 0)
				{
					this.MarkAsCompleted(pdfs, transferResult);
				}
				else
				{
					IList<FileRequest<ImageExportInfo>> images = _imageRepository.GetImagesByTargetFile(targetFile);
					if (images.Count > 0)
					{
						FileRequest<ImageExportInfo> image = images.FirstOrDefault(x => !x.TransferCompleted);
						if (image != null)
						{
							image.TransferCompleted = true;
							this.UpdateProcessedCountAndNotify(image.Artifact.ArtifactID, transferResult);
						}
					}
					else
					{
						_logger.LogWarning(
							"The process count isn't incremented for {TargetFile} because the native or image file doesn't exist in any repository.",
							targetFile.Secure());
					}
				}
			}
		}

		private void MarkAsCompleted(IList<FileRequest<ObjectExportInfo>> files, bool transferResult)
		{
			FileRequest<ObjectExportInfo> file = files.FirstOrDefault(x => !x.TransferCompleted);
			if (file != null)
			{
				file.TransferCompleted = true;
				this.UpdateProcessedCountAndNotify(file.Artifact.ArtifactID, transferResult);
			}
		}

		public void MarkLongTextAsCompleted(string targetFile, int lineNumber, bool transferResult)
		{
			lock (_syncObject)
			{
				_logger.LogVerbose("Marking {TargetFile} long text as completed.", targetFile.Secure());
				LongText longText = _longTextRepository.GetByLineNumber(lineNumber);
				if (longText != null)
				{
					longText.TransferCompleted = true;
					this.UpdateProcessedCountAndNotify(longText.ArtifactId, transferResult);
				}
				else
				{
					_logger.LogWarning(
						"The process count isn't incremented for {TargetFile} because the long text file doesn't exist in the long text repository.",
						targetFile.Secure());
				}
			}
		}
		
		private void UpdateProcessedCountAndNotify(int artifactId, bool transferResult)
		{
			_logger.LogVerbose(
				"Updating processed document count after artifact {ArtifactId} transfer has been completed with transfer result {TransferResult}.",
				artifactId,
				transferResult);
			const bool Finalizing = false;
			bool updated = this.UpdateProcessedCount(artifactId, Finalizing);
			if (!updated)
			{
				return;
			}

			FileRequest<ObjectExportInfo> native = _nativeRepository.GetFileRequest(artifactId);
			if (native == null)
			{
				return;
			}

			_logger.LogVerbose(
				"Document {identifierValue} export completed with transfer result {TransferResult}.",
				native.Artifact.IdentifierValue,
				transferResult);
			string suffixMessage = $" (artifact: {artifactId})";
			_status.WriteStatusLine(
				EventType2.Progress,
				$"Document {native.Artifact.IdentifierValue} export completed {suffixMessage}.",
				false);
		}

		private bool UpdateProcessedCount(int artifactId, bool finalizing)
		{
			if ((!finalizing && !this.IsDocumentProcessed(artifactId)) || _artifactsProcessed.Contains(artifactId))
			{
				return false;
			}

			_artifactsProcessed.Add(artifactId);
			this.PublishProcessedCount();
			return true;
		}

		private bool IsDocumentProcessed(int artifactId)
		{
			FileRequest<ObjectExportInfo> native = _nativeRepository.GetFileRequest(artifactId);
			if (native == null)
			{
				return false;
			}

			artifactId = native.Artifact.ArtifactID;
			if (!native.TransferCompleted)
			{
				return false;
			}

			IList<FileRequest<ImageExportInfo>> images = _imageRepository.GetArtifactImages(artifactId);
			if (images.Any(x => !x.TransferCompleted))
			{
				return false;
			}

			IEnumerable<LongText> longTexts = _longTextRepository.GetArtifactLongTexts(artifactId);
			if (longTexts.Any(x => !x.TransferCompleted))
			{
				return false;
			}

			FileRequest<ObjectExportInfo> pdf = this._pdfRepository.GetFileRequest(artifactId);
			if (pdf != null && !pdf.TransferCompleted)
			{
				return false;
			}

			return true;
		}

		private void PublishProcessedCount()
		{
			_status.UpdateDocumentExportedCount(_artifactsProcessed.Count);
		}
	}
}