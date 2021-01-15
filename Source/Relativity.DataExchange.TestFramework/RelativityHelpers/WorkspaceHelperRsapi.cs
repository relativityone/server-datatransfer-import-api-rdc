// ----------------------------------------------------------------------------
// <copyright file="WorkspaceHelperRsapi.cs" company="Relativity ODA LLC">
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

	using Relativity.DataExchange.TestFramework;

	using ArtifactQueryFieldNames = kCura.Relativity.Client.DTOs.ArtifactQueryFieldNames;
	using NumericConditionEnum = kCura.Relativity.Client.NumericConditionEnum;
	using WholeNumberCondition = kCura.Relativity.Client.WholeNumberCondition;

	/// <summary>
	/// Defines static helper methods to manage workspaces.
	/// </summary>
	public static class WorkspaceHelperRsapi
	{
		internal static void DeleteTestWorkspace(IntegrationTestParameters parameters, int workspaceToRemoveId, Relativity.Logging.ILog logger)
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

		internal static IList<string> QueryWorkspaceFolders(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				client.APIOptions.WorkspaceID = parameters.WorkspaceId;
				logger.LogInformation("Retrieving the {WorkspaceId} workspace folders...", parameters.WorkspaceId);
				var query = new Query<kCura.Relativity.Client.DTOs.Folder> { Fields = FieldValue.AllFields };
				QueryResultSet<kCura.Relativity.Client.DTOs.Folder> resultSet = client.Repositories.Folder.Query(query);
				List<string> folders = resultSet.Results.Select(x => x.Artifact.Name).ToList();
				logger.LogInformation(
					"Retrieved {FolderCount} {WorkspaceId} workspace folders.",
					folders.Count,
					parameters.WorkspaceId);
				return folders;
			}
		}

		internal static void RenameTestWorkspace(IntegrationTestParameters parameters, int workspaceId, string newName)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				Workspace workspace = client.Repositories.Workspace.ReadSingle(workspaceId);
				workspace.Name = newName;
				client.Repositories.Workspace.UpdateSingle(workspace);
			}
		}

		internal static void UpdateWorkspaceResources(
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

		internal static string GetDefaultFileRepository(IntegrationTestParameters parameters)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using (var client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(parameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var workspace = client.Repositories.Workspace.ReadSingle(parameters.WorkspaceId);
				return workspace.DefaultFileLocation.Name;
			}
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

			using (Task task = WorkspaceHelper.UpdateExtractedTextFieldAsync(parameters, workspaceId, logger))
			{
				task.Wait();
			}

			logger.LogInformation($"'Extracted Text' field values in workspace updated");
		}
	}
}