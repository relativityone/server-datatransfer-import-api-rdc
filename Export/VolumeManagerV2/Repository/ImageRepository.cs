using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class ImageRepository : IClearable
	{
		private List<Image> _images;

		public ImageRepository()
		{
			_images = new List<Image>();
		}

		public void Add(IList<Image> images)
		{
			_images.AddRange(images);
		}

		public Image GetImage(int artifactId, string batesNumber)
		{
			return _images.First(x => x.Artifact.ArtifactID == artifactId && x.Artifact.BatesNumber == batesNumber);
		}

		public IList<Image> GetArtifactImages(int artifactId)
		{
			return _images.Where(x => x.Artifact.ArtifactID == artifactId).ToList();
		}

		public IList<Image> GetImages()
		{
			return _images;
		}

		public IList<ExportRequest> GetExportRequests()
		{
			return _images.Where(x => !x.HasBeenDownloaded).Select(x => (ExportRequest) x.ExportRequest).ToList();
		}

		public Image GetByLineNumber(int lineNumber)
		{
			return _images.FirstOrDefault(x => x.ExportRequest?.Order == lineNumber);
		}

		public void Clear()
		{
			_images = new List<Image>();
		}
	}
}