using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
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
		private CookieContainer _cookieMonster;
		private ICredentials _credentials;

		/// <summary>
		/// Create an instance of ImportAPI.  Username and Password are required (unless using windows auth), and will be validated.
		/// </summary>
		/// <param name="UserName">UserName to log in</param>
		/// <param name="Password">Password for the user</param>
		public ImportAPI(String UserName, String Password)
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
		/// Workspaces - returns a collection of available workspaces for the logged in user
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Workspace> Workspaces()
		{

			var cm = new CaseManager(_credentials, _cookieMonster);
			var wsds = cm.RetrieveAll();
			var dt = wsds.Tables[0];

			return dt.Rows.OfType<DataRow>().Select(row => new Workspace 
				{ ArtifactID = (int)row["ArtifactID"], DocumentPath = (String)row["DefaultFileLocationName"], DownloadHandlerURL = (String)row["DownloadHandlerApplicationPath"], 
					MatterArtifactID = (int)row["MatterArtifactID"], Name = (String)row["Name"], RootArtifactID = (int)row["RootArtifactID"], 
					RootFolderID = (int)row["RootFolderID"], StatusCodeArtifactID = (int)row["StatusCodeArtifactID"] }).ToList();
		}

		/// <summary>
		/// Given a workspace artifact ID, and an artifact type ID, the fields that apply to that artifact type will be returned.
		/// </summary>
		/// <param name="WorkspaceArtifactID"></param>
		/// <param name="ArtifactTypeID"></param>
		/// <returns></returns>
		public DocumentFieldCollection GetWorkspaceFields(int WorkspaceArtifactID, int ArtifactTypeID)
		{
			var fm = new WinEDDS.Service.FieldManager(_credentials, _cookieMonster);
			return fm.Query.RetrieveAllAsDocumentFieldCollection(WorkspaceArtifactID, ArtifactTypeID);

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
		/// This method will return a new instance of an ImportBulkArtifactJob.
		/// The returned object can be used to import a set of native documents.
		/// Setting the username and password property for the job will not be required, as the credentials will be pre-populated using this method.
		/// </summary>
		/// <returns></returns>
		public ImportBulkArtifactJob NewNativeDocumentImportJob()
		{
			return new ImportBulkArtifactJob(_userName, _password);						
		}






#region "Private items"

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

#endregion

	}
}
