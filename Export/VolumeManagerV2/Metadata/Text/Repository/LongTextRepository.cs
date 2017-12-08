using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextRepository : IRepository
	{
		private List<LongText> _longTexts;

		public LongTextRepository()
		{
			_longTexts = new List<LongText>();
		}

		public void Add(IList<LongText> longTexts)
		{
			_longTexts.AddRange(longTexts);
		}

		public string GetTextFileLocation(int artifactId, int fieldArtifactId)
		{
			return GetLongText(artifactId, fieldArtifactId).Location;
		}

		public LongText GetLongText(int artifactId, int fieldArtifactId)
		{
			return _longTexts.First(x => x.FieldArtifactId == fieldArtifactId && x.ArtifactId == artifactId);
		}

		public IList<LongText> GetLongTexts()
		{
			return _longTexts;
		}

		public IList<LongTextExportRequest> GetExportRequests()
		{
			return _longTexts.Select(x => x.ExportRequest).Where(x => x != null).ToList();
		}

		public LongText GetByUniqueId(string id)
		{
			return _longTexts.FirstOrDefault(x => x.ExportRequest != null && x.ExportRequest.UniqueId == id);
		}

		public IList<LongText> GetArtifactLongTexts(int artifactId)
		{
			return _longTexts.Where(x => x.ArtifactId == artifactId).ToList();
		}

		public void Clear()
		{
			_longTexts = new List<LongText>();
		}
	}
}