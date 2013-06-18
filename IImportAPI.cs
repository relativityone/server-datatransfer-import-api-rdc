using System.Collections.Generic;
using System.Net;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.Data;
using kCura.Relativity.ImportAPI.Enumeration;

namespace kCura.Relativity.ImportAPI
{
	/// <summary>
	/// Provides methods for obtaining data on the current status of workspaces and cases.
	/// Also provides methods that create jobs for importing images and native documents.
	/// </summary>
	public interface IImportAPI
	{
		/// <summary>
		/// Returns all available workspaces.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Workspace> Workspaces();

		/// <summary>
		/// Returns all production sets belonging to the workspace.
		/// </summary>
		/// <param name="workspaceArtifactID">The artifact ID of the workspace.</param>
		/// <returns></returns>
		IEnumerable<ProductionSet> GetProductionSets(int workspaceArtifactID);

		/// <summary>
		/// Returns all fields related to the given artifact type in the given workspace.
		/// </summary>
		/// <param name="workspaceArtifactID">The ID of the workspace holding the artifact type.</param>
		/// <param name="artifactTypeID">The ID of the artifact type related to the fields.</param>
		/// <returns></returns>
		IEnumerable<Field> GetWorkspaceFields(int workspaceArtifactID, int artifactTypeID);

		/// <summary>
		/// Creates a new job to import images in bulk.
		/// </summary>
		/// <returns></returns>
		ImageImportBulkArtifactJob NewImageImportJob();

		/// <summary>
		/// Creates a new job to import production images in bulk.
		/// </summary>
		/// <param name="productionArtifactID">The ID of the production to which the images will belong.</param>
		/// <returns></returns>
		ImageImportBulkArtifactJob NewProductionImportJob(int productionArtifactID);

		/// <summary>
		/// Creates a new job to import native documents in bulk.
		/// </summary>
		/// <returns></returns>
		ImportBulkArtifactJob NewNativeDocumentImportJob();

		/// <summary>
		/// Creates a new job to import objects in bulk.
		/// </summary>
		/// <param name="artifactTypeId">The ID of the artifact type that will be imported.</param>
		/// <returns></returns>
		ImportBulkArtifactJob NewObjectImportJob(int artifactTypeId);

		/// <summary>
		/// Returns the mode in which the files will be uploaded.
		/// </summary>
		/// <param name="caseArtifactID">The ID of the case that will hold the files.</param>
		/// <returns></returns>
		UploadTypeEnum GetFileUploadMode(int caseArtifactID);

		/// <summary>
		/// Returns a collection of the artifact types it is possible to upload.
		/// </summary>
		/// <param name="caseArtifactID">The ID of the case that will hold the artifacts.</param>
		/// <returns></returns>
		IEnumerable<ArtifactType> GetUploadableArtifactTypes(int caseArtifactID);
	}
}