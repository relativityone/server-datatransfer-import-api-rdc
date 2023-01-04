// ----------------------------------------------------------------------------
// <copyright file="LoadTestsAssemblySetup.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit.LoadTests
{
	using System.Linq;
	using System.Threading.Tasks;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Import.NUnit.Integration;
	using Relativity.DataExchange.TestFramework;
	using Relativity.DataExchange.TestFramework.RelativityHelpers;

	[SetUpFixture]
	public class LoadTestsAssemblySetup
	{
		private readonly global::System.Collections.Generic.List<int> disabledAgentsArtifactIds = new global::System.Collections.Generic.List<int>();

		[OneTimeSetUp]
		public async Task SetupAsync()
		{
			await this.DisableAuditAgents().ConfigureAwait(false);
			await AssemblySetup.SetupAsync().ConfigureAwait(false);
		}

		[OneTimeTearDown]
		public async Task TearDownAsync()
		{
			await this.RestoreAgentsToOriginalStateAsync().ConfigureAwait(false);
			AssemblySetup.TearDown();
		}

		private async Task DisableAuditAgents()
		{
			const string AuditAgentsType = "Data Grid Audit%";
			int[] auditAgentsArtifactIds = await AgentHelper
												.QueryAgentsArtifactIdsByAgentTypeAsync(
													IntegrationTestHelper.IntegrationTestParameters,
													AuditAgentsType).ConfigureAwait(false);

			async Task DisableAgentAsync(int artifactId)
			{
				if (await AgentHelper.SetEnabledAsync(IntegrationTestHelper.IntegrationTestParameters, artifactId, false)
						.ConfigureAwait(false))
				{
					this.disabledAgentsArtifactIds.Add(artifactId);
				}
			}

			var disableAgentsTasks = auditAgentsArtifactIds.Select(DisableAgentAsync);
			await Task.WhenAll(disableAgentsTasks).ConfigureAwait(false);
		}

		private async Task RestoreAgentsToOriginalStateAsync()
		{
			var enableAgentsTasks = this.disabledAgentsArtifactIds.Select(
				id => AgentHelper.SetEnabledAsync(IntegrationTestHelper.IntegrationTestParameters, id, true));
			await Task.WhenAll(enableAgentsTasks).ConfigureAwait(false);
		}
	}
}