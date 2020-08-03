// ----------------------------------------------------------------------------
// <copyright file="GroupHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.IO;
	using System.Threading.Tasks;

	using Newtonsoft.Json.Linq;

	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Interfaces.Group;
	using Relativity.Services.Interfaces.Group.Models;
	using Relativity.Services.Interfaces.Shared;
	using Relativity.Services.Interfaces.Shared.Models;

	/// <summary>
	/// Defines static helper methods to manage groups in tests.
	/// </summary>
	public static class GroupHelper
	{
		public static async Task<int> CreateNewGroupAsync(IntegrationTestParameters parameters, string name)
		{
			return RelativityVersionChecker.VersionIsLowerThan(parameters, RelativityVersion.Lanceleaf)
			? await CreateNewGroupAsyncUsingHttpClient(parameters, name).ConfigureAwait(false)
			: await CreateNewGroupAsyncUsingKepler(parameters, name).ConfigureAwait(false);
		}

		public static async Task RemoveGroupAsync(IntegrationTestParameters parameters, int groupId)
		{
			using (var groupManager = ServiceHelper.GetServiceProxy<IGroupManager>(parameters))
			{
				await groupManager.DeleteAsync(groupId).ConfigureAwait(false);
			}
		}

		public static async Task AddMemberAsync(IntegrationTestParameters parameters, int groupId, int userId)
		{
			if (RelativityVersionChecker.VersionIsLowerThan(parameters, RelativityVersion.Lanceleaf))
			{
				await AddMemberAsyncUsingHttpClient(parameters, groupId, userId).ConfigureAwait(false);
			}
			else
			{
				await AddMemberAsyncUsingKepler(parameters, groupId, userId).ConfigureAwait(false);
			}
		}

		private static async Task<int> CreateNewGroupAsyncUsingKepler(IntegrationTestParameters parameters, string name)
		{
			using (var groupManager = ServiceHelper.GetServiceProxy<IGroupManager>(parameters))
			{
				const int RelativityClientId = 1015644;

				var groupRequest = new GroupRequest
				{
					Name = name,
					Client = new Securable<ObjectIdentifier>
					{
						Secured = false,
						Value = new ObjectIdentifier { ArtifactID = RelativityClientId },
					},
					Keywords = string.Empty,
					Notes = string.Empty,
				};

				var group = await groupManager.CreateAsync(groupRequest).ConfigureAwait(false);
				return group.ArtifactID;
			}
		}

		private static async Task<int> CreateNewGroupAsyncUsingHttpClient(IntegrationTestParameters parameters, string name)
		{
			string createGroupJson = ResourceFileHelper.GetResourceFolderPath("CreateGroupRequest.json");
			JObject request = JObject.Parse(File.ReadAllText(createGroupJson));

			request["Name"] = name;

			string url =
				$"{parameters.RelativityRestUrl.AbsoluteUri}/Relativity.REST/Relativity/Group";

			string result = await HttpClientHelper.PostAsync(parameters, new Uri(url), request.ToString())
								.ConfigureAwait(false);

			JObject resultObject = JObject.Parse(result);

			return (int)resultObject["Results"][0]["ArtifactID"];
		}

		private static async Task AddMemberAsyncUsingKepler(IntegrationTestParameters parameters, int groupId, int userId)
		{
			using (var groupManager = ServiceHelper.GetServiceProxy<IGroupManager>(parameters))
			{
				await groupManager.AddMembersAsync(groupId, new ObjectIdentifier { ArtifactID = userId }).ConfigureAwait(false);
			}
		}

		private static async Task AddMemberAsyncUsingHttpClient(IntegrationTestParameters parameters, int groupId, int userId)
		{
			string request = await PrepareAddMemberToGroupRequest(parameters, groupId, userId).ConfigureAwait(false);

			string url =
				$"{parameters.RelativityRestUrl.AbsoluteUri}/Relativity.REST/Relativity/User/{userId}";

			await HttpClientHelper.PutAsync(parameters, new Uri(url), request).ConfigureAwait(false);
		}

		private static async Task<string> PrepareAddMemberToGroupRequest(IntegrationTestParameters parameters, int groupId, int userId)
		{
			string addGroupMemberJson = ResourceFileHelper.GetResourceFolderPath("AddGroupMemberRequest.json");
			JObject request = JObject.Parse(File.ReadAllText(addGroupMemberJson));
			JObject userInfo = JObject.Parse(await UsersHelper.GetUserInfo(parameters, userId).ConfigureAwait(false));

			request["Artifact ID"] = userInfo["Artifact ID"];
			request["Groups"] = UpdateGroupsForRequest(JArray.Parse(userInfo["Groups"].ToString()), groupId);
			request["Last Name"] = userInfo["Last Name"];
			request["Type"]["Artifact ID"] = userInfo["Type"]["Artifact ID"];
			request["Client"]["Artifact ID"] = userInfo["Client"]["Artifact ID"];

			return request.ToString();
		}

		private static JArray UpdateGroupsForRequest(JArray groupsArray, int groupId)
		{
			JObject groupToAdd = new JObject
			{
				["Artifact ID"] = groupId,
			};

			groupsArray.Add(groupToAdd);

			return groupsArray;
		}
	}
}
