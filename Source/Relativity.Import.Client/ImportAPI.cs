using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using kCura.Relativity.ImportAPI.Data;
using kCura.Relativity.ImportAPI.Enumeration;
using kCura.WinEDDS;
using kCura.WinEDDS.Service;
using kCura.Relativity.DataReaderClient;
using kCura.WinEDDS.Exceptions;
using Relativity;

namespace kCura.Relativity.ImportAPI
{
	/// <summary>
	/// Provides methods for developing custom import utilities for documents, images, production sets, and Dynamic Objects.
	/// </summary>
	/// <remarks>
	/// Also provides methods for retrieving workspaces, fields, and other objects.
	/// </remarks>
	public  class ImportAPI : IImportAPI
	{
	    /// <summary>
	    /// The lazy-loaded case manager instance.
	    /// </summary>
	    private CaseManager _caseManager;

        /// <summary>
        /// The current Transfer API credentials.
        /// </summary>
        private NetworkCredential _tapiCredentials;

        /// <summary>
        /// The lazy-loaded object type manager instance.
        /// </summary>
        private ObjectTypeManager _objectTypeManager;

        /// <summary>
        /// The lazy-loaded production manager instance.
        /// </summary>
        private ProductionManager _productionManager;

		/// <summary>
		/// Holds cookies for the current session.
		/// </summary>
		protected CookieContainer _cookieMonster;

		/// <summary>
		/// The current WebAPI credentials.
		/// </summary>
		protected ICredentials _credentials;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportAPI"/> class using the Integrated Authentication provider.
        /// </summary>
        /// <remarks>
        /// The user will be validated against the Relativity WebAPI instance located at <paramref name="webServiceUrl"/>.
        /// </remarks>
        /// <param name="webServiceUrl">
        /// The URL to the Relativity WebAPI instance.
        /// </param>
        /// <exception cref="kCura.WinEDDS.Exceptions.CredentialsNotSupportedException">
        /// Thrown when integrated security is not supported when running within a service process.
        /// </exception>
        /// <exception cref="kCura.WinEDDS.Exceptions.InvalidLoginException">
        /// Thrown when an authentication failure occurs.
        /// </exception>
        public ImportAPI(string webServiceUrl)
	    {
	        this.ExecutionSource = ExecutionSourceEnum.ImportAPI;
	        this.PerformLogin(null, null, webServiceUrl);
	    }

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportAPI"/> class.
        /// Uses the Password Authentication provider when <paramref name="userName"/> and <paramref name="password"/> are specified; otherwise, Integrated Authentication provider.
        /// </summary>
        /// <remarks>
        /// User name and password are required (unless using Integrated Security) and will be validated.
        /// The ImportAPI tries to resolve the server name by reading the WebServiceURL key from the local app.config file.  If this fails, it checks the Windows Registry for the location set by the Relativity Desktop Client.
        /// </remarks>
        /// <param name="userName">
        /// The Relativity login user name.
        /// </param>
        /// <param name="password">
        /// The Relativity login password.
        /// </param>
        /// <exception cref="kCura.WinEDDS.Exceptions.CredentialsNotSupportedException">
        /// Thrown when integrated security is not supported when running within a service process.
        /// </exception>
        /// <exception cref="kCura.WinEDDS.Exceptions.InvalidLoginException">
        /// Thrown when an authentication failure occurs.
        /// </exception>
        public ImportAPI(string userName, string password)
		{
			this.ExecutionSource = ExecutionSourceEnum.ImportAPI;
			PerformLogin(userName, password, string.Empty );
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ImportAPI"/> class.
        /// Uses the Password Authentication provider when <paramref name="userName"/> and <paramref name="password"/> are specified; otherwise, Integrated Authentication provider.
        /// </summary>
        /// <remarks>
        /// User name and password are required (unless using Integrated Security) and will be validated
        /// against the Relativity WebAPI instance located at <paramref name="webServiceUrl"/>.
        /// </remarks>
        /// <param name="userName">
        /// The Relativity login user name.
        /// </param>
        /// <param name="password">
        /// The Relativity login password.
        /// </param>
        /// <param name="webServiceUrl">
        /// The URL to the Relativity WebAPI instance.
        /// </param>
        /// <exception cref="kCura.WinEDDS.Exceptions.CredentialsNotSupportedException">
        /// Thrown when integrated security is not supported when running within a service process.
        /// </exception>
        /// <exception cref="kCura.WinEDDS.Exceptions.InvalidLoginException">
        /// Thrown when an authentication failure occurs.
        /// </exception>
        public ImportAPI(string userName, string password, string webServiceUrl)
        {
            ExecutionSource = ExecutionSourceEnum.ImportAPI;
			this.PerformLogin(userName, password, webServiceUrl);
		}

        /// <summary>
        /// Creates a new instance of the <see cref="ImportAPI"/> class.
        /// Uses the bearer token for the current <see cref="System.Security.Claims.ClaimsPrincipal"/> and should only
        /// be used by processes hosted by the Relativity Service Account (IE Agent).
        /// </summary>
        /// <param name="webServiceUrl">
        /// The URL to the Relativity WebAPI instance.
        /// </param>
        /// <returns>
        /// The <see cref="ImportAPI"/> instance.
        /// </returns>
        /// <exception cref="kCura.WinEDDS.Exceptions.CredentialsNotSupportedException">
        /// Thrown when integrated security is not supported when running within a service process.
        /// </exception>
        /// <exception cref="kCura.WinEDDS.Exceptions.InvalidLoginException">
        /// Thrown when the current claims principal does not have an access token or an authentication failure occurs.
        /// </exception>
        public static ImportAPI CreateByRsaBearerToken(string webServiceUrl)
        {
            string token = System.Security.Claims.ClaimsPrincipal.Current.Claims.AccessToken();
	        if (string.IsNullOrEmpty(token))
	        {
                throw new InvalidLoginException("The current claims principal does not have a bearer token.");
	        }

            return CreateByBearerToken(webServiceUrl, token);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="ImportAPI"/> class.
        /// Uses the supplied bearer token to authenticate both WebAPI and REST API endpoints.
        /// </summary>
        /// <param name="webServiceUrl">
        /// The URL to the Relativity WebAPI instance.
        /// </param>
        /// <param name="bearerToken">
        /// The bearer token used to authenticate both WebAPI and REST API endpoints.
        /// </param>
        /// <returns>
        /// The <see cref="ImportAPI"/> instance.
        /// </returns>
        /// <exception cref="kCura.WinEDDS.Exceptions.InvalidLoginException">
        /// Thrown when bearer token authentication failure occurs.
        /// </exception>
        public static ImportAPI CreateByBearerToken(string webServiceUrl, string bearerToken)
	    {
	        return new ImportAPI(kCura.WinEDDS.Credentials.Constants.OAuthWebApiBearerTokenUserName, bearerToken, webServiceUrl);
        }

        /// <summary>
        /// For internal use only. Specifies where the document is being imported from.
        /// </summary>
        public ExecutionSourceEnum ExecutionSource { get; set; }

        /// <summary>
        /// Returns a collection of all workspaces that are available for the logged in user.
        /// </summary>
        public IEnumerable<Workspace> Workspaces()
		{
			var cm = GetCaseManager();
			var wsds = cm.RetrieveAll();
			var dt = wsds.Tables[0];

			// to-do:  maybe add an enumeration for artifact type IDs
			// for now, 8 = Workspaces
			return dt.Rows.OfType<DataRow>().Select(row => new Workspace 
				{ ArtifactID = (int)row["ArtifactID"], DocumentPath = (String)row["DefaultFileLocationName"], DownloadHandlerURL = (String)row["DownloadHandlerApplicationPath"], 
					MatterArtifactID = (int)row["MatterArtifactID"], Name = (String)row["Name"], RootArtifactID = (int)row["RootArtifactID"], 
					RootFolderID = (int)row["RootFolderID"], StatusCodeArtifactID = (int)row["StatusCodeArtifactID"], ArtifactTypeId = 8, 
					ParentArtifactID = (int)row["ParentArtifactID"] }).ToList();
		}

		/// <summary>
		/// Returns all production sets eligible for import.
		/// </summary>
		/// <param name="workspaceArtifactID">The artifact ID of the workspace holding the production sets.</param>
		public IEnumerable<ProductionSet> GetProductionSets(int workspaceArtifactID)
		{
			var prodMan = GetProductionManager();
			var prodSets = prodMan.RetrieveImportEligibleByContextArtifactID(workspaceArtifactID).Tables[0];

			// ArtifactTypeId of 17 indicates a 'Production' object type
			return from object row in prodSets.Rows select row as DataRow into row1 where row1 != null && (int)row1["ArtifactID"] > 0 select new ProductionSet
				{
					ArtifactID = (int) row1["ArtifactID"], ArtifactTypeId = 17, Name = (String) row1["Name"], ParentArtifactID = workspaceArtifactID, ProductionOrder = (int) row1["ProductionOrder"]
				};
		}

        /// <summary>
        /// Returns all fields that apply to a given artifact type.
        /// </summary>
        /// <param name="workspaceArtifactID">
        /// The artifact ID of the workspace holding the fields and artifact type.
        /// </param>
        /// <param name="artifactTypeID">
        /// The ID of the artifact type with the applied fields.
        /// </param>
        /// <returns>
        /// The <see cref="Field"/> instances.
        /// </returns>
        /// <remarks>
        /// <list type="bullet">
        ///		<listheader>
        ///			<description>The returned collection excludes those fields with one of the following FieldCategories:</description>
        ///		</listheader>
        ///		<item>
        ///			<description>FieldCategory.AutoCreate</description>
        ///		</item>
        ///		<item>
        ///			<description>FieldCategory.Batch</description>
        ///		</item>
        ///		<item>
        ///			<description>FieldCategory.FileInfo</description>
        ///		</item>
        ///		<item>
        ///			<description>FieldCategory.FileSize</description>
        ///		</item>
        ///		<item>
        ///			<description>FieldCategory.MarkupSetMarker</description>
        ///		</item>
        ///		<item>
        ///			<description>FieldCategory.MultiReflected</description>
        ///		</item>
        ///		<item>
        ///			<description>FieldCategory.ProductionMarker</description>
        ///		</item>
        ///		<item>
        ///			<description>FieldCategory.Reflected</description>
        ///		</item>
        /// </list>
        /// 
        /// </remarks>
        public IEnumerable<Field> GetWorkspaceFields(int workspaceArtifactID, int artifactTypeID)
		{
			var fm = new WinEDDS.Service.FieldManager(_credentials, _cookieMonster);
			//This returned collection contains fields excluding those with one of the following FieldCategories:
			// FieldCategory.AutoCreate
			// FieldCategory.Batch
			// FieldCategory.FileInfo
			// FieldCategory.FileSize
			// FieldCategory.MarkupSetMarker
			// FieldCategory.MultiReflected
			// FieldCategory.ProductionMarker
			// FieldCategory.Reflected
			// See kCura.WinEDDS.Service.FieldQuery.RetrieveAllAsArray -Phil S. 10/19/2011
			var fields = fm.Query.RetrieveAllAsDocumentFieldCollection(workspaceArtifactID, artifactTypeID);

			return (from DocumentField docfield in fields
					  select new Field
							{
								ArtifactID = docfield.FieldID, 
								ArtifactTypeId = docfield.FieldTypeID, 
								Name = docfield.FieldName,
								FieldLength = docfield.FieldLength, 
								FieldTypeID = (FieldTypeEnum)Enum.ToObject(typeof(FieldTypeEnum),docfield.FieldTypeID), 
								AssociatedObjectTypeID = docfield.AssociatedObjectTypeID, 
								UseUnicode = docfield.UseUnicode, 
								FieldCategory = (FieldCategoryEnum)Enum.ToObject(typeof(FieldCategoryEnum), docfield.FieldCategoryID),
								Guids = docfield.Guids,
								EnableDataGrid = docfield.EnableDataGrid
							}).ToList();
		}

        /// <summary>
        /// Creates an ImageImportBulkArtifactJob with which to import a set of images.
        /// </summary>
        /// <returns>
        /// The <see cref="ImageImportBulkArtifactJob"/> instance.
        /// </returns>
        /// <remarks>
        /// Setting the user name and password property for the job is not required, because the credentials are pre-populated.
        /// </remarks>
        public ImageImportBulkArtifactJob NewImageImportJob()
		{
			return new ImageImportBulkArtifactJob(_credentials, _cookieMonster, (int)ExecutionSource);
		}

        /// <summary>
        /// Creates an ImageImportBulkArtifactJob with which to import a set of images into the given production set.
        /// </summary>
        /// <remarks>
        /// Setting the user name and password property for the job is not required, because the credentials are pre-populated.
        /// </remarks>
        /// <param name="productionArtifactID">
        /// Artifact ID of the production set to hold the imported images.
        /// </param>
        /// <returns>
        /// The <see cref="ImageImportBulkArtifactJob"/> instance with <see cref="kCura.Relativity.DataReaderClient.ImageSettings.ForProduction"/> set to true
        /// and <see cref="kCura.Relativity.DataReaderClient.ImageSettings.ProductionArtifactID"/> set to <paramref name="productionArtifactID"/>.
        /// </returns>
        public ImageImportBulkArtifactJob NewProductionImportJob(int productionArtifactID)
        {
            ImageImportBulkArtifactJob imgJob = this.NewImageImportJob();
			imgJob.Settings.ForProduction = true;
			imgJob.Settings.ProductionArtifactID = productionArtifactID;
			return imgJob;
		}

        /// <summary>
        /// Creates an ImportBulkArtifactJob to be used to import a set of native documents.
        /// </summary>
        /// <returns>
        /// The <see cref="ImportBulkArtifactJob"/> instance with <see cref="kCura.Relativity.DataReaderClient.Settings.ArtifactTypeId"/> set to Document.
        /// </returns>
        /// <remarks>
        /// Setting the user name and password property for the job is not required, because the credentials are pre-populated.
        /// </remarks>
        public ImportBulkArtifactJob NewNativeDocumentImportJob()
        {
            return NewObjectImportJob(10);
        }

        /// <summary>
        /// Creates an ImportBulkArtifactJob with which to import a set of artifacts of the given type.
        /// </summary>
        /// <param name="artifactTypeId">
        /// The artifact type ID of the objects to be imported.
        /// </param>
        /// <returns>
        /// The <see cref="ImportBulkArtifactJob"/> instance with <see cref="kCura.Relativity.DataReaderClient.Settings.ArtifactTypeId"/> set to <paramref name="artifactTypeId"/>.
        /// </returns>
        public ImportBulkArtifactJob NewObjectImportJob(int artifactTypeId)
		{
			var returnJob = new ImportBulkArtifactJob(_credentials, _tapiCredentials, _cookieMonster, (int)ExecutionSource);
			returnJob.Settings.ArtifactTypeId = artifactTypeId;
			return returnJob;
		}

		/// <summary>
		/// Returns the UploadMode that will be used to upload files to
		/// the workspace specified by <paramref name="caseArtifactID"/>.
		/// </summary>
		/// <param name="caseArtifactID">
		/// The artifact ID of the destination workspace.
		/// </param>
		/// <returns>
		/// The <see cref="UploadTypeEnum"/> value.
		/// </returns>
		public UploadTypeEnum GetFileUploadMode(int caseArtifactID)
		{
			var cm = GetCaseManager();
			var caseInfo = cm.Read(caseArtifactID);

			return GetFileUploadMode(caseArtifactID, caseInfo.DocumentPath);
		}

        /// <summary>
        /// Returns all uploadable artifact types associated with a given case.
        /// </summary>
        /// <param name="caseArtifactID">
        /// The artifact ID of the case containing the artifact types.
        /// </param>
        /// <returns>
        /// The <see cref="Data.ArtifactType"/> instances.
        /// </returns>
        public IEnumerable<Data.ArtifactType> GetUploadableArtifactTypes(int caseArtifactID)
		{
			var om = GetObjectTypeManager();
			var ds = om.RetrieveAllUploadable(caseArtifactID);
			var dt = ds.Tables[0];
			var dr = dt.Rows;

			return (from DataRow singleRow in dr
					select new Data.ArtifactType
					{
						ID = (int)singleRow["DescriptorArtifactTypeID"],Name = (string)singleRow["Name"]
					}).ToList();
		}

        #region "Private items"

	    private void PerformLogin(string userName, string password, string webServiceURL)
	    {
	        ImportCredentialManager.SessionCredentials creds;

	        try
	        {
	            ImportCredentialManager.WebServiceURL = webServiceURL;
	            creds = ImportCredentialManager.GetCredentials(userName, password);
	        }
	        catch (kCura.WinEDDS.Exceptions.CredentialsNotSupportedException)
	        {
	            throw;
	        }
            catch (kCura.WinEDDS.Exceptions.InvalidLoginException)
	        {
	            throw;
	        }
	        catch (Exception e)
	        {
	            throw new kCura.WinEDDS.Exceptions.InvalidLoginException("Login failed.", e);
	        }

	        _credentials = creds.Credentials;
	        _tapiCredentials = creds.TapiCredential;
	        _cookieMonster = creds.CookieMonster;
	        if (_credentials == null)
	        {
	            throw new kCura.WinEDDS.Exceptions.InvalidLoginException("Login failed.");
	        }
	    }

        /// <summary>
        /// Create a repository path based on the given workspace ArtifactID and
        /// the default path for that workspace. This code is a combination of
        /// kCura.WinEDDS.BulkLoadFileImporter:New()  and
        /// kCura.WinEDDS.BulkImageFileImporter:New()
        /// -Phil S. 10/21/2011
        /// </summary>
        /// <param name="caseArtifactID">
        /// The artifact ID of the workspace.
        /// </param>
        /// <param name="defaultCasePath">
        /// The default repository path associated with the workspace.
        /// </param>
        /// <returns></returns>
        private string CreateRepositoryPath(int caseArtifactID, string defaultCasePath)
		{
			var repoSuffix = "\\EDDS" + caseArtifactID + "\\";
			var returnRepoPath = defaultCasePath.TrimEnd('\\') + repoSuffix;

			return returnRepoPath;
		}

		private CaseManager GetCaseManager()
		{
			if (_caseManager == null)
			{
				_caseManager = new CaseManager(_credentials, _cookieMonster);
			}

			return _caseManager;
		}

		private ObjectTypeManager GetObjectTypeManager()
		{
			if (_objectTypeManager == null)
			{
				_objectTypeManager = new ObjectTypeManager(_credentials, _cookieMonster);
			}

			return _objectTypeManager;
		}

		private ProductionManager GetProductionManager()
		{
			if (_productionManager == null)
			{
				_productionManager = new ProductionManager(_credentials, _cookieMonster);
			}

			return _productionManager;
		}

        /// <summary>
        /// This method determines the upload mode used for the workspace specified
        /// by <paramref name="caseArtifactID"/>. This code is taken from kCura.WinEDDS.FileUploader:SetType()
        /// -Phil S. 10/21/2011
        /// </summary>
        /// <param name="caseArtifactID">
        /// The artifact ID of the workspace to perform the check.
        /// </param>
        /// <param name="defaultCasePath">
        /// The default repository path associated with the workspace.
        /// </param>
        /// <returns>
        /// The <see cref="UploadTypeEnum"/> value.
        /// </returns>
        private UploadTypeEnum GetFileUploadMode(int caseArtifactID, string defaultCasePath)
		{
            // TODO: Replace this with TAPI's auto-discovery API and update UploadTypeEnum.
            UploadTypeEnum returnUploadType;
			var destFolderPath = CreateRepositoryPath(caseArtifactID, defaultCasePath);

			try
			{
				var dummyText = System.Guid.NewGuid().ToString().Replace("-", String.Empty).Substring(0, 5);
				//If the destination folder path is empty, we only need to test file Read/Write permissions
				if (!String.IsNullOrEmpty(destFolderPath))
				{
					if (!System.IO.Directory.Exists(destFolderPath))
					{
						System.IO.Directory.CreateDirectory(destFolderPath);
					}
				}

				System.IO.File.Create(destFolderPath + dummyText).Close();
				System.IO.File.Delete(destFolderPath + dummyText);
				returnUploadType = UploadTypeEnum.Direct;
			}
			catch (Exception)
			{
				returnUploadType = UploadTypeEnum.Web;
			}

			return returnUploadType;
		}

#endregion

	}
}
