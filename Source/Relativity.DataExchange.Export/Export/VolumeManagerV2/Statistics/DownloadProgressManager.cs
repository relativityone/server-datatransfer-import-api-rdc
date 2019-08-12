namespace Relativity.DataExchange.Export.VolumeManagerV2.Statistics
{
	using System.Collections.Generic;
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

		private readonly ThreadSafeAddOnlyHashSet<int> _artifactsCompleted;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly IFile _fileWrapper;

		private readonly IStatus _status;
		private readonly ILog _logger;

		public DownloadProgressManager(NativeRepository nativeRepository, ImageRepository imageRepository,
			LongTextRepository longTextRepository, IFile fileWrapper, IStatus status, ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_fileWrapper = fileWrapper;
			_status = status;
			_logger = logger;

			this._artifactsCompleted = new ThreadSafeAddOnlyHashSet<int>();
		}

		public void MarkFileAsCompleted(string fileName, int lineNumber)
		{
			_logger.LogVerbose("Marking {fileName} file as downloaded.", fileName);
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
			_logger.LogVerbose("Marking {fileName} long text as downloaded.", fileName);
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
			_logger.LogVerbose("Finalizing downloaded document count after batch has been downloaded.");
			foreach (Native native in _nativeRepository.GetNatives())
			{
				this.UpdateCompletedCount(native.Artifact.ArtifactID);
			}
		}

		public void SaveState()
		{
			_savedDocumentsDownloadedCount = this.CompletedDocumentsCount();
		}

		public void RestoreLastState()
		{
			_status.UpdateDocumentExportedCount(_savedDocumentsDownloadedCount);
		}

		private void MarkNativeAsCompleted(int lineNumber, Native native)
		{
			if (native.HasBeenTransferCompleted)
			{
				this.NativeAlreadyProcessed(native);
			}
			else
			{
				native.HasBeenTransferCompleted = true;
				this.UpdateCompletedCountAndNotify(native.Artifact.ArtifactID, lineNumber);
			}
		}

		private void MarkImageAsCompleted(string fileName, int lineNumber)
		{
			Image image = _imageRepository.GetByLineNumber(lineNumber);
			if (image != null)
			{
				if (image.HasBeenTransferCompleted)
				{
					this.ImageAlreadyProcessed(image);
				}
				else
				{
					image.HasBeenTransferCompleted = true;
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
				.Where(x => !x.HasBeenTransferCompleted).ToList();

			foreach (Native duplicatedNative in duplicatedNatives)
			{
				if (_fileWrapper.Exists(duplicatedNative.ExportRequest.DestinationLocation))
				{
					duplicatedNative.HasBeenTransferCompleted = true;
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
				.Where(x => !x.HasBeenTransferCompleted).ToList();

			foreach (Image duplicatedImage in duplicatedImages)
			{
				if (_fileWrapper.Exists(duplicatedImage.ExportRequest.DestinationLocation))
				{
					duplicatedImage.HasBeenTransferCompleted = true;
					this.UpdateCompletedCountAndNotify(duplicatedImage.Artifact.ArtifactID, duplicatedImage.ExportRequest.Order);
				}
			}
		}

		private void UpdateCompletedCountAndNotify(int artifactId, int lineNumber)
		{
			_logger.LogVerbose("Updating downloaded document count after artifact {artifactId} has been downloaded.",
				artifactId);
			bool documentCountUpdated = this.UpdateCompletedCount(artifactId);
			Native native = _nativeRepository.GetNative(artifactId);
			if (documentCountUpdated && native != null)
			{
				_logger.LogVerbose("Document {identifierValue} downloaded.", native.Artifact.IdentifierValue);
				string suffixMessage = string.Empty;
				if (lineNumber > 0)
				{
					suffixMessage = $" (line number: {lineNumber})";
				}

				_status.WriteStatusLine(EventType2.Progress,
					$"Document {native.Artifact.IdentifierValue} downloaded{suffixMessage}.", false);
			}
		}

		private bool UpdateCompletedCount(int artifactId)
		{
			//race condition may occur here, but after batch is downloaded we're refreshing 
			//the whole list, so final number of documents will be valid

			Native native = _nativeRepository.GetNative(artifactId);
			int nativeArtifactId = native.Artifact.ArtifactID;

			if (!native.HasBeenTransferCompleted)
			{
				return false;
			}

			if (this._artifactsCompleted.Contains(nativeArtifactId))
			{
				return false;
			}

			IList<Image> images = _imageRepository.GetArtifactImages(nativeArtifactId);
			if (images.Any(x => !x.HasBeenTransferCompleted))
			{
				return false;
			}

			IEnumerable<LongText> longTexts = _longTextRepository.GetArtifactLongTexts(nativeArtifactId);
			if (longTexts.Any(x => !x.HasBeenDownloaded))
			{
				return false;
			}

			this._artifactsCompleted.Add(nativeArtifactId);
			_status.UpdateDocumentExportedCount(this.CompletedDocumentsCount());
			return true;
		}

		private int CompletedDocumentsCount()
		{
			return this._artifactsCompleted.Count;
		}

	}

}