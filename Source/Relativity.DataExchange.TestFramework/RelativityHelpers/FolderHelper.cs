// ----------------------------------------------------------------------------
// <copyright file="FolderHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Folder;
	using Relativity.Services.Group;
	using Relativity.Services.Permission;

	public static class FolderHelper
	{
		public static async Task<int> GetWorkspaceRootArtifactIdAsync(IntegrationTestParameters parameters)
		{
			using (IFolderManager folderManager = ServiceHelper.GetServiceProxy<IFolderManager>(parameters))
			{
				var folder = await folderManager.GetWorkspaceRootAsync(parameters.WorkspaceId).ConfigureAwait(false);
				return folder.ArtifactID;
			}
		}

		public static async Task<List<int>> CreateFolders(IntegrationTestParameters parameters, IEnumerable<string> folders, int rootFolderArtifactId)
		{
			var folderIds = new List<int>();
			using (IFolderManager folderManager = ServiceHelper.GetServiceProxy<IFolderManager>(parameters))
			{
				var folderRef = new FolderRef(rootFolderArtifactId);

				foreach (string folderName in folders)
				{
					var folder = new Folder() { Name = folderName, ParentFolder = folderRef };

					int folderId = await folderManager.CreateSingleAsync(parameters.WorkspaceId, folder).ConfigureAwait(false);
					folderIds.Add(folderId);
				}
			}

			return folderIds;
		}

		public static async Task SetItemLevelSecurity(IntegrationTestParameters parameters, int folderId, string group)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				ItemLevelSecurity itemLevelSecurity =
					await permissionManager.GetItemLevelSecurityAsync(parameters.WorkspaceId, folderId).ConfigureAwait(false);
				itemLevelSecurity.Enabled = true;

				await permissionManager.SetItemLevelSecurityAsync(parameters.WorkspaceId, itemLevelSecurity).ConfigureAwait(false);

				GroupSelector groupSelector = await permissionManager.GetItemGroupSelectorAsync(parameters.WorkspaceId, folderId).ConfigureAwait(false);

				GroupRef groupRef = groupSelector.EnabledGroups.Single(p => p.Name == group);

				GroupPermissions groupPermissions = await permissionManager.GetItemGroupPermissionsAsync(parameters.WorkspaceId, folderId, groupRef).ConfigureAwait(false);
				ObjectPermission objectPermission = groupPermissions.ObjectPermissions.Single(p => p.Name == "Folder");
				objectPermission.AddSelected = true;

				await permissionManager.SetItemGroupPermissionsAsync(parameters.WorkspaceId, groupPermissions).ConfigureAwait(false);
			}
		}
	}
}
