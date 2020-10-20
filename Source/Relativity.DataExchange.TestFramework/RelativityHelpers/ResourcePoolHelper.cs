// <copyright file="ResourcePoolHelper.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.RelativityHelpers
{
	using System.Collections.Generic;
	using System.Configuration;
	using System.Linq;
	using System.Threading.Tasks;

	using kCura.Relativity.Client;
	using kCura.Relativity.Client.DTOs;

	using Relativity.DataExchange.TestFramework.ObjectManagers;
	using Relativity.Services.Choice;
	using Relativity.Services.ResourceServer;

	using Client = Relativity.Services.Client.Client;
	using FileShareResourceServer = Relativity.Services.ResourceServer.FileShareResourceServer;
	using ResourcePool = Relativity.Services.ResourcePool.ResourcePool;

	public class ResourcePoolHelper
	{
		private const string ResourcePoolName = "ResourcePoolForExportTests";

		private const string FileShareName = "FileShareForExportTests";

		private readonly ClientManager clientManager;

		private readonly string fileSharePath = ConfigurationManager.AppSettings["AdditionalFileShareUncPath"];

		private readonly FileShareServerManager fileShareServerManager;

		private readonly ResourcePoolManager resourcePoolManager;

		private readonly ResourceServerManager resourceServerManager;

		private readonly IntegrationTestParameters testParameters;

		public ResourcePoolHelper(IntegrationTestParameters testParameters)
		{
			this.testParameters = testParameters;
			var serviceProxyFactory = new ServiceProxyFactory(testParameters);
			resourceServerManager = new ResourceServerManager(serviceProxyFactory);
			resourcePoolManager = new ResourcePoolManager(resourceServerManager, serviceProxyFactory);
			fileShareServerManager = new FileShareServerManager(serviceProxyFactory);
			clientManager = new ClientManager(serviceProxyFactory);
		}

		public async Task CreateResourcePoolWithFileShareAsync()
		{
			if (!await ResourcePoolExistsAsync().ConfigureAwait(false))
			{
				await CreateResourcePoolWithDefaultServersAsync().ConfigureAwait(false);
				await CreateFileShareAsync().ConfigureAwait(false);
				await AddFileShareToResourcePool().ConfigureAwait(false);
			}

			await SetResourcePoolToWorkspace().ConfigureAwait(false);
		}

		private async Task SetResourcePoolToWorkspace()
		{
			ResourcePool resourcePool = await resourcePoolManager.ReadAsync(ResourcePoolName).ConfigureAwait(false);
			IEnumerable<ResourceServerRef> servers = (await resourcePoolManager
				                                          .GetResourceServersAsync(resourcePool.ArtifactID)
				                                          .ConfigureAwait(false)).ToList();

			var fileShareServer = servers.First(
				x => x.Name == FileShareName && x.ServerType.Name == ResourceServerTypeName.FileShare);

			var sqlServer = servers.First(x => x.ServerType.Name == ResourceServerTypeName.SqlServer);

			var cacheLocationServer = servers.First(
				x => x.ServerType.Name == ResourceServerTypeName.CacheLocationServer);

			UpdateWorkspace(resourcePool.ArtifactID, fileShareServer.ArtifactID, sqlServer.ArtifactID, cacheLocationServer.ArtifactID);
		}

		private void UpdateWorkspace(int resourcePoolId, int fileShareId, int sqlServerId, int cacheLocationServerId)
		{
#pragma warning disable CS0618 // Type or member is obsolete
			using (IRSAPIClient client = ServiceHelper.GetRSAPIServiceProxy<IRSAPIClient>(testParameters))
#pragma warning restore CS0618 // Type or member is obsolete
			{
				var artifactIdCondition = new WholeNumberCondition(ArtifactQueryFieldNames.ArtifactID, NumericConditionEnum.EqualTo, testParameters.WorkspaceId);
				var query = new Query<kCura.Relativity.Client.DTOs.Workspace>
					            {
						            Condition = artifactIdCondition,
						            Fields = FieldValue.AllFields,
					            };

				kCura.Relativity.Client.DTOs.Workspace workspace = client.Repositories.Workspace.Query(query).Results.First().Artifact;

				workspace.ResourcePoolID = resourcePoolId;
				var fileShareChoice = new kCura.Relativity.Client.DTOs.Choice(fileShareId);
				workspace.DefaultFileLocation = fileShareChoice;
				workspace.ServerID = sqlServerId;
				workspace.DefaultCacheLocation = cacheLocationServerId;
				workspace.DefaultDataGridLocation = fileShareChoice;

				client.Repositories.Workspace.UpdateSingle(workspace);
			}
		}

		private async Task<bool> ResourcePoolExistsAsync()
		{
			var resourcePool = await resourcePoolManager.ReadAsync(ResourcePoolName).ConfigureAwait(false);
			return resourcePool != null;
		}

		private async Task<int> CreateResourcePoolWithDefaultServersAsync()
		{
			Client relativityClient = await clientManager.ReadAsync("Relativity").ConfigureAwait(false);
			int resourcePoolId = await resourcePoolManager.CreateAsync(ResourcePoolName, relativityClient.ArtifactID)
				                     .ConfigureAwait(false);
			await resourcePoolManager.AddDefaultResourceServersToPoolAsync(resourcePoolId).ConfigureAwait(false);
			return resourcePoolId;
		}

		private async Task<int> CreateFileShareAsync()
		{
			ChoiceRef activeStatus =
				await resourceServerManager.GetServerStatusChoiceByName("Active").ConfigureAwait(false);
			return await fileShareServerManager.CreateAsync(FileShareName, fileSharePath, activeStatus)
				       .ConfigureAwait(false);
		}

		private async Task AddFileShareToResourcePool()
		{
			ResourcePool resourcePool = await resourcePoolManager.ReadAsync(ResourcePoolName).ConfigureAwait(false);
			FileShareResourceServer fileShareServer =
				await fileShareServerManager.ReadAsync(FileShareName).ConfigureAwait(false);
			await resourcePoolManager.AddFileShareToPoolAsync(resourcePool.ArtifactID, fileShareServer.ArtifactID)
				.ConfigureAwait(false);
		}
	}
}
