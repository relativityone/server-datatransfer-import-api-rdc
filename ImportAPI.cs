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
	/// The new face of the ImportAPI
	/// </summary>
	public  class ImportAPI
	{

		private String _userName;
		private String _password;
		private CaseManager _caseManager;
		private CookieContainer _cookieMonster;
		private ICredentials _credentials;
		private ObjectTypeManager _objectTypeManager;
		private ProductionManager _productionManager;

		public CookieContainer CookieCache  { get { return _cookieMonster; } }
		public ICredentials Credentials { get { return _credentials; } }

		/// <summary>
		/// Create an instance of ImportAPI.  Username and Password are required (unless using windows auth), and will be validated.
		/// </summary>
		/// <param name="UserName">UserName to log in</param>
		/// <param name="Password">Password for the user</param>
		public ImportAPI(String UserName, String Password)
		{
			PerformLogin(UserName, Password);
		}

		private void PerformLogin(string UserName, string Password)
		{
			_userName = UserName;
			_password = Password;
			_cookieMonster = new CookieContainer();

			try
			{
				_credentials = GetCredentials();
			}
			catch (Exception ex)
			{
				var newex = new Exception("Login failed.", ex);
				throw newex;
			}

			if (_credentials == null)
			{
				throw new Exception("Login failed.");
			}
		}

		/// <summary>
		/// Create an instance of ImportAPI.  Username and Password are required (unless using windows auth), and will be validated
		/// against the Relativity WebAPI instance located at <paramref name="WebServiceURL"/>.
		/// </summary>
		/// <param name="UserName">UserName to log in</param>
		/// <param name="Password">Password for the user</param>
		/// <param name="WebServiceURL"></param>
		public ImportAPI(String UserName, String Password, String WebServiceURL)
		{
			Config.ProgrammaticServiceURL = WebServiceURL;
			PerformLogin(UserName, Password);
		}

		/// <summary>
		/// Create an instance of ImportAPI. This is for the use of WinAuth and the user will be validated
		/// against the Relativity WebAPI instance located at <paramref name="WebServiceURL"/>.
		/// </summary>
		/// <param name="WebServiceURL"></param>
		public ImportAPI(String WebServiceURL)
		{
			Config.ProgrammaticServiceURL = WebServiceURL;
			this.PerformLogin("A","B");
		}

		/// <summary>
		/// Workspaces - returns a collection of available workspaces for the logged in user
		/// </summary>
		/// <returns></returns>
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
		/// Given a workspace artifact ID, return all production sets eligble for import
		/// </summary>
		/// <param name="workspaceArtifactID"></param>
		/// <returns></returns>
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
		/// Given a workspace artifact ID, and an artifact type ID, the fields that apply to that artifact type will be returned.
		/// </summary>
		/// <param name="workspaceArtifactID"></param>
		/// <param name="artifactTypeID"></param>
		/// <returns></returns>
		public IEnumerable<Field> GetWorkspaceFields(int workspaceArtifactID, int artifactTypeID)
		{
			var fm = new WinEDDS.Service.FieldManager(_credentials, _cookieMonster);
			//This returned collection contains fields excluding those with one of the following FieldCategory's
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
								ArtifactID = docfield.FieldID, ArtifactTypeId = docfield.FieldTypeID, Name = docfield.FieldName, FieldLength = docfield.FieldLength, FieldTypeID = (FieldTypeEnum)Enum.ToObject(typeof(FieldTypeEnum),
									docfield.FieldTypeID), AssociatedObjectTypeID = docfield.AssociatedObjectTypeID, UseUnicode = docfield.UseUnicode, FieldCategory = (FieldCategoryEnum)Enum.ToObject(typeof(FieldCategoryEnum), docfield.FieldCategoryID)
							}).ToList();
		}


		/// <summary>
		/// This method will return a new instance of an ImageImportBulkArtifactJob.
		/// The returned object can be used to import a set of images.
		/// Setting the username and password property for the job will not be required, as the credentials will be pre-populated using this method.
		/// </summary>
		/// <returns></returns>
		public ImageImportBulkArtifactJob NewImageImportJob()
		{
			return new ImageImportBulkArtifactJob(_credentials, _cookieMonster, _userName, _password);
		}

		/// <summary>
		/// This method will return a new instance of an ImageImportBulkArtifactJob.
		/// The returned object can be used to import a set of production images.
		/// Setting the username and password property for the job will not be required, as the credentials will be pre-populated using this method.
		/// </summary>
		/// <param name="productionArtifactID">Artifact ID of the production set to import to.</param>
		/// <returns>The returned ImageImportBulkArtifactJob object is identical to one returned
		/// from NewImageImportJob(), with 2 changes. The Settings.ForProduction is already set to true,
		/// along with the Settings.ProductionArtifactID = <paramref name="productionArtifactID"/>.
		/// </returns>
		public ImageImportBulkArtifactJob NewProductionImportJob(int productionArtifactID)
		{
			var imgJob = NewImageImportJob();
			imgJob.Settings.ForProduction = true;
			imgJob.Settings.ProductionArtifactID = productionArtifactID;

			return imgJob;
		}

		/// <summary>
		/// This method will return a new instance of an ImportBulkArtifactJob.
		/// The returned object can be used to import a set of native documents.
		/// Setting the username and password property for the job will not be required, as the credentials will be pre-populated using this method.
		/// </summary>
		/// <returns></returns>
		public ImportBulkArtifactJob NewNativeDocumentImportJob()
		{
			return NewNativeDocumentImportJob(null);
		}

		public ImportBulkArtifactJob NewNativeDocumentImportJob(string token)
		{
			var importJob = new ImportBulkArtifactJob(_userName, _password);
			int? userId = null;

			if (!string.IsNullOrEmpty(token))
			{
				var auditManager = new kCura.EDDS.WebAPI.AuditManagerBase.AuditManager();
				auditManager.CookieContainer = _cookieMonster;
				auditManager.Credentials = _credentials;
				auditManager.Url = System.IO.Path.Combine(Config.ProgrammaticServiceURL, "AuditManager.asmx");
				userId = auditManager.GetUserIdByGuid(token);
			}

			importJob.Settings.OnBehalfOfUserMasterId = userId;
			return importJob;
		}

		/// <summary>
		/// This method returns the UploadMode that will be used to upload files to
		/// the workspace specified by <paramref name="caseArtifactID"/>.
		/// </summary>
		/// <param name="caseArtifactID">The Artifact ID of the destination workspace.</param>
		/// <returns></returns>
		public UploadTypeEnum GetFileUploadMode(int caseArtifactID)
		{
			var cm = GetCaseManager();
			var caseInfo = cm.Read(caseArtifactID);

			return GetFileUploadMode(caseArtifactID, caseInfo.DocumentPath);
		}

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
