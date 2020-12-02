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
	using System.Threading.Tasks;

	using Newtonsoft.Json.Linq;

	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Security;
	using Relativity.Services.Security.Models;
	using Relativity.Services.User;

	using User = Relativity.Services.User.User;

	/// <summary>
	/// Defines static helper methods to manage users in tests.
	/// </summary>
	public static class UsersHelper
	{
		public static async Task<int> CreateNewUserAsync(
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

		public static async Task RemoveUserAsync(IntegrationTestParameters parameters, int userId)
		{
			if (RelativityVersionChecker.VersionIsLowerThan(parameters, RelativityVersion.Lanceleaf))
			{
				await RemoveUserAsyncUsingHttpClient(parameters, userId).ConfigureAwait(false);
			}
			else
			{
				await RemoveUserAsyncUsingKepler(parameters, userId).ConfigureAwait(false);
			}
		}

		public static async Task<string> GetUserInfo(IntegrationTestParameters parameters, int userId)
		{
			string url =
				$"{parameters.RelativityRestUrl.AbsoluteUri}/Relativity.REST/Relativity/User/{userId}";

			return await HttpClientHelper.GetAsync(parameters, new Uri(url)).ConfigureAwait(false);
		}

		private static async Task RemoveUserAsyncUsingKepler(IntegrationTestParameters parameters, int userId)
		{
			using (var userManager =
				ServiceHelper.GetServiceProxy<Services.Interfaces.UserInfo.IUserInfoManager>(parameters))
			{
				await userManager.DeleteAsync(userId).ConfigureAwait(false);
			}
		}

		private static async Task RemoveUserAsyncUsingHttpClient(IntegrationTestParameters parameters, int userId)
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
