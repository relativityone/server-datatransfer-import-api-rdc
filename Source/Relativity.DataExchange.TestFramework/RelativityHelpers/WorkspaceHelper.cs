// ----------------------------------------------------------------------------
// <copyright file="WorkspaceHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Polly;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Folder;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Interfaces.Workspace;
	using Relativity.Services.Interfaces.Workspace.Models;
	using Relativity.Services.Objects.DataContracts;

	using ArtifactType = Relativity.ArtifactType;
	using IFieldManager = Relativity.Services.Interfaces.Field.IFieldManager;

	/// <summary>
	/// Defines static helper methods to manage workspaces.
	/// </summary>
	public static class WorkspaceHelper
	{
		private static readonly RelativityVersion WorkspaceManagerReleaseVersion = RelativityVersion.Mayapple;

		public static (int workspaceId, string workspaceName) CreateTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			using (Task<(int workspaceId, string workspaceName)> task = CreateTestWorkspaceAsync(parameters, logger))
			{
				return task.GetAwaiter().GetResult();
			}
		}

		public static async Task<(int workspaceId, string workspaceName)> CreateTestWorkspaceAsync(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			(int workspaceId, string workspaceName) createdWorkspaceInfo = (-1, null);

			string workspaceTemplateName = parameters.WorkspaceTemplate;
			if (parameters.TestOnWorkspaceWithNonDefaultCollation)
			{
				workspaceTemplateName = await NonDefaultCollationWorkspaceHelper
											.GetOrCreateWorkspaceTemplateWithNonDefaultCollationAsync(
												parameters,
												logger)
											.ConfigureAwait(false);
			}

			string newWorkspaceName = GetWorkspaceName(parameters);

			// Prevent integration tests from failing due to workspace creation failures.
			const int MaxRetryCount = 3;
			int retryCount = 0;

			void OnRetry(Exception exception, TimeSpan span)
			{
				retryCount++;
				logger.LogError(
					"The workspace helper failed to create a test workspace. Retry: {retryCount} of {MaxRetryCount}, Error: {exception}",
					retryCount,
					MaxRetryCount,
					exception);
			}

			Task<(int workspaceId, string workspaceName)> CreateWorkspaceFunction() =>
				CanUseWorkspaceManagerKepler(parameters)
					? CreateWorkspaceFromTemplateUsingKeplerAsync(parameters, logger, workspaceTemplateName, newWorkspaceName)
					: WorkspaceHelperRsapi.CreateWorkspaceFromTemplateAsync(parameters, logger, workspaceTemplateName, newWorkspaceName);

			createdWorkspaceInfo = await Policy
									   .Handle<Exception>()
									   .WaitAndRetryAsync(
										   MaxRetryCount,
										   i => TimeSpan.FromSeconds(MaxRetryCount),
										   OnRetry)
									   .ExecuteAsync(() => CreateWorkspaceFunction())
									   .ConfigureAwait(false);

			logger.LogInformation(
				"Created the {WorkspaceName} workspace. Workspace Artifact ID: {WorkspaceId}.",
				createdWorkspaceInfo.workspaceName,
				createdWorkspaceInfo.workspaceId);

			return createdWorkspaceInfo;
		}

		public static async Task DeleteTestWorkspaceAsync(IntegrationTestParameters parameters, int workspaceToRemoveId, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			if (workspaceToRemoveId != 0)
			{
				if (!CanUseWorkspaceManagerKepler(parameters))
				{
					WorkspaceHelperRsapi.DeleteTestWorkspace(parameters, workspaceToRemoveId, logger);
					return;
				}

				using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
				{
					logger.LogInformation("Deleting the {WorkspaceId} workspace.", workspaceToRemoveId);
					await workspaceManager.DeleteAsync(workspaceToRemoveId).ConfigureAwait(false);

					logger.LogInformation("Deleted the {WorkspaceId} workspace.", workspaceToRemoveId);
				}
			}
			else
			{
				logger.LogInformation("Skipped deleting the {WorkspaceId} workspace.", workspaceToRemoveId);
			}
		}

		public static async Task<IList<string>> QueryWorkspaceFoldersAsync(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			if (!CanUseWorkspaceManagerKepler(parameters))
			{
				return WorkspaceHelperRsapi.QueryWorkspaceFolders(parameters, logger);
			}

			using (IFolderManager folderManager = ServiceHelper.GetServiceProxy<IFolderManager>(parameters))
			{
				var folderQuery = new Services.Query();
				var queryResponse = await folderManager.QueryAsync(parameters.WorkspaceId, folderQuery).ConfigureAwait(false);
				List<string> folders = queryResponse.Results.Select(x => x.Artifact.Name).ToList();
				logger.LogInformation(
					"Retrieved {FolderCount} {WorkspaceId} workspace folders.",
					folders.Count,
					parameters.WorkspaceId);
				return folders;
			}
		}

		public static void RenameTestWorkspace(IntegrationTestParameters parameters, int workspaceId, string newName)
		{
			using (Task task = RenameTestWorkspaceAsync(parameters, workspaceId, newName))
			{
				task.Wait();
			}
		}

		public static async Task RenameTestWorkspaceAsync(IntegrationTestParameters parameters, int workspaceId, string newName)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (newName == null)
			{
				throw new ArgumentNullException(nameof(newName));
			}

			// Prevent integration tests from failing due to RSAPI failures.
			const int MaxRetryCount = 3;
			int retryCount = 0;
			Action<Exception, TimeSpan> onRetry = (exception, span) =>
			{
				retryCount++;
				Console.WriteLine($"The workspace helper failed to rename a test workspace. Retry: {retryCount} of {MaxRetryCount}, Error: {exception}");
			};

			if (!CanUseWorkspaceManagerKepler(parameters))
			{
				Policy.Handle<Exception>().WaitAndRetry(
					3,
					i => TimeSpan.FromSeconds(MaxRetryCount),
					onRetry).Execute(
					() =>
						{
							WorkspaceHelperRsapi.RenameTestWorkspace(parameters, workspaceId, newName);
						});
			}
			else
			{
				await Policy.Handle<Exception>().WaitAndRetryAsync(
					3,
					i => TimeSpan.FromSeconds(MaxRetryCount),
					onRetry).ExecuteAsync(
					async () =>
						{
							using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
							using (Task<WorkspaceResponse> readTask = workspaceManager.ReadAsync(workspaceId))
							{
								WorkspaceResponse readResponse = readTask.Result;
								var updateRequest = new WorkspaceRequest(readResponse)
								{
									Name = newName,
									EnableDataGrid = parameters.EnableDataGrid,
								};
								await workspaceManager.UpdateAsync(workspaceId, updateRequest).ConfigureAwait(false);
							}
						}).ConfigureAwait(false);
			}
		}

		public static async Task UpdateWorkspaceResourcesAsync(
			IntegrationTestParameters parameters,
			int resourcePoolId,
			int fileShareId,
			int sqlServerId,
			int cacheLocationServerId)
		{
			if (!CanUseWorkspaceManagerKepler(parameters))
			{
				WorkspaceHelperRsapi.UpdateWorkspaceResources(parameters, resourcePoolId, fileShareId, sqlServerId, cacheLocationServerId);
				return;
			}

			using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
			{
				WorkspaceResponse existingWorkspace = await workspaceManager.ReadAsync(parameters.WorkspaceId, includeMetadata: false, includeActions: false).ConfigureAwait(false);
				WorkspaceRequest updateRequest = new WorkspaceRequest(existingWorkspace)
				{
					ResourcePool = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = resourcePoolId }),
					DefaultFileRepository = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = fileShareId }),
					DataGridFileRepository = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = fileShareId }),
					SqlServer = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = sqlServerId }),
					DefaultCacheLocation = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = cacheLocationServerId }),
				};

				await workspaceManager.UpdateAsync(parameters.WorkspaceId, updateRequest).ConfigureAwait(false);
			}
		}

		public static async Task<string> GetDefaultFileRepositoryAsync(IntegrationTestParameters parameters)
		{
			if (!CanUseWorkspaceManagerKepler(parameters))
			{
				return WorkspaceHelperRsapi.GetDefaultFileRepository(parameters);
			}

			using (var workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
			{
				var workspace = await workspaceManager.ReadAsync(parameters.WorkspaceId).ConfigureAwait(false);
				return workspace.DefaultFileRepository.Value.Name;
			}
		}

		public static async Task UpdateExtractedTextFieldAsync(IntegrationTestParameters parameters, int workspaceId, Relativity.Logging.ILog logger)
		{
			int fieldId = await FieldHelper
							  .GetFieldArtifactIdAsync(parameters, workspaceId, logger, WellKnownFields.ExtractedText)
							  .ConfigureAwait(false);

			bool enableDataGrid;
			if (parameters.EnableDataGrid)
			{
				enableDataGrid = parameters.EnableDataGrid;
			}
			else
			{
				// it is not possible to disable DataGrid for field, if it is already enabled.
				using (IFieldManager fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(parameters))
				{
					var fieldResponse = await fieldManager.ReadAsync(workspaceId, fieldId).ConfigureAwait(false);
					enableDataGrid = fieldResponse.EnableDataGrid;
				}
			}

			var longTextFieldRequest = new LongTextFieldRequest
			{
				Name = WellKnownFields.ExtractedText,
				ObjectType = new ObjectTypeIdentifier { ArtifactTypeID = (int)ArtifactType.Document },
				EnableDataGrid = enableDataGrid,
				IncludeInTextIndex = !enableDataGrid,
				FilterType = FilterType.None,
				AvailableInViewer = true,
				HasUnicode = true,
			};

			using (IFieldManager fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(parameters))
			{
				await fieldManager.UpdateLongTextFieldAsync(workspaceId, fieldId, longTextFieldRequest).ConfigureAwait(false);
			}
		}

		internal static Task<(int workspaceId, string workspaceName)> CreateWorkspaceFromTemplateAsync(
			IntegrationTestParameters parameters,
			Relativity.Logging.ILog logger,
			string workspaceTemplateName,
			string newWorkspaceName)
		{
			return CanUseWorkspaceManagerKepler(parameters)
				? CreateWorkspaceFromTemplateUsingKeplerAsync(parameters, logger, workspaceTemplateName, newWorkspaceName)
				: WorkspaceHelperRsapi.CreateWorkspaceFromTemplateAsync(parameters, logger, workspaceTemplateName, newWorkspaceName);
		}

		internal static Task<int> RetrieveWorkspaceIdAsync(
			IntegrationTestParameters parameters,
			Relativity.Logging.ILog logger,
			string workspaceName)
		{
			return CanUseWorkspaceManagerKepler(parameters)
					   ? RetrieveWorkspaceIdUsingKeplerAsync(parameters, logger, workspaceName)
					   : Task.FromResult(WorkspaceHelperRsapi.RetrieveWorkspaceId(parameters, logger, workspaceName));
		}

		private static async Task<(int workspaceId, string workspaceName)> CreateWorkspaceFromTemplateUsingKeplerAsync(
			IntegrationTestParameters parameters,
			Relativity.Logging.ILog logger,
			string workspaceTemplateName,
			string newWorkspaceName)
		{
			try
			{
				int templateWorkspaceId = await RetrieveWorkspaceIdUsingKeplerAsync(parameters, logger, workspaceTemplateName).ConfigureAwait(false);

				using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
				{
					WorkspaceResponse readResponse = await workspaceManager.ReadAsync(templateWorkspaceId).ConfigureAwait(false);

					var createRequest = new WorkspaceRequest(readResponse)
					{
						Name = newWorkspaceName,
						Template = new Securable<ObjectIdentifier>(new ObjectIdentifier { ArtifactID = templateWorkspaceId }),
					};

					logger.LogInformation("Creating the {WorkspaceName} workspace...", newWorkspaceName);

					WorkspaceResponse createResponse =
						await workspaceManager.CreateAsync(createRequest).ConfigureAwait(false);
					logger.LogInformation("Completed the create workspace process.");

					if (parameters.EnableDataGrid)
					{
						var updateRequest = new WorkspaceRequest(createResponse)
						{
							EnableDataGrid = true,
						};

						await workspaceManager.UpdateAsync(createResponse.ArtifactID, updateRequest, createResponse.LastModifiedOn).ConfigureAwait(false);
						logger.LogInformation($"DataGrid enabled for workspace with id {parameters.WorkspaceId}");
					}

					await UpdateExtractedTextFieldAsync(parameters, createResponse.ArtifactID, logger).ConfigureAwait(false);
					logger.LogInformation($"'Extracted Text' field values in workspace updated");

					return (createResponse.ArtifactID, newWorkspaceName);
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex.Message);
				throw;
			}
		}

		private static async Task<int> RetrieveWorkspaceIdUsingKeplerAsync(
			IntegrationTestParameters parameters,
			Relativity.Logging.ILog logger,
			string workspaceName)
		{
			logger.LogInformation("Retrieving the {workspaceName} workspace...", workspaceName);
			var queryRequest = new QueryRequest { Condition = $"'Name' == '{workspaceName}'" };

			QueryResultSlim queryResponse;
			using (IWorkspaceManager workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
			{
				queryResponse = await workspaceManager
									.QueryEligibleTemplatesAsync(queryRequest, 0, 2)
									.ConfigureAwait(false);
			}

			switch (queryResponse.Objects.Count)
			{
				case 0:
					throw new InvalidOperationException($"Workspace with the following name does not exist: {workspaceName}");
				case 1:
					int workspaceId = queryResponse.Objects[0].ArtifactID;
					logger.LogInformation(
						$"Retrieved the {workspaceName} workspace. workspaceId={workspaceId}.",
						workspaceName,
						workspaceId);
					return workspaceId;
				default:
					throw new InvalidOperationException($"More then one Workspace with the following name exists: {workspaceName}");
			}
		}

		private static string GetWorkspaceName(IntegrationTestParameters parameters)
		{
			return string.IsNullOrEmpty(parameters.WorkspaceName)
			   ? $"Import API Sample Workspace ({DateTime.Now:MM-dd HH.mm.ss.fff})"
			   : parameters.WorkspaceName;
		}

		private static bool CanUseWorkspaceManagerKepler(IntegrationTestParameters parameters) =>
			!RelativityVersionChecker.VersionIsLowerThan(parameters, WorkspaceManagerReleaseVersion);
	}
}