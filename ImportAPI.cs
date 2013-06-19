using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using kCura.Relativity.ImportAPI.Data;
using kCura.Relativity.ImportAPI.Enumeration;
using kCura.WinEDDS;
using kCura.WinEDDS.Service;
using kCura.Relativity.DataReaderClient;

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
		private String _userName;
		private String _password;
		private CaseManager _caseManager;
		/// <summary>
		/// Holds cookies for the current session.
		/// </summary>
		protected CookieContainer _cookieMonster;
		/// <summary>
		/// Holds credentials for the logged-in user.
		/// </summary>
		protected ICredentials _credentials;
		private ObjectTypeManager _objectTypeManager;
		private ProductionManager _productionManager;

		/// <summary>
		/// Creates an instance of ImportAPI.
		/// </summary>
		/// <remarks>
		/// User name and password are required (unless using Windows Authentication) and will be validated.
		/// The ImportAPI tries to resolve the server name by reading the WebServiceURL key from the local app.config file.  If this fails, it checks the Windows Registry for the location set by the Relativity Desktop Client.
		/// </remarks>
		/// <param name="UserName">User name for the account you're logging in with.</param>
		/// <param name="Password">Password associated with the user name.</param>
		public ImportAPI(String UserName, String Password)
		{
			PerformLogin(UserName, Password, string.Empty );
		}


		private void PerformLogin(string UserName, string Password, string WebServiceURL)
		{
			ImportCredentialManager.SessionCredentials creds;

			try
			{
				ImportCredentialManager.WebServiceURL = WebServiceURL;
				creds = ImportCredentialManager.GetCredentials(UserName, Password);
			}
			catch (Exception ex)
			{
				var newex = new Exception("Login failed.", ex);
				throw newex;
			}

			_credentials = creds.Credentials;
			_cookieMonster = creds.CookieMonster;

			if (_credentials == null)
			{
				throw new Exception("Login failed.");
			}
		}


		/// <summary>
		/// Creates an instance of ImportAPI.
		/// </summary>
		/// <remarks>
		/// User name and password are required (unless using Windows Authentication) and will be validated
		/// against the Relativity WebAPI instance located at <paramref name="WebServiceURL"/>.
		/// </remarks>
		/// <param name="UserName">User name for the account you're logging in with.</param>
		/// <param name="Password">Password for the user name.</param>
		/// <param name="WebServiceURL">Location of the Relativity WebAPI instance.</param>
		public ImportAPI(String UserName, String Password, String WebServiceURL)
		{
			PerformLogin(UserName, Password, WebServiceURL );
		}

		/// <summary>
		/// Creates an instance of ImportAPI with WinAuth.
		/// </summary>
		/// <remarks>
		/// The user will be validated against the Relativity WebAPI instance located at <paramref name="WebServiceURL"/>.
		/// </remarks>
		/// <param name="WebServiceURL">Location of the Relativity WebAPI instance.</param>
		public ImportAPI(String WebServiceURL)
		{
			this.PerformLogin(null, null, WebServiceURL );
		}

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
		/// <param name="workspaceArtifactID">The artifact ID of the workspace holding the fields and artifact type.</param>
		/// <param name="artifactTypeID">The ID of the artiface type with the applied fields.</param>
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
								Guids = docfield.Guids
							}).ToList();
		}

		/// <summary>
		/// Creates an ImageImportBulkArtifactJob with which to import a set of images.
		/// </summary>
		/// <returns>
		/// Returns a new instance of an ImageImportBulkArtifactJob.
		/// </returns>
		/// <remarks>
		/// Setting the user name and password property for the job is not required, because the credentials are pre-populated.
		/// </remarks>
		public ImageImportBulkArtifactJob NewImageImportJob()
		{
			return new ImageImportBulkArtifactJob(_credentials, _cookieMonster, _userName, _password);
		}

		/// <summary>
		/// Creates an ImageImportBulkArtifactJob with which to import a set of images into the given production set.
		/// </summary>
		/// <remarks>
		/// Setting the user name and password property for the job is not required, because the credentials are pre-populated.
		/// </remarks>
		/// <param name="productionArtifactID">Artifact ID of the production set to hold the imported images.</param>
		/// <returns>Returns a new instance of an ImageImportBulkArtifactJob. It is identical to one returned
		/// from NewImageImportJob(), with two changes: Settings.ForProduction is set to true,
		/// and Settings.ProductionArtifactID = <paramref name="productionArtifactID"/>.
		/// </returns>
		public ImageImportBulkArtifactJob NewProductionImportJob(int productionArtifactID)
		{
			var imgJob = NewImageImportJob();
			imgJob.Settings.ForProduction = true;
			imgJob.Settings.ProductionArtifactID = productionArtifactID;

			return imgJob;
		}

		/// <summary>
		/// Creates an ImportBulkArtifactJob to be used to import a set of native documents.
		/// </summary>
		/// <returns>
		/// Returns a new instance of an ImportBulkArtifactJob with Settings.ArtifactTypeId set to Document.
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
		/// <returns>
		/// Returns a new instance of an ImportBulkArtifactJob with the Settings.ArtifactTypeId property set to <paramref name="artifactTypeId"/>.
		/// </returns>
		/// <param name="artifactTypeId">The artifact type ID of the objects to be imported.</param>
		public ImportBulkArtifactJob NewObjectImportJob(int artifactTypeId) {
			var returnJob = new ImportBulkArtifactJob(_credentials, _cookieMonster, _userName, _password);

			returnJob.Settings.ArtifactTypeId = artifactTypeId;

			return returnJob;
		}

		/// <summary>
		/// Returns the UploadMode that will be used to upload files to
		/// the workspace specified by <paramref name="caseArtifactID"/>.
		/// </summary>
		/// <param name="caseArtifactID">The artifact ID of the destination workspace.</param>
		public UploadTypeEnum GetFileUploadMode(int caseArtifactID)
		{
			var cm = GetCaseManager();
			var caseInfo = cm.Read(caseArtifactID);

			return GetFileUploadMode(caseArtifactID, caseInfo.DocumentPath);
		}

		/// <summary>
		/// Returns all uploadable artifact types associated with a given case.
		/// </summary>
		/// <param name="caseArtifactID">The artifact ID of the case containing the artifact types.</param>
		public IEnumerable<ArtifactType> GetUploadableArtifactTypes(int caseArtifactID)
		{
			var om = GetObjectTypeManager();
			var ds = om.RetrieveAllUploadable(caseArtifactID);
			var dt = ds.Tables[0];
			var dr = dt.Rows;

			return (from DataRow singleRow in dr
					select new ArtifactType
					{
						ID = (int)singleRow["DescriptorArtifactTypeID"],Name = (string)singleRow["Name"]
					}).ToList();
		}

		#region "Private items"

		/// <summary>
		/// Create a repository path based on the given workspace ArtifactID and
		/// the default path for that workspace. This code is a combination of
		/// kCura.WinEDDS.BulkLoadFileImporter:New()  and
		/// kCura.WinEDDS.BulkImageFileImporter:New()
		/// -Phil S. 10/21/2011
		/// </summary>
		/// <param name="caseArtifactID"></param>
		/// <param name="defaultCasePath"></param>
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

		private ICredentials GetCredentials()
		{
			ICredentials credentials = null;

			credentials = kCura.WinEDDS.Api.LoginHelper.LoginWindowsAuth(_cookieMonster);
			if (credentials == null)
			{
				credentials = kCura.WinEDDS.Api.LoginHelper.LoginUsernamePassword(_userName, _password, _cookieMonster);
			}

			return credentials;
		}

		/// <summary>
		/// This method determines the upload mode used for the workspace specified
		/// by <paramref name="caseArtifactID"/>. This code is taken from kCura.WinEDDS.FileUploader:SetType()
		/// -Phil S. 10/21/2011
		/// </summary>
		/// <param name="caseArtifactID"></param>
		/// <param name="defaultCasePath"></param>
		/// <returns></returns>
		private UploadTypeEnum GetFileUploadMode(int caseArtifactID, string defaultCasePath)
		{
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
