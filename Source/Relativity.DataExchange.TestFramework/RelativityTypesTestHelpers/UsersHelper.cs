// ----------------------------------------------------------------------------
// <copyright file="UsersHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityTypesTestHelpers
{
	using System.Threading.Tasks;

	using Relativity.Services.Security;
	using Relativity.Services.Security.Models;
	using Relativity.Services.User;

	/// <summary>
	/// Defines static helper methods to manage users in tests.
	/// </summary>
	public static class UsersHelper
	{
		/// <summary>
		/// Adds or removes integrated authentication provider for a current user.
		/// </summary>
		/// <param name="parameters">Test context parameters.</param>
		/// <param name="isEnabled">Determines if integrated authentication should be enabled for current user.</param>
		/// <returns><see cref="Task"/> which completes when setting was changed.</returns>
		public static async Task SwitchIntegratedAuthenticationForCurrentUser(IntegrationTestParameters parameters, bool isEnabled)
		{
			int currentUserID;
			using (var userManager = ServiceHelper.GetServiceProxy<IUserManager>(parameters))
			{
				User currentUser = await userManager.RetrieveCurrentAsync(parameters.WorkspaceId).ConfigureAwait(false);
				currentUserID = currentUser.ArtifactID;
			}

			using (var loginProfileManager = ServiceHelper.GetServiceProxy<ILoginProfileManager>(parameters))
			{
				LoginProfile loginProfile = await loginProfileManager.GetLoginProfileAsync(currentUserID).ConfigureAwait(false);

				IntegratedAuthenticationMethod integratedAuthentication = null;
				if (isEnabled)
				{
					string userNameWithDomain = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
					integratedAuthentication = new IntegratedAuthenticationMethod
					{
						Account = userNameWithDomain,
						IsEnabled = true,
					};
				}

				loginProfile.IntegratedAuthentication = integratedAuthentication;
				await loginProfileManager.SaveLoginProfileAsync(loginProfile).ConfigureAwait(false);
			}
		}
	}
}
