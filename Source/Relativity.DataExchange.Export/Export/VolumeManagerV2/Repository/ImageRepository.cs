namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using System;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	using System.Collections.Generic;
	using System.Linq;

	using kCura.WinEDDS.Exporters;

	public class ImageRepository : IClearable
	{
		private List<ImageRequest> _images;
		private readonly object _syncLock = new object();

		public ImageRepository()
		{
			_images = new List<ImageRequest>();
		}

		public void Add(IList<ImageRequest> images)
		{
			lock (_syncLock)
			{
				_images.AddRange(images);
			}
		}

		public ImageRequest GetImage(int artifactId, string batesNumber)
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

		public IList<ImageRequest> GetImagesByTargetFile(string targetFile)
		{
			lock (_syncLock)
			{
				string trimmedTargetFile = targetFile != null ? targetFile.TrimEnd() : string.Empty;
				List<ImageRequest> images = _images.Where(
					x => x.ExportRequest?.DestinationLocation != null && string.Compare(
						     x.ExportRequest.DestinationLocation.TrimEnd(),
						     trimmedTargetFile,
						     StringComparison.OrdinalIgnoreCase) == 0).ToList();
				return images;
			}
		}

		public IList<ImageRequest> GetArtifactImages(int artifactId)
		{
			lock (_syncLock)
			{
				return _images.Where(x => x.Artifact.ArtifactID == artifactId).ToList();
			}
		}

		public IList<ImageRequest> GetImages()
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

		public void Clear()
		{
			lock (_syncLock)
			{
				_images = new List<ImageRequest>();
			}
		}
	}
}