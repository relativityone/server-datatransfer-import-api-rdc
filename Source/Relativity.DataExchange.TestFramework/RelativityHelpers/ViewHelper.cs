﻿// ----------------------------------------------------------------------------
// <copyright file="ViewHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Field;
	using Relativity.Services.User;
	using Relativity.Services.View;

	public static class ViewHelper
	{
		private const int DefaultViewOrder = 9999;

		public static async Task<int> CreateViewAsync(
			IntegrationTestParameters parameters,
			string viewName,
			int artifactTypeId,
			params int[] fieldIds)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			View view = new View
			{
				ArtifactTypeID = artifactTypeId,
				Order = ViewHelper.DefaultViewOrder,
				VisibleInDropdown = true,
				QueryHint = string.Empty,
				Owner = new UserRef { ArtifactID = 0, Name = "Public" },
				Name = viewName,
				Fields = fieldIds.Select(fieldId => new FieldRef(fieldId)).ToList(),
			};

			using (var viewManager = ServiceHelper.GetServiceProxy<IViewManager>(parameters))
			{
				int viewArtifactId = await viewManager.CreateSingleAsync(parameters.WorkspaceId, view).ConfigureAwait(false);
				return viewArtifactId;
			}
		}
	}
}