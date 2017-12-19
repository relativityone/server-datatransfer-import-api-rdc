using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Statistics
{
	public class DownloadProgressManager : IDownloadProgress
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

		public void MarkNativeAsDownloaded(string id)
		{
			_logger.LogVerbose("Marking {id} native as downloaded.", id);
			Native native = _nativeRepository.GetByUniqueId(id);
			if (native != null)
			{
				native.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(native.Artifact.ArtifactID);
			}
			else
			{
				_logger.LogWarning("Native for {id} not found.", id);
			}
		}

		public void MarkImageAsDownloaded(string id)
		{
			_logger.LogVerbose("Marking {id} image as downloaded.", id);
			Image image = _imageRepository.GetByUniqueId(id);
			if (image != null)
			{
				image.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(image.Artifact.ArtifactID);
			}
			else
			{
				_logger.LogWarning("Image for {id} not found.", id);
			}
		}

		public void MarkLongTextAsDownloaded(string id)
		{
			_logger.LogVerbose("Marking {id} long text as downloaded.", id);
			LongText longText = _longTextRepository.GetByUniqueId(id);
			if (longText != null)
			{
				longText.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(longText.ArtifactId);
			}
			else
			{
				_logger.LogWarning("Long text for {id} not found.", id);
			}
		}

		private void UpdateDownloadedCountAndNotify(int artifactId)
		{
			_logger.LogVerbose("Updating downloaded document count after artifact {artifactId} has been downloaded.", artifactId);
			bool documentCountUpdated = UpdateDownloadedCount(artifactId);
			Native native = _nativeRepository.GetNative(artifactId);
			if (documentCountUpdated && native != null)
			{
				_logger.LogVerbose("Document {identifierValue} downloaded.", native.Artifact.IdentifierValue);
				_status.WriteStatusLine(EventType.Progress, $"Document {native.Artifact.IdentifierValue} downloaded.", true);
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