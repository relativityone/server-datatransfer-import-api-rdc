using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
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
				return _nativesByArtifactIdDictionary[artifactId];
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
				return _natives.Where(x => !x.HasBeenDownloaded).Select(x => (ExportRequest) x.ExportRequest);
			}
		}

		public bool AnyRequestForLocation(string destinationLocation)
		{
			lock (_syncLock)
			{
				return GetExportRequests().Any(x => x.DestinationLocation == destinationLocation);
			}
		}

		public Native GetByLineNumber(int lineNumber)
		{
			lock (_syncLock)
			{
				return _natives.FirstOrDefault(x => x.ExportRequest?.Order == lineNumber);
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