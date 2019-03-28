using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class ImageRepository : IClearable
	{
		private List<Image> _images;
		private readonly object _syncLock = new object();

		public ImageRepository()
		{
			_images = new List<Image>();
		}

		public void Add(IList<Image> images)
		{
			lock (_syncLock)
			{
				_images.AddRange(images);
			}
		}

		public Image GetImage(int artifactId, string batesNumber)
		{
			lock (_syncLock)
			{
				return _images.First(x => x.Artifact.ArtifactID == artifactId && x.Artifact.BatesNumber == batesNumber);
			}
		}

		public IList<Image> GetArtifactImages(int artifactId)
		{
			lock (_syncLock)
			{
				return _images.Where(x => x.Artifact.ArtifactID == artifactId).ToList();
			}
		}

		public IList<Image> GetImages()
		{
			lock (_syncLock)
			{
				return _images;
			}
		}

		public IEnumerable<ExportRequest> GetExportRequests()
		{
			lock (_syncLock)
			{
				return _images.Where(x => !x.HasBeenDownloaded).Select(x => (ExportRequest) x.ExportRequest);
			}
		}

		public bool AnyRequestForLocation(string destinationLocation)
		{
			lock (_syncLock)
			{
				return GetExportRequests().Any(x => x.DestinationLocation == destinationLocation);
			}
		}

		public Image GetByLineNumber(int lineNumber)
		{
			lock (_syncLock)
			{
				return _images.FirstOrDefault(x => x.ExportRequest?.Order == lineNumber);
			}
		}

		public void Clear()
		{
			lock (_syncLock)
			{
				_images = new List<Image>();
			}
		}
	}
}