namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;

	using kCura.WinEDDS;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Process;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.Logging;

	public class DownloadProgressManager : IDownloadProgress, IDownloadProgressManager
	{
		private int _savedDocumentsDownloadedCount;

		private readonly HashSet<int> _artifactsCompleted;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly IFile _fileWrapper;

		private readonly IStatus _status;
		private readonly ILog _logger;

		private readonly object _syncObject = new object();

		public DownloadProgressManager(
			NativeRepository nativeRepository,
			ImageRepository imageRepository,
			LongTextRepository longTextRepository,
			IFile fileWrapper,
			IStatus status,
			ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_fileWrapper = fileWrapper;
			_status = status;
			_logger = logger;

			_artifactsCompleted = new HashSet<int>();
		}

		public void MarkArtifactAsError(int artifactId, string message)
		{
			if (!_artifactsCompleted.Contains(artifactId))
			{
				_logger.LogVerbose(
					"Marking artifact {ArtifactId} as error. Message: {ErrorMessage}",
					artifactId,
					message);
				_artifactsCompleted.Add(artifactId);
			}

			this.PublishProcessedCount();
		}

		public void MarkFileAsCompleted(string fileName, int lineNumber)
		{
			_logger.LogVerbose("Marking {fileName} file as completed.", fileName);
			Native native = _nativeRepository.GetByLineNumber(lineNumber);
			if (native != null)
			{
				this.MarkNativeAsCompleted(lineNumber, native);
			}
			else
			{
				this.MarkImageAsCompleted(fileName, lineNumber);
			}
		}

		public void MarkLongTextAsCompleted(string fileName, int lineNumber)
		{
			_logger.LogVerbose("Marking {fileName} long text as completed.", fileName);
			LongText longText = _longTextRepository.GetByLineNumber(lineNumber);
			if (longText != null)
			{
				longText.HasBeenDownloaded = true;
				this.UpdateCompletedCountAndNotify(longText.ArtifactId, lineNumber);
			}
			else
			{
				_logger.LogWarning("Long text for {fileName} not found.", fileName);
			}
		}

		public void UpdateCompletedCount()
		{
			_logger.LogVerbose("Finalizing processed document count after batch has been completed.");
			foreach (Native native in _nativeRepository.GetNatives())
			{
				this.UpdateCompletedCount(native.Artifact.ArtifactID);
			}
		}

		public void SaveState()
		{
			lock (_syncObject)
			{
				_savedDocumentsDownloadedCount = _artifactsCompleted.Count;
			}
		}

		public void RestoreLastState()
		{
			_status.UpdateDocumentExportedCount(_savedDocumentsDownloadedCount);
		}

		private void MarkNativeAsCompleted(int lineNumber, Native native)
		{
			if (native.TransferCompleted)
			{
				this.NativeAlreadyProcessed(native);
			}
			else
			{
				native.TransferCompleted = true;
				this.UpdateCompletedCountAndNotify(native.Artifact.ArtifactID, lineNumber);
			}
		}

		private void MarkImageAsCompleted(string fileName, int lineNumber)
		{
			Image image = _imageRepository.GetByLineNumber(lineNumber);
			if (image != null)
			{
				if (image.TransferCompleted)
				{
					this.ImageAlreadyProcessed(image);
				}
				else
				{
					image.TransferCompleted = true;
					this.UpdateCompletedCountAndNotify(image.Artifact.ArtifactID, lineNumber);
				}
			}
			else
			{
				_logger.LogWarning("File for image {fileName} and line {lineNumber} not found.", fileName, lineNumber);
			}
		}

		/// <summary>
		///     TODO remove it after REL-206933 is fixed
		/// </summary>
		private void NativeAlreadyProcessed(Native native)
		{
			if (native.ExportRequest == null)
			{
				_logger.LogWarning("The export request of native {native} is Empty", native.Artifact?.ArtifactID);
			}

			IList<Native> duplicatedNatives = _nativeRepository.GetNatives()
				.Where(x => x.ExportRequest != null)
				.Where(x => x.ExportRequest.SourceLocation == native.ExportRequest.SourceLocation)
				.Where(x => x.ExportRequest.Order != native.ExportRequest.Order)
				.Where(x => !x.TransferCompleted).ToList();

			foreach (Native duplicatedNative in duplicatedNatives)
			{
				if (_fileWrapper.Exists(duplicatedNative.ExportRequest.DestinationLocation))
				{
					duplicatedNative.TransferCompleted = true;
					this.UpdateCompletedCountAndNotify(duplicatedNative.Artifact.ArtifactID, duplicatedNative.ExportRequest.Order);
				}
			}
		}

		/// <summary>
		///     TODO remove it after REL-206933 is fixed
		/// </summary>
		private void ImageAlreadyProcessed(Image image)
		{
			if (image.ExportRequest == null)
			{
				_logger.LogWarning("The export request of image {image} is Empty", image.Artifact?.ArtifactID);
			}

			IList<Image> duplicatedImages = _imageRepository.GetImages()
				.Where(x => x.ExportRequest != null)
				.Where(x => x.ExportRequest.SourceLocation == image.ExportRequest.SourceLocation)
				.Where(x => x.ExportRequest.Order != image.ExportRequest.Order)
				.Where(x => !x.TransferCompleted).ToList();

			foreach (Image duplicatedImage in duplicatedImages)
			{
				if (_fileWrapper.Exists(duplicatedImage.ExportRequest.DestinationLocation))
				{
					duplicatedImage.TransferCompleted = true;
					this.UpdateCompletedCountAndNotify(duplicatedImage.Artifact.ArtifactID, duplicatedImage.ExportRequest.Order);
				}
			}
		}

		private void UpdateCompletedCountAndNotify(int artifactId, int lineNumber)
		{
			_logger.LogVerbose("Updating completed document count after artifact {artifactId} transfer has been completed.",
				artifactId);
			bool documentCountUpdated = this.UpdateCompletedCount(artifactId);
			Native native = _nativeRepository.GetNative(artifactId);
			if (documentCountUpdated && native != null)
			{
				_logger.LogVerbose("Document {identifierValue} transfer completed.", native.Artifact.IdentifierValue);
				string suffixMessage = string.Empty;
				if (lineNumber > 0)
				{
					suffixMessage = $" (line number: {lineNumber})";
				}

				_status.WriteStatusLine(EventType2.Progress,
					$"Document {native.Artifact.IdentifierValue} transfer completed {suffixMessage}.", false);
			}
		}

		private bool UpdateCompletedCount(int artifactId)
		{
			//race condition may occur here, but after batch is downloaded we're refreshing 
			//the whole list, so final number of documents will be valid

			Native native = _nativeRepository.GetNative(artifactId);
			if (native == null)
			{
				return false;
			}

			int nativeArtifactId = native.Artifact.ArtifactID;

			if (!native.TransferCompleted)
			{
				return false;
			}
			IList<Image> images = _imageRepository.GetArtifactImages(nativeArtifactId);
			if (images.Any(x => !x.TransferCompleted))
			{
				return false;
			}

			IEnumerable<LongText> longTexts = _longTextRepository.GetArtifactLongTexts(nativeArtifactId);
			if (longTexts.Any(x => !x.HasBeenDownloaded))
			{
				return false;
			}

			lock (_syncObject)
			{
				if (!_artifactsCompleted.Contains(nativeArtifactId))
				{
					_artifactsCompleted.Add(nativeArtifactId);
					this.PublishProcessedCount();
					return true;
				}
				return false;
			}
		}

		private void PublishProcessedCount()
		{
			_status.UpdateDocumentExportedCount(_artifactsCompleted.Count);
		}
	}
}