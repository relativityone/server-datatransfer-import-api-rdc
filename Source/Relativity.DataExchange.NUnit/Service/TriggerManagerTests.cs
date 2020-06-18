// ----------------------------------------------------------------------------
// <copyright file="TriggerManagerTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Threading.Tasks;
	using global::NUnit.Framework;
	using Moq;

	using Relativity.AutomatedWorkflows.Services.Interfaces;
	using Relativity.AutomatedWorkflows.Services.Interfaces.DataContracts.Triggers;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Services.ServiceProxy;

	public class TriggerManagerTests
	{
		private const int WorkspaceId = 123456789;

		private TriggerManager triggerManager;
		private Mock<IServiceFactory> serviceFactory;
		private Mock<IAutomatedWorkflowsService> automatedWorkflowService;

		[SetUp]
		public void Setup()
		{
			this.serviceFactory = new Mock<IServiceFactory>();
			this.automatedWorkflowService = new Mock<IAutomatedWorkflowsService>();

			this.triggerManager = new TriggerManager(new TestNullLogger(), this.serviceFactory.Object);

			this.serviceFactory.Setup(
					x => x.CreateProxy<IAutomatedWorkflowsService>()).
				Returns(this.automatedWorkflowService.Object);

			this.automatedWorkflowService.Setup(
				x => x.SendTriggerAsync(
					It.IsAny<int>(),
					It.IsAny<string>(),
					It.IsAny<SendTriggerBody>())).
				Returns(Task.CompletedTask);
		}

		[Test]
		public async Task ShouldNotSendTriggerDueToUnsupportedRelativityVersionAsync()
		{
			var unsupportedRelativityVersion = new Version(9, 7, 3);

			await this.triggerManager
					.AttemptSendingTriggerAsync(
						WorkspaceId,
						true,
						unsupportedRelativityVersion).
					ConfigureAwait(false);

			this.automatedWorkflowService.Verify(
				x => x.SendTriggerAsync(
					It.IsAny<int>(),
					It.IsAny<string>(),
					It.IsAny<SendTriggerBody>()),
				Times.Never());
		}

		[TestCase(true)]
		[TestCase(false)]
		public async Task ShouldSendTriggerAsync(bool hasErrors)
		{
			const string TriggerName = "relativity@on-new-documents-added";
			var firstSupportedRelativityVersion = new Version(11, 3, 16); // This is the first supported version used for searchable PDFs

			await this.triggerManager
					.AttemptSendingTriggerAsync(
						WorkspaceId,
						hasErrors,
						firstSupportedRelativityVersion).
					ConfigureAwait(false);

			this.automatedWorkflowService.Verify(
				x => x.SendTriggerAsync(
				It.Is<int>(actualWorkspaceId => actualWorkspaceId.Equals(WorkspaceId)),
				It.Is<string>(actualTriggerName => actualTriggerName.Equals(TriggerName)),
				It.Is<SendTriggerBody>(triggerBody => this.CheckTriggerBody(triggerBody, hasErrors))),
				Times.Once());
		}

		private bool CheckTriggerBody(SendTriggerBody triggerBody, bool hasErrors)
		{
			const string StateWithErrors = "complete-with-errors";
			const string StateWithoutErrors = "complete";

			return triggerBody.State.Equals(hasErrors ? StateWithErrors : StateWithoutErrors);
		}
	}
}