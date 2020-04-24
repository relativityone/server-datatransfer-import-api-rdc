// ----------------------------------------------------------------------------
// <copyright file="GroupHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System.Threading.Tasks;
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
			using (var groupManager = ServiceHelper.GetServiceProxy<IGroupManager>(parameters))
			{
				var groupRequest = new GroupRequest
				{
					Name = name,
					Client = new Securable<ObjectIdentifier>
					{
						Secured = false,
						Value = new ObjectIdentifier { ArtifactID = 1015644 },
					},
					Keywords = string.Empty,
					Notes = string.Empty,
				};

				var group = await groupManager.CreateAsync(groupRequest).ConfigureAwait(false);
				return group.ArtifactID;
			}
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
			using (var groupManager = ServiceHelper.GetServiceProxy<IGroupManager>(parameters))
			{
				await groupManager.AddMembersAsync(groupId, new ObjectIdentifier { ArtifactID = userId }).ConfigureAwait(false);
			}
		}
	}
}
