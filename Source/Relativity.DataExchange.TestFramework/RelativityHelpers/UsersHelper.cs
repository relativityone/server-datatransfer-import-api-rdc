// ----------------------------------------------------------------------------
// <copyright file="UsersHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Net.Http;
	using System.Text;
	using System.Threading.Tasks;

	using Newtonsoft.Json.Linq;

	using Relativity.Services.Security;
	using Relativity.Services.Security.Models;
	using Relativity.Services.User;

	using User = Relativity.Services.User.User;

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
		public static async Task SwitchIntegratedAuthenticationForCurrentUserAsync(IntegrationTestParameters parameters, bool isEnabled)
		{
			int currentUserId;
			using (var userManager = ServiceHelper.GetServiceProxy<IUserManager>(parameters))
			{
				User currentUser = await userManager.RetrieveCurrentAsync(parameters.WorkspaceId).ConfigureAwait(false);
				currentUserId = currentUser.ArtifactID;
			}

			using (var loginProfileManager = ServiceHelper.GetServiceProxy<ILoginProfileManager>(parameters))
			{
				LoginProfile loginProfile = await loginProfileManager.GetLoginProfileAsync(currentUserId).ConfigureAwait(false);

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

		public static async Task<int> EnsureUserAsync(IntegrationTestParameters parameters, string firstName, string lastName, string password, IEnumerable<int> groupArtifactIds)
		{
			UserRef[] users = await GetUserListAsync(parameters).ConfigureAwait(false);
			string name = $"{lastName}, {firstName}";
			UserRef user = users.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

			if (user != null)
			{
				return user.ArtifactID;
			}

			return await CreateNewUserAsync(parameters, firstName, lastName, password, groupArtifactIds).ConfigureAwait(false);
		}

		public static async Task<UserRef[]> GetUserListAsync(IntegrationTestParameters parameters)
		{
			using (IUserManager userManager = ServiceHelper.GetServiceProxy<IUserManager>(parameters))
			{
				WorkspaceUserData user = await userManager.RetrieveAllActiveAsync(parameters.WorkspaceId).ConfigureAwait(false);
				return user.ActiveUsers;
			}
		}

		public static async Task<int> CreateNewUserAsync(IntegrationTestParameters parameters, string firstName, string lastName, string password, IEnumerable<int> groupArtifactIds)
		{
			string createInputJson = ResourceFileHelper.GetResourceFolderPath("CreateInput.json");
			JObject request = JObject.Parse(File.ReadAllText(createInputJson));
			request["First Name"] = firstName;
			request["Last Name"] = lastName;
			request["Email Address"] = $"{firstName}.{lastName}@relativity.com";
			request["Password"] = password;

			var groups = (JArray)request["Groups"];
			groups.Clear();

			foreach (int groupArtifactId in groupArtifactIds)
			{
				JObject group = new JObject();
				group.Add("Artifact ID", groupArtifactId);
				groups.Add(group);
			}

			string absoluteUri = parameters.RelativityRestUrl.AbsoluteUri;
			var uri = new Uri(absoluteUri.Substring(0, absoluteUri.IndexOf("/api", StringComparison.OrdinalIgnoreCase)));

			using (var httpClient = new HttpClient())
			{
				httpClient.BaseAddress = uri;

				httpClient.DefaultRequestHeaders.Add("X-CSRF-Header", "-");

				string basicAuth = $"{parameters.RelativityUserName}:{parameters.RelativityPassword}";
				httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {Convert.ToBase64String(Encoding.ASCII.GetBytes(basicAuth))}");

				string url = $"{uri.AbsolutePath}/Relativity/User";

				StringContent content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
				HttpResponseMessage response = await httpClient.PostAsync(url, content).ConfigureAwait(false);
				string result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

				if (response.StatusCode != HttpStatusCode.Created)
				{
					throw new HttpServiceException($"{nameof(CreateNewUserAsync)} failed." + Environment.NewLine +
							  new
							  {
								  request,
								  response,
								  url,
								  basicAuth,
								  httpClient.DefaultRequestHeaders,
							  });
				}

				JObject resultObject = JObject.Parse(result);
				return (int)resultObject["Results"][0]["ArtifactID"];
			}
		}

		public static async Task RemoveUserAsync(IntegrationTestParameters parameters, int userId)
		{
			using (var userManager =
				ServiceHelper.GetServiceProxy<Services.Interfaces.UserInfo.IUserInfoManager>(parameters))
			{
				await userManager.DeleteAsync(userId).ConfigureAwait(false);
			}
		}
	}
}
