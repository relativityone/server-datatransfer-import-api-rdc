// <copyright file="ResourceServerManager.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ObjectManagers
{
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Choice;
	using Relativity.Services.ResourceServer;

	internal class ResourceServerManager
	{
		private readonly ServiceProxyFactory serviceProxyFactory;

		public ResourceServerManager(ServiceProxyFactory serviceProxyFactory)
		{
			this.serviceProxyFactory = serviceProxyFactory;
		}

		public async Task<IEnumerable<ResourceServerTypeRef>> GetResourceServerTypesAsync()
		{
			using (var resourceServerManager = serviceProxyFactory.CreateServiceProxy<IResourceServerManager>())
			{
				return await resourceServerManager.RetrieveAllServerTypesAsync().ConfigureAwait(false);
			}
		}

		public async Task<ChoiceRef> GetServerStatusChoiceByName(string name)
		{
			using (var resourceServerManager = serviceProxyFactory.CreateServiceProxy<IResourceServerManager>())
			{
				var choices = await resourceServerManager.RetrieveAllServerStatusChoicesAsync().ConfigureAwait(false);
				ChoiceRef choice = choices.SingleOrDefault(x => x.Name == name);
				return choice;
			}
		}
	}
}