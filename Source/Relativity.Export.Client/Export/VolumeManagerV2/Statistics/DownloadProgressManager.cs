using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class DownloadProgressManager : IDownloadProgress, IDownloadProgressManager
	{
		private int _savedDocumentsDownloadedCount;

		private readonly ThreadSafeAddOnlyHashSet<int> _artifactsDownloaded;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;
		private readonly IFileHelper _fileHelper;

		private readonly IStatus _status;
		private readonly ILog _logger;

		public DownloadProgressManager(NativeRepository nativeRepository, ImageRepository imageRepository,
			LongTextRepository longTextRepository, IFileHelper fileHelper, IStatus status, ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_fileHelper = fileHelper;
			_status = status;
			_logger = logger;

			_artifactsDownloaded = new ThreadSafeAddOnlyHashSet<int>();
		}

		public void MarkFileAsDownloaded(string fileName, int lineNumber)
		{
			_logger.LogVerbose("Marking {fileName} file as downloaded.", fileName);
			Native native = _nativeRepository.GetByLineNumber(lineNumber);
			if (native != null)
			{
				MarkNativeAsDownloaded(lineNumber, native);
			}
			else
			{
				MarkImageAsDownloaded(fileName, lineNumber);
			}
		}

		private void MarkNativeAsDownloaded(int lineNumber, Native native)
		{
			if (native.HasBeenDownloaded)
			{
				NativeAlreadyDownloaded(native);
			}
			else
			{
				native.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(native.Artifact.ArtifactID, lineNumber);
			}
		}

		private void MarkImageAsDownloaded(string fileName, int lineNumber)
		{
			Image image = _imageRepository.GetByLineNumber(lineNumber);
			if (image != null)
			{
				if (image.HasBeenDownloaded)
				{
					ImageAlreadyDownloaded(image);
				}
				else
				{
					image.HasBeenDownloaded = true;
					UpdateDownloadedCountAndNotify(image.Artifact.ArtifactID, lineNumber);
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
		private void NativeAlreadyDownloaded(Native native)
		{
			if (native.ExportRequest == null)
			{
				_logger.LogWarning("The export request of native {native} is Empty", native.Artifact?.ArtifactID);
			}

			IList<Native> duplicatedNatives = _nativeRepository.GetNatives()
				.Where(x => x.ExportRequest != null)
				.Where(x => x.ExportRequest.SourceLocation == native.ExportRequest.SourceLocation)
				.Where(x => x.ExportRequest.Order != native.ExportRequest.Order)
				.Where(x => !x.HasBeenDownloaded).ToList();

			foreach (Native duplicatedNative in duplicatedNatives)
			{
				if (_fileHelper.Exists(duplicatedNative.ExportRequest.DestinationLocation))
				{
					duplicatedNative.HasBeenDownloaded = true;
					UpdateDownloadedCountAndNotify(duplicatedNative.Artifact.ArtifactID, duplicatedNative.ExportRequest.Order);
				}
			}
		}

		/// <summary>
		///     TODO remove it after REL-206933 is fixed
		/// </summary>
		private void ImageAlreadyDownloaded(Image image)
		{
			if (image.ExportRequest == null)
			{
				_logger.LogWarning("The export request of image {image} is Empty", image.Artifact?.ArtifactID);
			}

			IList<Image> duplicatedImages = _imageRepository.GetImages()
				.Where(x => x.ExportRequest != null)
				.Where(x => x.ExportRequest.SourceLocation == image.ExportRequest.SourceLocation)
				.Where(x => x.ExportRequest.Order != image.ExportRequest.Order)
				.Where(x => !x.HasBeenDownloaded).ToList();

			foreach (Image duplicatedImage in duplicatedImages)
			{
				if (_fileHelper.Exists(duplicatedImage.ExportRequest.DestinationLocation))
				{
					duplicatedImage.HasBeenDownloaded = true;
					UpdateDownloadedCountAndNotify(duplicatedImage.Artifact.ArtifactID, duplicatedImage.ExportRequest.Order);
				}
			}
		}

		public void MarkLongTextAsDownloaded(string fileName, int lineNumber)
		{
			_logger.LogVerbose("Marking {fileName} long text as downloaded.", fileName);
			LongText longText = _longTextRepository.GetByLineNumber(lineNumber);
			if (longText != null)
			{
				longText.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(longText.ArtifactId, lineNumber);
			}
			else
			{
				_logger.LogWarning("Long text for {fileName} not found.", fileName);
			}
		}

		private void UpdateDownloadedCountAndNotify(int artifactId, int lineNumber)
		{
			_logger.LogVerbose("Updating downloaded document count after artifact {artifactId} has been downloaded.",
				artifactId);
			bool documentCountUpdated = UpdateDownloadedCount(artifactId);
			Native native = _nativeRepository.GetNative(artifactId);
			if (documentCountUpdated && native != null)
			{
				_logger.LogVerbose("Document {identifierValue} downloaded.", native.Artifact.IdentifierValue);
				string suffixMessage = string.Empty;
				if (lineNumber > 0)
				{
					suffixMessage = $" (line number: {lineNumber})";
				}

				_status.WriteStatusLine(EventType.Progress,
					$"Document {native.Artifact.IdentifierValue} downloaded{suffixMessage}.", false);
			}
		}

		public void UpdateDownloadedCount()
		{
			_logger.LogVerbose("Finalizing downloaded document count after batch has been downloaded.");
			foreach (Native native in _nativeRepository.GetNatives())
			{
				UpdateDownloadedCount(native.Artifact.ArtifactID);
			}
		}

		private bool UpdateDownloadedCount(int artifactId)
		{
			//race condition may occur here, but after batch is downloaded we're refreshing 
			//the whole list, so final number of documents will be valid

			Native native = _nativeRepository.GetNative(artifactId);
			int nativeArtifactId = native.Artifact.ArtifactID;

			if (!native.HasBeenDownloaded)
			{
				return false;
			}

			if (_artifactsDownloaded.Contains(nativeArtifactId))
			{
				return false;
			}

			IList<Image> images = _imageRepository.GetArtifactImages(nativeArtifactId);
			if (images.Any(x => !x.HasBeenDownloaded))
			{
				return false;
			}

			IEnumerable<LongText> longTexts = _longTextRepository.GetArtifactLongTexts(nativeArtifactId);
			if (longTexts.Any(x => !x.HasBeenDownloaded))
			{
				return false;
			}

			_artifactsDownloaded.Add(nativeArtifactId);
			_status.UpdateDocumentExportedCount(DownloadedDocumentsCount());
			return true;
		}

		private int DownloadedDocumentsCount()
		{
			return _artifactsDownloaded.Count;
		}

		public void SaveState()
		{
			_savedDocumentsDownloadedCount = DownloadedDocumentsCount();
		}

		public void RestoreLastState()
		{
			_status.UpdateDocumentExportedCount(_savedDocumentsDownloadedCount);
		}
	}

}