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
	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Polly;
	using Polly.Retry;

	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityVersions;
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared;
	using Relativity.Services.Interfaces.Shared.Models;
	using Relativity.Services.Interfaces.Workspace;
	using Relativity.Services.Interfaces.Workspace.Models;

	using ArtifactType = Relativity.ArtifactType;
	using IFieldManager = Relativity.Services.Interfaces.Field.IFieldManager;

	/// <summary>
	/// Defines static helper methods to manage workspaces.
	/// </summary>
	public static class WorkspaceHelper
	{
		private static readonly RelativityVersion WorkspaceManagerReleaseVersion = RelativityVersion.Lanceleaf;

		public static (int workspaceId, string workspaceName) CreateTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
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
				workspaceTemplateName =
					NonDefaultCollationWorkspaceHelper.GetOrCreateWorkspaceTemplateWithNonDefaultCollation(
						parameters,
						logger);
			}

			string newWorkspaceName = GetWorkspaceName(parameters);

			// Prevent integration tests from failing due to workspace creation failures.
			const int MaxRetryCount = 3;
			int retryCount = 0;
			RetryPolicy retryPolicy = Policy.Handle<Exception>().WaitAndRetry(
				MaxRetryCount,
				i => TimeSpan.FromSeconds(MaxRetryCount),
				(exception, span) =>
					{
						retryCount++;
						logger.LogError(
							"The workspace helper failed to create a test workspace. Retry: {retryCount} of {MaxRetryCount}, Error: {exception}",
							retryCount,
							MaxRetryCount,
							exception);
					});

			retryPolicy.Execute(
				() =>
					{
						createdWorkspaceInfo = CreateWorkspaceFromTemplate(parameters, logger, workspaceTemplateName, newWorkspaceName);

						logger.LogInformation(
							"Created the {WorkspaceName} workspace. Workspace Artifact ID: {WorkspaceId}.",
							parameters.WorkspaceName,
							parameters.WorkspaceId);
					});
			return createdWorkspaceInfo;
		}

		public static void DeleteTestWorkspace(IntegrationTestParameters parameters, int workspaceToRemoveId, Relativity.Logging.ILog logger)
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
#pragma warning disable CS0618 // Type or member is obsolete
				using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
				{
					client.APIOptions.WorkspaceID = -1;
					logger.LogInformation("Deleting the {WorkspaceId} workspace.", workspaceToRemoveId);
					client.Repositories.Workspace.DeleteSingle(workspaceToRemoveId);
					logger.LogInformation("Deleted the {WorkspaceId} workspace.", workspaceToRemoveId);
				}
			}
			else
			{
				logger.LogInformation("Skipped deleting the {WorkspaceId} workspace.", workspaceToRemoveId);
			}
		}

		public static IList<string> QueryWorkspaceFolders(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

#pragma warning disable CS0618 // Type or member is obsolete
			using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				client.APIOptions.WorkspaceID = parameters.WorkspaceId;
				logger.LogInformation("Retrieving the {WorkspaceId} workspace folders...", parameters.WorkspaceId);
				Query<Folder> query = new Query<Folder> { Fields = FieldValue.AllFields };
				QueryResultSet<Folder> resultSet = client.Repositories.Folder.Query(query);
				List<string> folders = resultSet.Results.Select(x => x.Artifact.Name).ToList();
				logger.LogInformation(
					"Retrieved {FolderCount} {WorkspaceId} workspace folders.",
					folders.Count,
					parameters.WorkspaceId);
				return folders;
			}
		}

		public static void RenameTestWorkspace(IntegrationTestParameters parameters, int workspaceId, string newName)
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
			Policy.Handle<Exception>().WaitAndRetry(
				3,
				i => TimeSpan.FromSeconds(MaxRetryCount),
				(exception, span) =>
				{
					retryCount++;
					Console.WriteLine($"The workspace helper failed to rename a test workspace. Retry: {retryCount} of {MaxRetryCount}, Error: {exception}");
				}).Execute(
				() =>
				{
#pragma warning disable CS0618 // Type or member is obsolete
					using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
					{
						Workspace workspace = client.Repositories.Workspace.ReadSingle(workspaceId);
						workspace.Name = newName;
						client.Repositories.Workspace.UpdateSingle(workspace);
					}
				});
		}

		public static async Task UpdateWorkspaceResourcesAsync(
			IntegrationTestParameters parameters,
			int resourcePoolId,
			int fileShareId,
			int sqlServerId,
			int cacheLocationServerId)
		{
			if (CanUseWorkspaceManagerKepler(parameters))
			{
				await UpdateWorkspaceUsingKeplerAsync(
					parameters,
					resourcePoolId,
					fileShareId,
					sqlServerId,
					cacheLocationServerId).ConfigureAwait(false);
			}
			else
			{
				UpdateWorkspaceUsingRSAPI(
					parameters,
					resourcePoolId,
					fileShareId,
					sqlServerId,
					cacheLocationServerId);
			}
		}

		public static async Task<string> GetDefaultFileRepositoryAsync(IntegrationTestParameters parameters)
		{
			return CanUseWorkspaceManagerKepler(parameters)
				? await GetDefaultFileRepositoryUsingKeplerAsync(parameters).ConfigureAwait(false)
				: GetDefaultFileRepositoryUsingRSAPI(parameters);
		}

		internal static (int workspaceId, string workspaceName) CreateWorkspaceFromTemplate(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string workspaceTemplateName, string newWorkspaceName)
		{
			int templateWorkspaceId = RetrieveWorkspaceId(
				parameters,
				logger,
				workspaceTemplateName);

			if (templateWorkspaceId == -1)
			{
				throw new InvalidOperationException(
					$"Trying to create a workspace. Template with the following name does not exist: {workspaceTemplateName}");
			}

#pragma warning disable CS0618 // Type or member is obsolete
			using (var client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var workspace = new Workspace
				{
					Name = newWorkspaceName,
					DownloadHandlerApplicationPath = "Relativity.Distributed",
				};

				string workspaceName = workspace.Name;
				logger.LogInformation("Creating the {WorkspaceName} workspace...", workspaceName);
				ProcessOperationResult result =
					client.Repositories.Workspace.CreateAsync(templateWorkspaceId, workspace);
				int workspaceId = QueryWorkspaceArtifactId(client, result, logger);

				EnableDataGrid(client, parameters, workspaceId, logger);

				return (workspaceId, workspaceName);
			}
		}

		internal static int RetrieveWorkspaceId(
			IntegrationTestParameters parameters,
			Relativity.Logging.ILog logger,
			string workspaceName)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using (var client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				int workspaceId;

				logger.LogInformation("Retrieving the {workspaceName} workspace...", workspaceName);
				client.APIOptions.WorkspaceID = -1;
				QueryResultSet<Workspace> resultSet = QueryWorkspaceTemplate(client, workspaceName);
				if (!resultSet.Success)
				{
					throw new InvalidOperationException(
						$"An error occurred while attempting to create a workspace from template {workspaceName}: {resultSet.Message}");
				}

				if (resultSet.Results.Count == 0)
				{
					logger.LogWarning($"Workspace with the following name does not exist: {workspaceName}");
					workspaceId = -1;
				}
				else
				{
					workspaceId = resultSet.Results[0].Artifact.ArtifactID;
					logger.LogInformation(
						$"Retrieved the {workspaceName} workspace. workspaceId={workspaceId}.",
						workspaceName,
						workspaceId);
				}

				return workspaceId;
			}
		}

		private static QueryResultSet<Workspace> QueryWorkspaceTemplate(IRSAPIClient client, string templateName)
		{
			Query<Workspace> query = new Query<Workspace>
			{
				Condition = new TextCondition(WorkspaceFieldNames.Name, TextConditionEnum.EqualTo, templateName),
				Fields = FieldValue.AllFields,
			};

			QueryResultSet<Workspace> resultSet = client.Repositories.Workspace.Query(query);
			return resultSet;
		}

		private static int QueryWorkspaceArtifactId(
			IRSAPIClient client,
			ProcessOperationResult processResult,
			Relativity.Logging.ILog logger)
		{
			if (processResult.Message != null)
			{
				logger.LogError("Failed to create the workspace. Message: {Message}", processResult.Message);
				throw new InvalidOperationException(processResult.Message);
			}

			TaskCompletionSource<ProcessInformation> source = new TaskCompletionSource<ProcessInformation>();
			client.ProcessComplete += (sender, args) =>
			{
				logger.LogInformation("Completed the create workspace process.");
				source.SetResult(args.ProcessInformation);
			};

			client.ProcessCompleteWithError += (sender, args) =>
			{
				logger.LogError(
					"The create process completed with errors. Message: {Message}",
					args.ProcessInformation.Message);
				source.SetResult(args.ProcessInformation);
			};

			client.ProcessFailure += (sender, args) =>
			{
				logger.LogError(
					"The create process failed to complete. Message: {Message}",
					args.ProcessInformation.Message);
				source.SetResult(args.ProcessInformation);
			};
			client.MonitorProcessState(client.APIOptions, processResult.ProcessID);
			ProcessInformation processInfo = source.Task.GetAwaiter().GetResult();
			if (processInfo.OperationArtifactIDs.Any() && processInfo.OperationArtifactIDs[0] != null)
			{
				return processInfo.OperationArtifactIDs.FirstOrDefault().Value;
			}

			logger.LogError("The create process failed. Message: {Message}", processResult.Message);
			throw new InvalidOperationException(processResult.Message);
		}

		private static string GetWorkspaceName(IntegrationTestParameters parameters)
		{
			return string.IsNullOrEmpty(parameters.WorkspaceName)
			   ? $"Import API Sample Workspace ({DateTime.Now:MM-dd HH.mm.ss.fff})"
			   : parameters.WorkspaceName;
		}

		private static void EnableDataGrid(IRSAPIClient client, IntegrationTestParameters parameters, int workspaceId, Relativity.Logging.ILog logger)
		{
			// By default DataGrid is disable on workspace templates used in tests
			// It is impossible to disable DataGrid for workspace if it was enabled before
			if (parameters.EnableDataGrid)
			{
				var createdWorkspace = client.Repositories.Workspace.ReadSingle(parameters.WorkspaceId);
				createdWorkspace.EnableDataGrid = true;
				client.Repositories.Workspace.UpdateSingle(createdWorkspace);
				logger.LogInformation($"DataGrid enabled for workspace with id {parameters.WorkspaceId}");
			}

			UpdateExtractedTextField(parameters, workspaceId, logger).ConfigureAwait(false);
			logger.LogInformation($"'Extracted Text' field values in workspace updated");
		}

		private static async Task UpdateExtractedTextField(IntegrationTestParameters parameters, int workspaceId, Relativity.Logging.ILog logger)
		{
			var longTextFieldRequest = new LongTextFieldRequest()
			{
				Name = WellKnownFields.ExtractedText,
				ObjectType = new ObjectTypeIdentifier() { ArtifactTypeID = (int)ArtifactType.Document },
				EnableDataGrid = parameters.EnableDataGrid,
				IncludeInTextIndex = !parameters.EnableDataGrid,
				FilterType = FilterType.None,
				AvailableInViewer = true,
				HasUnicode = true,
			};

			using (IFieldManager fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(parameters))
			{
				int fieldId = FieldHelper.GetFieldArtifactId(parameters, workspaceId, logger, WellKnownFields.ExtractedText);
				await fieldManager.UpdateLongTextFieldAsync(workspaceId, fieldId, longTextFieldRequest)
					.ConfigureAwait(false);
			}
		}

		private static bool CanUseWorkspaceManagerKepler(IntegrationTestParameters parameters) =>
			!RelativityVersionChecker.VersionIsLowerThan(parameters, WorkspaceManagerReleaseVersion);

		private static async Task UpdateWorkspaceUsingKeplerAsync(
			IntegrationTestParameters parameters,
			int resourcePoolId,
			int fileShareId,
			int sqlServerId,
			int cacheLocationServerId)
		{
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

		private static void UpdateWorkspaceUsingRSAPI(
			IntegrationTestParameters parameters,
			int resourcePoolId,
			int fileShareId,
			int sqlServerId,
			int cacheLocationServerId)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var artifactIdCondition = new WholeNumberCondition(ArtifactQueryFieldNames.ArtifactID, NumericConditionEnum.EqualTo, parameters.WorkspaceId);
				var query = new Query<Workspace>
					            {
						            Condition = artifactIdCondition,
						            Fields = FieldValue.AllFields,
					            };

				Workspace workspace = client.Repositories.Workspace.Query(query).Results.First().Artifact;

				workspace.ResourcePoolID = resourcePoolId;
				var fileShareChoice = new kCura.Relativity.Client.DTOs.Choice(fileShareId);
				workspace.DefaultFileLocation = fileShareChoice;
				workspace.DefaultDataGridLocation = fileShareChoice;
				workspace.ServerID = sqlServerId;
				workspace.DefaultCacheLocation = cacheLocationServerId;

				client.Repositories.Workspace.UpdateSingle(workspace);
			}
		}

		private static async Task<string> GetDefaultFileRepositoryUsingKeplerAsync(IntegrationTestParameters parameters)
		{
			using (var workspaceManager = ServiceHelper.GetServiceProxy<IWorkspaceManager>(parameters))
			{
				var workspace = await workspaceManager.ReadAsync(parameters.WorkspaceId).ConfigureAwait(false);
				return workspace.DefaultFileRepository.Value.Name;
			}
		}

		private static string GetDefaultFileRepositoryUsingRSAPI(IntegrationTestParameters parameters)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using (var client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var workspace = client.Repositories.Workspace.ReadSingle(parameters.WorkspaceId);
				return workspace.DefaultFileLocation.Name;
			}
		}
	}
}