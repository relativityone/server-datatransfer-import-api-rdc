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
		IEnumerable<Workspace> Workspaces();

		/// <summary>
		/// Returns all production sets belonging to the workspace.
		/// </summary>
		/// <param name="workspaceArtifactID">The ArtifactID of the workspace.</param>
		IEnumerable<ProductionSet> GetProductionSets(int workspaceArtifactID);

		/// <summary>
		/// Returns all fields related to the given ArtifactType in the given workspace.
		/// </summary>
		/// <param name="workspaceArtifactID">The ArtifactID of the workspace holding the fields.</param>
		/// <param name="artifactTypeID">The ID of the ArtifactType.</param>
		IEnumerable<Field> GetWorkspaceFields(int workspaceArtifactID, int artifactTypeID);

		/// <summary>
		/// Creates a new job to import images in bulk.
		/// </summary>
		/// <returns>Returns a new ImageImportBulkArtifactJob.</returns>
		ImageImportBulkArtifactJob NewImageImportJob();

		/// <summary>
		/// Creates a new job to import production images in bulk.
		/// </summary>
		/// <param name="productionArtifactID">The ArtifactID of the production to which the images will belong.</param>
		/// <returns></returns>
		ImageImportBulkArtifactJob NewProductionImportJob(int productionArtifactID);

		/// <summary>
		/// Creates a new job to import native documents in bulk.
		/// </summary>
		/// <returns>Returns a new ImportBulkArtifactJob.</returns>
		ImportBulkArtifactJob NewNativeDocumentImportJob();

		/// <summary>
		/// Creates a new job to import objects in bulk.
		/// </summary>
		/// <param name="artifactTypeId">The ArtifactTypeID of the Artifacts to be imported.</param>
		/// <returns>Returns a new ImportBulkArtifactJob.</returns>
		ImportBulkArtifactJob NewObjectImportJob(int artifactTypeId);

		/// <summary>
		/// Returns the mode in which the files will be uploaded.
		/// </summary>
		/// <param name="caseArtifactID">The ArtifactID of the case that will hold the files.</param>
		UploadTypeEnum GetFileUploadMode(int caseArtifactID);

		/// <summary>
		/// Returns a collection of the ArtifactTypes it is possible to upload.
		/// </summary>
		/// <param name="caseArtifactID">The ArtifactID of the case to which Artifacts of the returned ArtifactType can be uploaded.</param>
		IEnumerable<ArtifactType> GetUploadableArtifactTypes(int caseArtifactID);
	}
}