namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using System;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	using System.Collections.Generic;
	using System.Linq;

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
				return _images.FirstOrDefault(
					x => x.Artifact.ArtifactID == artifactId && string.Compare(
						     x.Artifact.BatesNumber,
						     batesNumber,
						     StringComparison.OrdinalIgnoreCase) == 0);
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
				return _images.Where(x => !x.TransferCompleted).Select(x => x.ExportRequest);
			}
		}

		public bool AnyRequestForLocation(string destinationLocation)
		{
			if (string.IsNullOrWhiteSpace(destinationLocation))
			{
				return false;
			}

			lock (_syncLock)
			{
				return GetExportRequests().Any(
					x => string.Compare(x.DestinationLocation, destinationLocation, StringComparison.OrdinalIgnoreCase)
					     == 0);
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