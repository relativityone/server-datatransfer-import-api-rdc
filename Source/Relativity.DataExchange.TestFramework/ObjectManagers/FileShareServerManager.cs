// <copyright file="FileShareServerManager.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework.ObjectManagers
{
	using System.Linq;
	using System.Threading.Tasks;

	using Relativity.Services.Choice;
	using Relativity.Services.ResourceServer;

	internal class FileShareServerManager
	{
		private readonly ServiceProxyFactory serviceProxyFactory;

		public FileShareServerManager(ServiceProxyFactory serviceProxyFactory)
		{
			this.serviceProxyFactory = serviceProxyFactory;
		}

		public async Task<int> CreateAsync(string name, string uncPath, ChoiceRef status)
		{
			using (var fileShareServerManager = serviceProxyFactory.CreateServiceProxy<IFileShareServerManager>())
			{
				var serverToCreate = new FileShareResourceServer
				{
					Name = name,
					Status = status,
					UNCPath = uncPath,
				};

				return await fileShareServerManager.CreateSingleAsync(serverToCreate).ConfigureAwait(false);
			}
		}

		public async Task<FileShareResourceServer> ReadAsync(string name)
		{
			using (var fileShareServerManager = serviceProxyFactory.CreateServiceProxy<IFileShareServerManager>())
			{
				var query = new Services.Query { Condition = $"('Name' == '{name}')" };
				var resultSet = await fileShareServerManager.QueryAsync(query).ConfigureAwait(false);
				return resultSet.Results.FirstOrDefault()?.Artifact;
			}
		}

		public async Task<FileShareResourceServer> ReadAsync(int artifactId)
		{
			using (var fileShareServerManager = serviceProxyFactory.CreateServiceProxy<IFileShareServerManager>())
			{
				var query = new Services.Query
					            {
						            Condition = $"('ArtifactID' == '{artifactId}')",
					            };
				var resultSet = await fileShareServerManager.QueryAsync(query).ConfigureAwait(false);
				return resultSet.Results.FirstOrDefault()?.Artifact;
			}
		}

		public async Task<bool> DeleteAsync(string name)
		{
			var fileShare = await ReadAsync(name).ConfigureAwait(false);
			if (fileShare == null)
			{
				return false;
			}

			using (var resourceServerManager = serviceProxyFactory.CreateServiceProxy<IResourceServerManager>())
			{
				await resourceServerManager.DeleteSingleAsync(
					new ResourceServer() { ArtifactID = fileShare.ArtifactID }).ConfigureAwait(false);
				return true;
			}
		}
	}
}