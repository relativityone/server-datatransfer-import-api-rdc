using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using kCura.Windows.Process;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Download
{
	public class DownloadStatistics
	{
		private readonly ConcurrentDictionary<int, bool> _artifactsDownloaded;

		private readonly NativeRepository _nativeRepository;
		private readonly ImageRepository _imageRepository;
		private readonly LongTextRepository _longTextRepository;

		private readonly IStatus _status;

		public DownloadStatistics(NativeRepository nativeRepository, ImageRepository imageRepository, LongTextRepository longTextRepository, IStatus status)
		{
			_nativeRepository = nativeRepository;
			_imageRepository = imageRepository;
			_longTextRepository = longTextRepository;
			_status = status;

			_artifactsDownloaded = new ConcurrentDictionary<int, bool>();
		}

		public void MarkNativeAsDownloaded(string id)
		{
			Native native = _nativeRepository.GetByUniqueId(id);
			if (native != null)
			{
				native.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(native.Artifact.ArtifactID, id);
			}
		}

		public void MarkImageAsDownloaded(string id)
		{
			Image image = _imageRepository.GetByUniqueId(id);
			if (image != null)
			{
				image.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(image.Artifact.ArtifactID, id);
			}
		}

		public void MarkLongTextAsDownloaded(string id)
		{
			LongText longText = _longTextRepository.GetByUniqueId(id);
			if (longText != null)
			{
				longText.HasBeenDownloaded = true;
				UpdateDownloadedCountAndNotify(longText.ArtifactId, id);
			}
		}

		private void UpdateDownloadedCountAndNotify(int artifactId, string id)
		{
			bool documentCountUpdated = UpdateDownloadedCount(artifactId);
			if (documentCountUpdated)
			{
				_status.WriteStatusLine(EventType.Progress, $"Document downloaded {id}.", true);
			}
		}

		public void FinalizeDownloadedCount()
		{
			foreach (Native native in _nativeRepository.GetNatives())
			{
				UpdateDownloadedCount(native.Artifact.ArtifactID);
			}
			_status.WriteStatusLine(EventType.Progress, "Files for batch downloaded.", true);
		}

		private bool UpdateDownloadedCount(int artifactId)
		{
			//TODO race condition may occur, but we should be good for now

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

		public int DownloadedDocumentsCount()
		{
			return _artifactsDownloaded.Count(x => x.Value);
		}
	}
}