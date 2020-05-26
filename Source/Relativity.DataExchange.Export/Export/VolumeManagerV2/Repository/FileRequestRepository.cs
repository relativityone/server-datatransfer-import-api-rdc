
namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.Linq;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	public class FileRequestRepository : IClearable
	{
		private readonly ConcurrentDictionary<int, FileRequest<ObjectExportInfo>> _filesByArtifactIdDictionary;

		public FileRequestRepository()
		{
			this._filesByArtifactIdDictionary = new ConcurrentDictionary<int, FileRequest<ObjectExportInfo>>();
		}

		public void Add(FileRequest<ObjectExportInfo> file)
		{
			this._filesByArtifactIdDictionary[file.Artifact.ArtifactID] = file;
		}

		public FileRequest<ObjectExportInfo> GetFileRequest(int artifactId)
		{
			this._filesByArtifactIdDictionary.TryGetValue(artifactId, out var file);
			return file;
		}

		public IList<FileRequest<ObjectExportInfo>> GetFileRequestByDestinationLocation(string destinationLocation)
		{
			string trimmedDestinationLocation = destinationLocation != null ? destinationLocation.TrimEnd() : string.Empty;
			List<FileRequest<ObjectExportInfo>> fileRequests = this._filesByArtifactIdDictionary.Values.Where(
				x => x.ExportRequest?.DestinationLocation != null && string.Compare(
						 x.ExportRequest.DestinationLocation.TrimEnd(),
						 trimmedDestinationLocation,
						 StringComparison.OrdinalIgnoreCase) == 0).ToList();
			return fileRequests;
		}

		public IList<FileRequest<ObjectExportInfo>> GetFileRequests()
		{
			return this._filesByArtifactIdDictionary.Values.ToList();
		}

		public IEnumerable<ExportRequest> GetExportRequests()
		{
			return this._filesByArtifactIdDictionary.Values.Where(x => !x.TransferCompleted).Select(x => x.ExportRequest);
		}

		public bool AnyRequestForLocation(string destinationLocation)
		{
			if (string.IsNullOrWhiteSpace(destinationLocation))
			{
				return false;
			}

			return this.GetFileRequestByDestinationLocation(destinationLocation).Any(x => !x.TransferCompleted);
		}

		public void Clear()
		{
			this._filesByArtifactIdDictionary.Clear();
		}
	}
}
