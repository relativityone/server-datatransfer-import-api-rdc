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
		public static async Task<List<int>> CreateFolders(IntegrationTestParameters parameters, IEnumerable<string> folders)
		{
			var folderIds = new List<int>();
			using (IFolderManager folderManager = ServiceHelper.GetServiceProxy<IFolderManager>(parameters))
			{
				Folder rootFolder = await folderManager.GetWorkspaceRootAsync(parameters.WorkspaceId);
				var folderRef = new FolderRef(rootFolder.ArtifactID);

				foreach (string folderName in folders)
				{
					var folder = new Folder() { Name = folderName, ParentFolder = folderRef };

					int folderId = await folderManager.CreateSingleAsync(parameters.WorkspaceId, folder);
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
					await permissionManager.GetItemLevelSecurityAsync(parameters.WorkspaceId, folderId);
				itemLevelSecurity.Enabled = true;

				await permissionManager.SetItemLevelSecurityAsync(parameters.WorkspaceId, itemLevelSecurity);

				GroupSelector groupSelector = await permissionManager.GetItemGroupSelectorAsync(parameters.WorkspaceId, folderId);

				GroupRef groupRef = groupSelector.EnabledGroups.Single(p => p.Name == group);

				GroupPermissions groupPermissions = await permissionManager.GetItemGroupPermissionsAsync(parameters.WorkspaceId, folderId, groupRef);
				ObjectPermission objectPermission = groupPermissions.ObjectPermissions.Single(p => p.Name == "Folder");
				objectPermission.AddSelected = true;

				await permissionManager.SetItemGroupPermissionsAsync(parameters.WorkspaceId, groupPermissions);
			}
		}
	}
}
