// ----------------------------------------------------------------------------
// <copyright file="AgentHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Interfaces.Agent;
	using Relativity.Services.Interfaces.Agent.Models;
	using Relativity.Services.Objects;
	using Relativity.Services.Objects.DataContracts;

	public static class AgentHelper
	{
		private const int WorkspaceId = -1;

		public static async Task<bool> SetEnabledAsync(IntegrationTestParameters parameters, int artifactId, bool enabled)
		{
			AgentResponse originalState = await QueryAgentDataAsync(parameters, artifactId).ConfigureAwait(false);
			if (originalState.Enabled == enabled)
			{
				return false;
			}

			AgentRequest request = new AgentRequest(originalState)
			{
				Enabled = enabled,
			};

			await UpdateAgentAsync(parameters, artifactId, request).ConfigureAwait(false);
			return true;
		}

		public static async Task<int[]> QueryAgentsArtifactIdsByAgentTypeAsync(IntegrationTestParameters parameters, string agentType)
		{
			QueryRequest queryRequest = new QueryRequest
			{
				ObjectType = new ObjectTypeRef { ArtifactTypeID = (int)ArtifactType.Agent },
				Condition = $"'Agent Type' LIKE '{agentType}'",
			};

			using (var manager = ServiceHelper.GetServiceProxy<IObjectManager>(parameters))
			{
				var result = await manager.QueryAsync(WorkspaceId, queryRequest, 0, ServiceHelper.MaxItemsToFetch).ConfigureAwait(false);
				return result.Objects.Select(rdo => rdo.ArtifactID).ToArray();
			}
		}

		private static async Task<AgentResponse> QueryAgentDataAsync(IntegrationTestParameters parameters, int artifactId)
		{
			using (var manager = ServiceHelper.GetServiceProxy<IAgentManager>(parameters))
			{
				return await manager.ReadAsync(WorkspaceId, artifactId).ConfigureAwait(false);
			}
		}

		private static async Task UpdateAgentAsync(IntegrationTestParameters parameters, int artifactId, AgentRequest request)
		{
			using (var manager = ServiceHelper.GetServiceProxy<IAgentManager>(parameters))
			{
				await manager.UpdateAsync(WorkspaceId, artifactId, request).ConfigureAwait(false);
			}
		}
	}
}