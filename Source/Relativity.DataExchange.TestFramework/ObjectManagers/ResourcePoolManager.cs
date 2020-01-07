// <copyright file="ResourcePoolManager.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ObjectManagers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Client;
	using Relativity.Services.ResourcePool;
	using Relativity.Services.ResourceServer;

	internal class ResourcePoolManager
	{
		private readonly ResourceServerManager resourceServerManager;
		private readonly ServiceProxyFactory serviceProxyFactory;

		public ResourcePoolManager(ResourceServerManager resourceServerManager, ServiceProxyFactory serviceProxyFactory)
		{
			this.serviceProxyFactory = serviceProxyFactory;
			this.resourceServerManager = resourceServerManager;
		}

		public async Task<int> CreateAsync(string name, int clientId)
		{
			var client = new ClientRef(clientId);
			var resourcePool = new ResourcePool { Name = name, Client = client };

			using (var resourcePoolManager = serviceProxyFactory.CreateServiceProxy<IResourcePoolManager>())
			{
				return await resourcePoolManager.CreateSingleAsync(resourcePool).ConfigureAwait(false);
			}
		}

		public async Task<ResourcePool> ReadAsync(string name)
		{
			using (var resourcePoolManager = serviceProxyFactory.CreateServiceProxy<IResourcePoolManager>())
			{
				return await ReadAsync(resourcePoolManager, name).ConfigureAwait(false);
			}
		}

		public async Task<IEnumerable<ResourceServerRef>> GetResourceServersAsync(int resourcePoolId)
		{
			using (var resourcePoolManager = serviceProxyFactory.CreateServiceProxy<IResourcePoolManager>())
			{
				return await resourcePoolManager
					       .RetrieveResourceServersAsync(new ResourcePoolRef(resourcePoolId))
					       .ConfigureAwait(false);
			}
		}

		public async Task AddDefaultResourceServersToPoolAsync(int resourcePoolId)
		{
			using (var resourcePoolManager = serviceProxyFactory.CreateServiceProxy<IResourcePoolManager>())
			{
				var resourcePool = new ResourcePoolRef { ArtifactID = resourcePoolId };
				var defaultServers = await GetDefaultResourceServersAsync(resourcePoolManager).ConfigureAwait(false);

				foreach (ResourceServerRef resourceServer in defaultServers)
				{
					await resourcePoolManager.AddServerAsync(resourceServer, resourcePool).ConfigureAwait(false);
				}
			}
		}

		public Task AddFileShareToPoolAsync(int resourcePoolId, int fileShareServerId)
		{
			return AddServerAsync(resourcePoolId, fileShareServerId, ResourceServerTypeName.FileShare);
		}

		private static async Task<ResourcePool> ReadAsync(IResourcePoolManager resourcePoolManager, string name)
		{
			var query = new Services.Query { Condition = $"('Name' == '{name}')" };

			ResourcePoolQueryResultSet resourcePoolResultSet = await resourcePoolManager.QueryAsync(query).ConfigureAwait(false);
			return resourcePoolResultSet.Results.FirstOrDefault()?.Artifact;
		}

		private static async Task<IEnumerable<ResourceServerRef>> GetDefaultResourceServersAsync(IResourcePoolManager resourcePoolManager)
		{
			ResourcePool defaultResourcePool = await ReadAsync(resourcePoolManager, "Default").ConfigureAwait(false);
			return await resourcePoolManager
				       .RetrieveResourceServersAsync(new ResourcePoolRef(defaultResourcePool.ArtifactID))
				       .ConfigureAwait(false);
		}

		private async Task AddServerAsync(int resourcePoolId, int resourceServerId, string serverTypeName)
		{
			ResourceServerTypeRef serverType = await GetResourceServerTypeByName(serverTypeName).ConfigureAwait(false);

			using (var resourcePoolManager = serviceProxyFactory.CreateServiceProxy<IResourcePoolManager>())
			{
				await resourcePoolManager.AddServerAsync(
					new ResourceServerRef { ArtifactID = resourceServerId, ServerType = serverType },
					new ResourcePoolRef { ArtifactID = resourcePoolId }).ConfigureAwait(false);
			}
		}

		private async Task<ResourceServerTypeRef> GetResourceServerTypeByName(string name)
		{
			var resourceServerTypes = await resourceServerManager.GetResourceServerTypesAsync().ConfigureAwait(false);
			return resourceServerTypes.Single(x => x.Name == name);
		}
	}
}