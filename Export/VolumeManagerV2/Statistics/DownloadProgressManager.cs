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

		private readonly ConcurrentDictionary<int, bool> _artifactsDownloaded;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;

		private readonly IStatus _status;
		private readonly ILog _logger;

		public DownloadProgressManager(NativeRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository, IStatus status, ILog logger)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_status = status;
			_logger = logger;

			_artifactsDownloaded = new ConcurrentDictionary<int, bool>();
		}

		public void MarkFileAsDownloaded(string fileName, int lineNumber)
		{
			_logger.LogVerbose("Marking {fileName} file as downloaded.", fileName);
			Native native = _nativeRepository.GetByLineNumber(lineNumber);
			if (native != null)
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
			else
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

				_logger.LogWarning("File or Image for {fileName} not found.", fileName);
			}
		}

		/// <summary>
		///     TODO remove it after REL-206933 is fixed
		/// </summary>
		private void NativeAlreadyDownloaded(Native native)
		{
			IList<Native> duplicatedNatives = _nativeRepository.GetNatives()
				.Where(x => x.ExportRequest.SourceLocation == native.ExportRequest.SourceLocation)
				.Where(x => x.ExportRequest.Order != native.ExportRequest.Order)
				.Where(x => !x.HasBeenDownloaded).ToList();

			foreach (Native duplicatedNative in duplicatedNatives)
			{
				if (File.Exists(duplicatedNative.ExportRequest.DestinationLocation))
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
			IList<Image> duplicatedImages = _imageRepository.GetImages()
				.Where(x => x.ExportRequest.SourceLocation == image.ExportRequest.SourceLocation)
				.Where(x => x.ExportRequest.Order != image.ExportRequest.Order)
				.Where(x => !x.HasBeenDownloaded).ToList();

			foreach (Image duplicatedImage in duplicatedImages)
			{
				if (File.Exists(duplicatedImage.ExportRequest.DestinationLocation))
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
			_logger.LogVerbose("Updating downloaded document count after artifact {artifactId} has been downloaded.", artifactId);
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

				_status.WriteStatusLine(EventType.Progress, $"Document {native.Artifact.IdentifierValue} downloaded{suffixMessage}.", true);
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
			if (_artifactsDownloaded.ContainsKey(native.Artifact.ArtifactID) && _artifactsDownloaded[native.Artifact.ArtifactID])
			{
				return false;
			}

			if (!native.HasBeenDownloaded)
			{
				return false;
			}

			IList<Image> images = _imageRepository.GetArtifactImages(native.Artifact.ArtifactID);
			if (images.Any(x => !x.HasBeenDownloaded))
			{
				return false;
			}

			IList<LongText> longTexts = _longTextRepository.GetArtifactLongTexts(native.Artifact.ArtifactID);
			if (longTexts.Any(x => !x.HasBeenDownloaded))
			{
				return false;
			}

			_artifactsDownloaded.AddOrUpdate(native.Artifact.ArtifactID, true, (i, b) => true);
			_status.UpdateDocumentExportedCount(DownloadedDocumentsCount());
			return true;
		}

		private int DownloadedDocumentsCount()
		{
			return _artifactsDownloaded.Count(x => x.Value);
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