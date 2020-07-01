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

	using Relativity.DataExchange.Logger;

	/// <summary>
	/// Defines static helper methods to manage workspaces.
	/// </summary>
	public static class WorkspaceHelper
	{
		public static void CreateTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			// Prevent integration tests from failing due to workspace creation failures.
			const int MaxRetryCount = 3;
			int retryCount = 0;
			Policy.Handle<Exception>().WaitAndRetry(
				3,
				i => TimeSpan.FromSeconds(MaxRetryCount),
				(exception, span) =>
					{
						retryCount++;
						Console.WriteLine($"The workspace helper failed to create a test workspace. Retry: {retryCount} of {MaxRetryCount}, Error: {exception}");
					}).Execute(
				() =>
					{
						using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
						{
							logger.LogInformation(
								"Retrieving the {TemplateName} workspace template...",
								parameters.WorkspaceTemplate.Secure());
							client.APIOptions.WorkspaceID = -1;
							QueryResultSet<Workspace> resultSet = QueryWorkspaceTemplate(
								client,
								parameters.WorkspaceTemplate);
							if (!resultSet.Success)
							{
								throw new InvalidOperationException(
									$"An error occurred while attempting to create a workspace from template {parameters.WorkspaceTemplate}: {resultSet.Message}");
							}

							if (resultSet.Results.Count == 0)
							{
								throw new InvalidOperationException(
									$"Trying to create a workspace. Template with the following name does not exist: {parameters.WorkspaceTemplate}");
							}

							int templateWorkspaceId = resultSet.Results[0].Artifact.ArtifactID;
							logger.LogInformation(
								"Retrieved the {TemplateName} workspace template. TemplateWorkspaceId={TemplateWorkspaceId}.",
								parameters.WorkspaceTemplate.Secure(),
								templateWorkspaceId);

							var workspace = new Workspace
							{
								Name = GetWorkspaceName(parameters),
								DownloadHandlerApplicationPath = "Relativity.Distributed",
							};

							logger.LogInformation("Creating the {WorkspaceName} workspace...", workspace.Name.Secure());
							ProcessOperationResult result =
								client.Repositories.Workspace.CreateAsync(templateWorkspaceId, workspace);
							parameters.WorkspaceId = QueryWorkspaceArtifactId(client, result, logger);
							parameters.WorkspaceName = workspace.Name;

							EnableDataGrid(client, parameters, logger);

							logger.LogInformation(
								"Created the {WorkspaceName} workspace. Workspace Artifact ID: {WorkspaceId}.",
								workspace.Name.Secure(),
								parameters.WorkspaceId);
						}
					});
		}

		public static void DeleteTestWorkspace(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			if (parameters.WorkspaceId != 0)
			{
				using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
				{
					client.APIOptions.WorkspaceID = -1;
					logger.LogInformation("Deleting the {WorkspaceId} workspace.", parameters.WorkspaceId);
					client.Repositories.Workspace.DeleteSingle(parameters.WorkspaceId);
					logger.LogInformation("Deleted the {WorkspaceId} workspace.", parameters.WorkspaceId);
				}
			}
			else
			{
				logger.LogInformation("Skipped deleting the {WorkspaceId} workspace.", parameters.WorkspaceId);
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

			using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
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
					using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
					{
						Workspace workspace = client.Repositories.Workspace.ReadSingle(workspaceId);
						workspace.Name = newName;
						client.Repositories.Workspace.UpdateSingle(workspace);
					}
				});
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

		private static void EnableDataGrid(IRSAPIClient client, IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			var createdWorkspace = client.Repositories.Workspace.ReadSingle(parameters.WorkspaceId);
			createdWorkspace.EnableDataGrid = parameters.EnableDataGrid;
			client.Repositories.Workspace.UpdateSingle(createdWorkspace);
			logger.LogInformation(parameters.EnableDataGrid ? $"Set DataGrid enabled for workspace with id {parameters.WorkspaceId}" : $"Set DataGrid disabled for workspace with id {parameters.WorkspaceId}");
		}
	}
}