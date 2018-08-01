using kCura.Relativity.DataReaderClient;

namespace kCura.Relativity.ImportAPI
{
	public interface IExtendedImportAPI : IImportAPI
	{
		void CleanUpAfterJobWithSpoofing(string onBehalfOfUserToken);
		ImportBulkArtifactJob NewNativeDocumentImportJob(string token);
		ImportBulkArtifactJob NewArtifactImportJob(string token, int artifactTypeID);
	}
}