// ----------------------------------------------------------------------------
// <copyright file="PermissionsHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Group;
	using Relativity.Services.Permission;

	public static class PermissionsHelper
	{
		public static async Task AddGroupToWorkspaceAsync(IntegrationTestParameters parameters, int groupId)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				GroupRef group = new GroupRef(groupId);
				GroupSelector groupSelector = await permissionManager.GetWorkspaceGroupSelectorAsync(parameters.WorkspaceId).ConfigureAwait(false);
				groupSelector.DisabledGroups.Remove(group);
				groupSelector.EnabledGroups.Add(group);
				await permissionManager.AddRemoveWorkspaceGroupsAsync(parameters.WorkspaceId, groupSelector).ConfigureAwait(false);
			}
		}

		public static async Task AddGroupToAdminAsync(IntegrationTestParameters parameters, int groupId)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				GroupRef group = new GroupRef(groupId);
				GroupSelector groupSelector = await permissionManager.GetAdminGroupSelectorAsync().ConfigureAwait(false);
				groupSelector.DisabledGroups.Remove(group);
				groupSelector.EnabledGroups.Add(group);
				await permissionManager.AddRemoveAdminGroupsAsync(groupSelector).ConfigureAwait(false);
			}
		}

		public static async Task SetWorkspaceOtherSettingsAsync(
			IntegrationTestParameters parameters,
			int groupId,
			List<string> permissionNames,
			bool permissionValue)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				GroupPermissions workspacePermission =
					await permissionManager.GetWorkspaceGroupPermissionsAsync(parameters.WorkspaceId, new GroupRef(groupId)).ConfigureAwait(false);

				bool Selector(GenericPermission permission) => permissionNames.Contains(permission.Name);

				SetGenericPermissions(workspacePermission.AdminPermissions, Selector, permissionValue);
				SetGenericPermissions(workspacePermission.MassActionPermissions, Selector, permissionValue);
				SetGenericPermissions(workspacePermission.BrowserPermissions, Selector, permissionValue);

				await permissionManager
					.SetWorkspaceGroupPermissionsAsync(parameters.WorkspaceId, workspacePermission)
					.ConfigureAwait(false);
			}
		}

		public static async Task SetAllWorkspaceOtherSettingsAsync(
			IntegrationTestParameters parameters,
			int groupId,
			bool value)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				GroupPermissions workspacePermission =
					await permissionManager.GetWorkspaceGroupPermissionsAsync(parameters.WorkspaceId, new GroupRef(groupId)).ConfigureAwait(false);

				SetGenericPermissions(workspacePermission.AdminPermissions, permission => true, value);
				SetGenericPermissions(workspacePermission.MassActionPermissions, permission => true, value);
				SetGenericPermissions(workspacePermission.BrowserPermissions, permission => true, value);

				await permissionManager
					.SetWorkspaceGroupPermissionsAsync(parameters.WorkspaceId, workspacePermission)
					.ConfigureAwait(false);
			}
		}

		public static async Task SetWorkspaceObjectSecurityAsync(
			IntegrationTestParameters parameters,
			int groupId,
			List<string> permissionNames,
			bool canAdd,
			bool canEdit,
			bool subPermissionSelected)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				GroupPermissions workspacePermission = await permissionManager
					                                       .GetWorkspaceGroupPermissionsAsync(parameters.WorkspaceId, new GroupRef(groupId))
					                                       .ConfigureAwait(false);

				SetObjectAndSubPermissions(
					workspacePermission.ObjectPermissions,
					permission => permissionNames.Contains(permission.Name),
					canAdd,
					canEdit,
					subPermissionSelected);

				await permissionManager
					.SetWorkspaceGroupPermissionsAsync(parameters.WorkspaceId, workspacePermission)
					.ConfigureAwait(false);
			}
		}

		public static async Task SetAdminObjectSecurityAsync(
			IntegrationTestParameters parameters,
			int groupId,
			List<string> permissionNames,
			bool canAdd,
			bool canEdit,
			bool subPermissionSelected)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				GroupPermissions adminPermission = await permissionManager
					                                   .GetAdminGroupPermissionsAsync(new GroupRef(groupId))
					                                   .ConfigureAwait(false);

				SetObjectAndSubPermissions(
					adminPermission.ObjectPermissions,
					permission => permissionNames.Contains(permission.Name),
					canAdd,
					canEdit,
					subPermissionSelected);

				await permissionManager
					.SetAdminGroupPermissionsAsync(adminPermission)
					.ConfigureAwait(false);
			}
		}

		public static async Task ApplyItemLevelSecurityAsync(
			IntegrationTestParameters parameters,
			int artifactId,
			int groupId,
			bool enabled)
		{
			using (IPermissionManager permissionManager = ServiceHelper.GetServiceProxy<IPermissionManager>(parameters))
			{
				ItemLevelSecurity itemLevelSecurity = await permissionManager.GetItemLevelSecurityAsync(parameters.WorkspaceId, artifactId)
					                                      .ConfigureAwait(false);

				itemLevelSecurity.Enabled = true;

				await permissionManager.SetItemLevelSecurityAsync(parameters.WorkspaceId, itemLevelSecurity)
					.ConfigureAwait(false);

				GroupSelector selector = await permissionManager.GetItemGroupSelectorAsync(parameters.WorkspaceId, artifactId)
					                         .ConfigureAwait(false);

				var groupRef = new GroupRef(groupId);

				if (enabled)
				{
					selector.EnabledGroups.Add(groupRef);
					selector.DisabledGroups.RemoveAll(x => x.ArtifactID == groupRef.ArtifactID);
				}
				else
				{
					selector.DisabledGroups.Add(groupRef);
					selector.EnabledGroups.RemoveAll(x => x.ArtifactID == groupRef.ArtifactID);
				}

				await permissionManager.AddRemoveItemGroupsAsync(parameters.WorkspaceId, artifactId, selector)
					.ConfigureAwait(false);
			}
		}

		private static void SetGenericPermissions(
			List<GenericPermission> availablePermissions,
			Func<GenericPermission, bool> predicate,
			bool value)
		{
			foreach (var permission in availablePermissions.Where(predicate))
			{
				permission.Selected = value;
			}
		}

		private static void SetObjectAndSubPermissions(
			List<ObjectPermission> availablePermissions,
			Func<ObjectPermission, bool> predicate,
			bool canAdd,
			bool canEdit,
			bool subPermissionSelected)
		{
			foreach (var permission in availablePermissions.Where(predicate))
			{
				permission.AddSelected = canAdd;
				permission.EditSelected = canEdit;
				foreach (PermissionDetail subPermission in permission.SubPermissions)
				{
					subPermission.Selected = subPermissionSelected;
				}
			}
		}
	}
}