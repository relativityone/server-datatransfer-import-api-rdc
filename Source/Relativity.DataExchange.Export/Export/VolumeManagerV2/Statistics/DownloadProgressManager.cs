namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System.Collections.Generic;
	using System.Linq;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	public class DownloadProgressManager : IDownloadProgress, IDownloadProgressManager
	{
		private readonly HashSet<int> _artifactsProcessed;
		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly IStatus _status;
		private readonly ILog _logger;
		private readonly object _syncObject = new object();

		public DownloadProgressManager(
			NativeRepository nativeRepository,
			ImageRepository imageRepository,
			LongTextRepository longTextRepository,
			IStatus status,
			ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
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
				foreach (Native native in _nativeRepository.GetNatives())
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
				IList<Native> natives = _nativeRepository.GetNativesByTargetFile(targetFile);
				if (natives.Count > 0)
				{
					Native native = natives.FirstOrDefault(x => !x.TransferCompleted);
					if (native != null)
					{
						native.TransferCompleted = true;
						this.MarkNativeAsCompleted(native, transferResult);
					}
				}
				else
				{
					this.MarkImageAsCompleted(targetFile, transferResult);
				}
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

		private void MarkImageAsCompleted(string targetFile, bool transferResult)
		{
			IList<Image> images = _imageRepository.GetImagesByTargetFile(targetFile);
			if (images.Count > 0)
			{
				Image image = images.FirstOrDefault(x => !x.TransferCompleted);
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

		private void MarkNativeAsCompleted(Native native, bool transferResult)
		{
			native.TransferCompleted = true;
			this.UpdateProcessedCountAndNotify(native.Artifact.ArtifactID, transferResult);
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

			Native native = _nativeRepository.GetNative(artifactId);
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
			Native native = _nativeRepository.GetNative(artifactId);
			if (native == null)
			{
				return false;
			}

			artifactId = native.Artifact.ArtifactID;
			if (!native.TransferCompleted)
			{
				return false;
			}

			IList<Image> images = _imageRepository.GetArtifactImages(artifactId);
			if (images.Any(x => !x.TransferCompleted))
			{
				return false;
			}

			IEnumerable<LongText> longTexts = _longTextRepository.GetArtifactLongTexts(artifactId);
			if (longTexts.Any(x => !x.TransferCompleted))
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