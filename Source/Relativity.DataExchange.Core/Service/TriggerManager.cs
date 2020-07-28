// ----------------------------------------------------------------------------
// <copyright file="TriggerManager.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;

	using Relativity.AutomatedWorkflows.Services.Interfaces;
	using Relativity.AutomatedWorkflows.Services.Interfaces.DataContracts.Triggers;
	using Relativity.Logging;
	using Relativity.Services.ServiceProxy;

	/// <summary>
	/// Contains methods for triggering the automated workflow.
	/// </summary>
	public class TriggerManager
	{
		private readonly IServiceFactory _serviceFactory;
		private readonly ILog _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="TriggerManager"/> class.
		/// </summary>
		/// <param name="logger">Logger.</param>
		/// <param name="sf">Service factory for using the proxy.</param>
		public TriggerManager(ILog logger, IServiceFactory sf)
		{
			this._logger = logger;
			this._serviceFactory = sf;
		}

		/// <summary>
		/// Checks if Relativity version supports the automated workflow.
		/// </summary>
		/// <param name="workspaceId">Workspace ID.</param>
		/// <param name="hasErrors">Checks whether the import has completed with errors or without them.</param>
		/// <param name="relativityVersion">Current Relativity version.</param>
		/// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
		public async Task AttemptSendingTriggerAsync(int workspaceId, bool hasErrors, Version relativityVersion)
		{
			var firstSupportedRelativityVersion = new Version(11, 2, 151); // This is the RAW test Relativity env version where the triggers are implemented.

			if (relativityVersion >= firstSupportedRelativityVersion)
			{
				const string TriggerName = "relativity@on-new-documents-added";
				const string StateWithErrors = "complete-with-errors";
				const string StateWithoutErrors = "complete";

				await this.SendTriggerAsync(workspaceId, TriggerName, hasErrors ? StateWithErrors : StateWithoutErrors).ConfigureAwait(false);
			}
			else
			{
				this._logger.LogInformation(
					"Automated Workflow is not supported on this version of Relativity. First supported Relativity version is: {0}",
					firstSupportedRelativityVersion);
			}
		}

		private async Task SendTriggerAsync(int workSpaceId, string triggerName, string state)
		{
			try
			{
				this._logger.LogInformation("For workspace artifact ID : {0} {1} trigger called with status {2}.", workSpaceId, triggerName, state);
				SendTriggerBody body = new SendTriggerBody()
				{
					Inputs = new List<TriggerInput>
															{
																new TriggerInput()
																	{
																		ID = "type",
																		Value = "rdc",
																	},
															},
					State = state,
				};
				using (IAutomatedWorkflowsService triggerProcessor = this._serviceFactory.CreateProxy<IAutomatedWorkflowsService>())
				{
					await triggerProcessor.SendTriggerAsync(workSpaceId, triggerName, body).ConfigureAwait(false);
					this._logger.LogInformation("Execution of trigger '{0}' in Workspace Id: {1} finished successfully.", triggerName, workSpaceId);
				}
			}
			catch (Exception ex)
			{
				const string Message = "Execution of trigger : '{0}' in Workspace Id: {1} failed. Most likely Workspace is not configured with Relativity Workflow Automation.";
				this._logger.LogWarning(ex, Message, triggerName, workSpaceId);
			}
		}
	}
}