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
	using Relativity.Services.Interfaces.Field.Models;
	using Relativity.Services.Interfaces.Shared.Models;

	using ArtifactType = Relativity.ArtifactType;
	using IFieldManager = Relativity.Services.Interfaces.Field.IFieldManager;

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

			string workspaceTemplateName = parameters.TestOnWorkspaceWithNonDefaultCollation
				? NonDefaultCollationWorkspaceHelper.GetOrCreateWorkspaceTemplateWithNonDefaultCollation(parameters, logger)
				: parameters.WorkspaceTemplate;

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
						CreateWorkspaceFromTemplate(parameters, logger, workspaceTemplateName, newWorkspaceName);

						logger.LogInformation(
							"Created the {WorkspaceName} workspace. Workspace Artifact ID: {WorkspaceId}.",
							parameters.WorkspaceName,
							parameters.WorkspaceId);
					});
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
				using (IRSAPIClient client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
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

		internal static void CreateWorkspaceFromTemplate(IntegrationTestParameters parameters, Relativity.Logging.ILog logger, string workspaceTemplateName, string newWorkspaceName)
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

			using (var client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
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

				parameters.WorkspaceId = workspaceId;
				parameters.WorkspaceName = workspaceName;

				EnableDataGrid(client, parameters, logger);
			}
		}

		internal static int RetrieveWorkspaceId(
			IntegrationTestParameters parameters,
			Relativity.Logging.ILog logger,
			string workspaceName)
		{
			using (var client = ServiceHelper.GetServiceProxy<IRSAPIClient>(parameters))
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

		private static void EnableDataGrid(IRSAPIClient client, IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			// By default DataGrid is disable on workspace templates used in tests
			// It is impossible to disable DataGrid for workspace if it was enabled before
			if (parameters.EnableDataGrid)
			{
				var createdWorkspace = client.Repositories.Workspace.ReadSingle(parameters.WorkspaceId);
				createdWorkspace.EnableDataGrid = true;
				client.Repositories.Workspace.UpdateSingle(createdWorkspace);
				logger.LogInformation($"DataGrid enabled for workspace with id {parameters.WorkspaceId}");

				UpdateExtractedTextField(parameters, logger).ConfigureAwait(false);
				logger.LogInformation($"'Extracted Text' field values in workspace updated");
			}
		}

		private static async Task UpdateExtractedTextField(IntegrationTestParameters parameters, Relativity.Logging.ILog logger)
		{
			var longTextFieldRequest = new LongTextFieldRequest()
				                           {
											   Name = WellKnownFields.ExtractedText,
											   ObjectType = new ObjectTypeIdentifier() { ArtifactTypeID = (int)ArtifactType.Document },
											   EnableDataGrid = true,
											   IncludeInTextIndex = false,
											   FilterType = FilterType.None,
											   AvailableInViewer = true,
											   HasUnicode = true,
				                           };
			using (IFieldManager fieldManager = ServiceHelper.GetServiceProxy<IFieldManager>(parameters))
			{
				int fieldId = TestFramework.RelativityHelpers.FieldHelper.GetFieldArtifactId(parameters, logger, WellKnownFields.ExtractedText);

				await fieldManager.UpdateLongTextFieldAsync(parameters.WorkspaceId, fieldId, longTextFieldRequest)
						.ConfigureAwait(false);
			}
		}
	}
}