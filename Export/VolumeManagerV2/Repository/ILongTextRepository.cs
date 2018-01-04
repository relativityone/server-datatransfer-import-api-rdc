using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;

namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository
{
	public interface ILongTextRepository
	{
		void Add(IList<LongText> longTexts);
		IList<LongText> GetArtifactLongTexts(int artifactId);
		LongText GetByUniqueId(string id);
		IList<LongTextExportRequest> GetExportRequests();
		LongText GetLongText(int artifactId, int fieldArtifactId);
		IList<LongText> GetLongTexts();
		string GetTextFileLocation(int artifactId, int fieldArtifactId);
	}
}