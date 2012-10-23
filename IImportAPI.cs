using System.Collections.Generic;
using System.Net;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.Data;
using kCura.Relativity.ImportAPI.Enumeration;

namespace kCura.Relativity.ImportAPI
{
	public interface IImportAPI
	{
		IEnumerable<Workspace> Workspaces();
		IEnumerable<ProductionSet> GetProductionSets(int workspaceArtifactID);
		IEnumerable<Field> GetWorkspaceFields(int workspaceArtifactID, int artifactTypeID);
		ImageImportBulkArtifactJob NewImageImportJob();
		ImageImportBulkArtifactJob NewProductionImportJob(int productionArtifactID);
		ImportBulkArtifactJob NewNativeDocumentImportJob();
		ImportBulkArtifactJob NewObjectImportJob(int artifactTypeId);
		UploadTypeEnum GetFileUploadMode(int caseArtifactID);
		IEnumerable<ArtifactType> GetUploadableArtifactTypes(int caseArtifactID);
	}
}