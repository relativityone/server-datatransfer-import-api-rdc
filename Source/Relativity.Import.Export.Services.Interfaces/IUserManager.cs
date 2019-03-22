using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relativity.Import.Export.Services
{
	public interface IUserManager
	{
		/// <summary>
		/// Updates the image viewer default view mode.
		/// </summary>
		/// <param name="defaultViewMode">default view mode</param>
		void UpdateImageViewerDefaultViewMode(string defaultViewMode);

		/// <summary>
		/// Updates the OIX document mode.
		/// </summary>
		/// <param name="mode">mode</param>
		void UpdateOIXDocumentMode(int mode);

		/// <summary>
		/// Updates the selected markup set.
		/// </summary>
		/// <param name="selectedMarkupSetArtifactID">Markup Set ArtifactID</param>
		void UpdateSelectedMarkupSetArtifactID(int selectedMarkupSetArtifactID);

		/// <summary>
		/// Updates the default redaction text.
		/// </summary>
		/// <param name="text"></param>
		void UpdateDefaultRedactionText(string text);

		/// <summary>
		/// Logs out the user.
		/// </summary>
		void Logout();

		/// <summary>
		/// Clears cookies and creates authentication ticket cookie for Login web method. Must be called before Login.
		/// </summary>
		void ClearCookiesBeforeLogin();

	    /// <summary>
		/// Checks that the user is currently logged in.
		/// </summary>
		/// <returns>True if the user is logged in.  Throws NeedToReLoginException if not.</returns>
		/// <remarks>Checks that the user is currently logged in. Throws NeedToReLoginException if not.</remarks>
		bool LoggedIn();

		/// <summary>
		/// Logs in the user
		/// </summary>
		/// <param name="emailAddress">email of the user</param>
		/// <param name="password">password of the user</param>
		/// <returns></returns>
		/// <remarks>For Login with Windows Authentication, see RelativityManager.ValidateSuccessfulLogin</remarks>
		bool Login(string emailAddress, string password);

		/// <summary>
		/// Logs in the user.
		/// </summary>
		/// <param name="authenticationToken">authentication token used to login</param>
		/// <returns></returns>
		string LoginWithAuthenticationToken(string authenticationToken);

		/// <summary>
		/// Generates a token for the logged in user.
		/// </summary>
		/// <returns>generated token</returns>
		string GenerateAuthenticationToken();

		/// <summary>
		/// Generates a token for the logged in user.
		/// </summary>
		/// <returns></returns>
		string GenerateDistributedAuthenticationToken();

		/// <summary>
		/// Gets the latest authentication token
		/// </summary>
		/// <returns></returns>
		string GetLatestAuthenticationToken();

		/// <summary>
		/// Retrieves a list of all usernames and email addresses assigned to workspace.
		/// </summary>
		/// <param name="caseContextArtifactID">ArtifactID of the workspace</param>
		/// <returns></returns>
		System.Data.DataSet RetrieveAllAssignableInCase(int caseContextArtifactID);
	}
}
