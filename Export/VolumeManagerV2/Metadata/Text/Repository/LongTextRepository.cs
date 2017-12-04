using System.Collections.Generic;
using System.Linq;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository
{
	public class LongTextRepository
	{
		private readonly IList<LongText> _longTexts;

		public LongTextRepository()
		{
			_longTexts = new List<LongText>();
		}

		public void Add(IList<LongText> longTexts)
		{
			foreach (var longText in longTexts)
			{
				_longTexts.Add(longText);
			}
		}

		public string GetTextFileLocation(int artifactId, int fieldArtifactId)
		{
			return _longTexts.First(x => x.FieldArtifactId == fieldArtifactId && x.ArtifactId == artifactId).Location;
		}

		public IList<LongText> GetLongTexts()
		{
			return _longTexts;
		}
	}
}