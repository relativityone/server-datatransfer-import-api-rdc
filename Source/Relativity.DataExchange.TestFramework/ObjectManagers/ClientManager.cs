// <copyright file="ClientManager.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ObjectManagers
{
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Client;

	internal class ClientManager
	{
		private readonly ServiceProxyFactory serviceProxyFactory;

		public ClientManager(ServiceProxyFactory serviceProxyFactory)
		{
			this.serviceProxyFactory = serviceProxyFactory;
		}

		public async Task<Client> ReadAsync(string name)
		{
			using (var clientManager = serviceProxyFactory.CreateServiceProxy<IClientManager>())
			{
				var query = new Services.Query { Condition = $"('Name' == '{name}')" };

				ClientQueryResultSet result = await clientManager.QueryAsync(query, 1).ConfigureAwait(false);
				return result.Results.FirstOrDefault()?.Artifact;
			}
		}
	}
}