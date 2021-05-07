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
	using System.Threading.Tasks;

	using Newtonsoft.Json.Linq;

	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Interfaces.Group;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Interfaces.UserInfo.Models;
	using Relativity.Services.Security;
	using Relativity.Services.Security.Models;

	/// <summary>
	/// Defines static helper methods to manage users in tests.
	/// </summary>
	public static class UsersHelper
	{
		public static Task<int> CreateNewUserAsync(
			IntegrationTestParameters parameters,
			string username,
			string password,
			IEnumerable<int> groupArtifactIds)
		{
			return RelativityVersionChecker.VersionIsLowerThan(parameters, RelativityVersion.PrairieSmoke)
					   ? CreateNewUserUsingHttpClientAsync(parameters, username, password, groupArtifactIds)
					   : CreateNewUserUsingKeplerAsync(parameters, username, password, groupArtifactIds);
		}

		public static Task RemoveUserAsync(IntegrationTestParameters parameters, int userId)
		{
			return RelativityVersionChecker.VersionIsLowerThan(parameters, RelativityVersion.Lanceleaf)
					   ? RemoveUserUsingHttpClientAsync(parameters, userId)
					   : RemoveUserUsingKeplerAsync(parameters, userId);
		}

		public static async Task<string> GetUserInfo(IntegrationTestParameters parameters, int userId)
		{
			string url =
				$"{parameters.RelativityRestUrl.AbsoluteUri}/Relativity.REST/Relativity/User/{userId}";

			return await HttpClientHelper.GetAsync(parameters, new Uri(url)).ConfigureAwait(false);
		}

		private static async Task<int> CreateNewUserUsingHttpClientAsync(
			IntegrationTestParameters parameters,
			string username,
			string password,
			IEnumerable<int> groupArtifactIds)
		{
			string createInputJson = ResourceFileHelper.GetResourceFolderPath("CreateInput.json");
			JObject request = JObject.Parse(File.ReadAllText(createInputJson));
			request["Email Address"] = username;
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
			absoluteUri = absoluteUri.Substring(0, absoluteUri.IndexOf("/api", StringComparison.OrdinalIgnoreCase));
			absoluteUri = $"{absoluteUri}/Relativity/User";

			bool adminsCanSetPasswordsInstanceSettingWasChanged =
				await ChangeAdminsCanSetPasswordsInstanceSettingAsync(parameters, true).ConfigureAwait(false);
			string result = await HttpClientHelper.PostAsync(parameters, new Uri(absoluteUri), request.ToString()).ConfigureAwait(false);
			if (adminsCanSetPasswordsInstanceSettingWasChanged)
			{
				await ChangeAdminsCanSetPasswordsInstanceSettingAsync(parameters, false).ConfigureAwait(false);
			}

			JObject resultObject = JObject.Parse(result);

			return (int)resultObject["Results"][0]["ArtifactID"];
		}

		private static async Task<int> CreateNewUserUsingKeplerAsync(
			IntegrationTestParameters parameters,
			string emailAddress,
			string password,
			IEnumerable<int> groupArtifactIds)
		{
			int userArtifactId;
			using (var userManager = ServiceHelper.GetServiceProxy<Services.Interfaces.UserInfo.IUserInfoManager>(parameters))
			{
				const int AdminUserId = 9;
				UserResponse adminUserDetails = await userManager.ReadAsync(AdminUserId).ConfigureAwait(false);

				var userRequest = new UserRequest(adminUserDetails)
				{
					FirstName = "Test User 001",
					LastName = "REST",
					EmailAddress = emailAddress,
				};
				var response = await userManager.CreateAsync(userRequest).ConfigureAwait(false);
				userArtifactId = response.ArtifactID;
			}

			var userIdentifier = new ObjectIdentifier { ArtifactID = userArtifactId };
			using (var groupManager = ServiceHelper.GetServiceProxy<IGroupManager>(parameters))
			{
				const int EveryoneGroupId = 1015005;
				foreach (int groupArtifactId in groupArtifactIds.Where(x => x != EveryoneGroupId))
				{
					await groupManager.AddMembersAsync(groupArtifactId, userIdentifier).ConfigureAwait(false);
				}
			}

			bool adminsCanSetPasswordsInstanceSettingWasChanged = await ChangeAdminsCanSetPasswordsInstanceSettingAsync(parameters, true).ConfigureAwait(false);
			using (var loginManager = ServiceHelper.GetServiceProxy<ILoginProfileManager>(parameters))
			{
				var loginProfile = await loginManager.GetLoginProfileAsync(userArtifactId).ConfigureAwait(false);
				loginProfile.Password = new PasswordMethod()
				{
					Email = emailAddress,
					IsEnabled = true,
					MustResetPasswordOnNextLogin = false,
					PasswordExpirationInDays = 7,
				};
				await loginManager.SaveLoginProfileAsync(loginProfile).ConfigureAwait(false);
				await loginManager.SetPasswordAsync(userArtifactId, password).ConfigureAwait(false);
			}

			if (adminsCanSetPasswordsInstanceSettingWasChanged)
			{
				await ChangeAdminsCanSetPasswordsInstanceSettingAsync(parameters, false).ConfigureAwait(false);
			}

			return userArtifactId;
		}

		private static async Task RemoveUserUsingKeplerAsync(IntegrationTestParameters parameters, int userId)
		{
			using (var userManager =
				ServiceHelper.GetServiceProxy<Services.Interfaces.UserInfo.IUserInfoManager>(parameters))
			{
				await userManager.DeleteAsync(userId).ConfigureAwait(false);
			}
		}

		private static async Task RemoveUserUsingHttpClientAsync(IntegrationTestParameters parameters, int userId)
		{
			string url =
				$"{parameters.RelativityRestUrl.AbsoluteUri}/Relativity.REST/Relativity/User/{userId}";

			await HttpClientHelper.DeleteAsync(parameters, new Uri(url)).ConfigureAwait(false);
		}

		private static async Task<bool> ChangeAdminsCanSetPasswordsInstanceSettingAsync(IntegrationTestParameters parameters, bool isEnabled)
		{
			const string Section = "Relativity.Authentication";
			const string Setting = "AdminsCanSetPasswords";
			string newValue = isEnabled.ToString();

			return await InstanceSettingsHelper
					   .ChangeInstanceSettingAndWait(parameters, Section, Setting, newValue)
					   .ConfigureAwait(false);
		}
	}
}
