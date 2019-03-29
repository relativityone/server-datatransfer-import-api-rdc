namespace Relativity.Export.VolumeManagerV2.Repository
{
	using System.Collections.Generic;

	using Relativity.Export.VolumeManagerV2.Download;
	using Relativity.Export.VolumeManagerV2.Metadata.Text;

	public interface ILongTextRepository
	{
		void Add(IList<LongText> longTexts);
		IEnumerable<LongText> GetArtifactLongTexts(int artifactId);
		LongText GetByLineNumber(int lineNumber);
		IEnumerable<LongTextExportRequest> GetExportRequests();
		LongText GetLongText(int artifactId, int fieldArtifactId);
		IList<LongText> GetLongTexts();
		string GetTextFileLocation(int artifactId, int fieldArtifactId);
	}
}