using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public class NativeRepository : IClearable
	{
		private List<Native> _natives;

		public NativeRepository()
		{
			_natives = new List<Native>();
		}

		public void Add(IList<Native> natives)
		{
			_natives.AddRange(natives);
		}

		public Native GetNative(int artifactId)
		{
			return _natives.First(x => x.Artifact.ArtifactID == artifactId);
		}

		public IList<Native> GetNatives()
		{
			return _natives;
		}

		public IList<ExportRequest> GetExportRequests()
		{
			return _natives.Where(x => !x.HasBeenDownloaded).Select(x => x.ExportRequest).ToList();
		}

		public Native GetByUniqueId(string id)
		{
			return _natives.FirstOrDefault(x => !x.HasBeenDownloaded && x.ExportRequest.UniqueId == id);
		}

		public void Clear()
		{
			_natives = new List<Native>();
		}
	}
}