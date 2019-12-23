namespace Relativity.DataExchange.Export.VolumeManagerV2.Repository
{
	using System;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;

	using System.Collections.Generic;
	using System.Linq;

	public class NativeRepository : IClearable
	{
		private List<Native> _natives;
		private Dictionary<int, Native> _nativesByArtifactIdDictionary;

		private readonly object _syncLock = new object();

		public NativeRepository()
		{
			_natives = new List<Native>();
			_nativesByArtifactIdDictionary = new Dictionary<int, Native>();
		}

		public void Add(Native native)
		{
			lock (_syncLock)
			{
				_natives.Add(native);
				_nativesByArtifactIdDictionary[native.Artifact.ArtifactID] = native;
			}
		}

		public Native GetNative(int artifactId)
		{
			lock (_syncLock)
			{
				_nativesByArtifactIdDictionary.TryGetValue(artifactId, out var native);
				return native;
			}
		}

		public IList<Native> GetNativesByTargetFile(string targetFile)
		{
			lock (_syncLock)
			{
				string trimmedTargetFile = targetFile != null ? targetFile.TrimEnd() : string.Empty;
				List<Native> natives = _natives.Where(
					x => x.ExportRequest?.DestinationLocation != null && string.Compare(
						     x.ExportRequest.DestinationLocation.TrimEnd(),
						     trimmedTargetFile,
						     StringComparison.OrdinalIgnoreCase) == 0).ToList();
				return natives;
			}
		}

		public IList<Native> GetNatives()
		{
			lock (_syncLock)
			{
				return _natives;
			}
		}

		public IEnumerable<ExportRequest> GetExportRequests()
		{
			//Only to sync access to natives
			lock (_syncLock)
			{
				return _natives.Where(x => !x.TransferCompleted).Select(x => x.ExportRequest);
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
				_natives = new List<Native>();
				_nativesByArtifactIdDictionary = new Dictionary<int, Native>();
			}
		}
	}
}