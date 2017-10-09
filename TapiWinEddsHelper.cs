// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiWinEddsHelper.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using kCura.WinEDDS.TApi.Resources;

	using Relativity.Transfer;

	/// <summary>
	/// Defines helper methods to provide WinEDDS compatibility functionality.
	/// </summary>
	public static class TapiWinEddsHelper
	{
		/// <summary>
		/// Searches for all available clients and builds the documentation text from the discovered metadata.
		/// </summary>
		/// <returns>
		/// The documentation text.
		/// </returns>
		public static string BuildDocText()
		{
			var sb = new StringBuilder();
			foreach (var clientMetadata in TransferClientHelper.SearchAvailableClients().OrderBy(x => x.DisplayName))
			{
				if (sb.Length > 0)
				{
					sb.AppendLine();
					sb.AppendLine();
				}

				sb.AppendFormat(" • {0} • ", clientMetadata.DisplayName);
				sb.AppendLine();
				sb.Append(clientMetadata.Description);
			}

			return sb.ToString();
		}

		/// <summary>
		/// Gets the client display name associated with the specified transfer client identifier.
		/// </summary>
		/// <param name="clientId">
		/// The transfer client identifier.
		/// </param>
		/// <returns>
		/// The client display name.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// Thrown when the client doesn't exist.
		/// </exception>
		public static string GetClientDisplayName(Guid clientId)
		{
			if (clientId == Guid.Empty)
			{
				throw new ArgumentException("The client unique identifier must be non-empty.", nameof(clientId));
			}

			foreach (var clientMetadata in TransferClientHelper.SearchAvailableClients())
			{
				if (new Guid(clientMetadata.Id) == clientId)
				{
					return clientMetadata.DisplayName;
				}
			}

			throw new ArgumentException(Strings.ClientIdNotFoundExceptionMessage);
		}

		/// <summary>
		/// Creates a Relativity connection information object.
		/// </summary>
		/// <param name="parameters">
		/// The TAPI bridge parameters.
		/// </param>
		/// <returns>
		/// The <see cref="RelativityConnectionInfo"/> instance.
		/// </returns>
		public static RelativityConnectionInfo CreateRelativityConnectionInfo(TapiBridgeParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			return CreateRelativityConnectionInfo(
				parameters.WebServiceUrl,
				parameters.WorkspaceId,
				parameters.Credentials.UserName,
				parameters.Credentials.Password);
		}

		/// <summary>
		/// Creates a Relativity connection information object.
		/// </summary>
		/// <param name="webServiceUrl">
		/// The Relativity web service URL.
		/// </param>
		/// <param name="workspaceId">
		/// The workspace artifact identifier.
		/// </param>
		/// <param name="userName">
		/// The Relativity user name.
		/// </param>
		/// <param name="password">
		/// The Relativity password.
		/// </param>
		/// <returns>
		/// The <see cref="RelativityConnectionInfo"/> instance.
		/// </returns>
		public static RelativityConnectionInfo CreateRelativityConnectionInfo(
			string webServiceUrl,
			int workspaceId,
			string userName,
			string password)
		{
			// Note: this is a temporary workaround to support integration tests.
			var baseUri = new Uri(webServiceUrl);
			var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
			return string.Compare(userName, "XxX_BearerTokenCredentials_XxX", StringComparison.OrdinalIgnoreCase) == 0
					   ? new RelativityConnectionInfo(host, new BearerTokenCredential(password), workspaceId)
					   : new RelativityConnectionInfo(
						   host,
						   new BasicAuthenticationCredential(userName, password),
						   workspaceId);
		}

		/// <summary>
		/// Gets the client identifier.
		/// </summary>
		/// <param name="parameters">
		/// The parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/> value.
		/// </returns>
		public static Guid GetClientId(TapiBridgeParameters parameters)
		{
			var clientId = Guid.Empty;
			if (parameters.ForceAsperaClient)
			{
				clientId = new Guid(TransferClientConstants.AsperaClientId);
			}
			else if (parameters.ForceHttpClient)
			{
				clientId = new Guid(TransferClientConstants.HttpClientId);
			}
			else if (parameters.ForceFileShareClient)
			{
				clientId = new Guid(TransferClientConstants.FileShareClientId);
			}

			return clientId;
		}

		/// <summary>
		/// Asynchronously gets the TAPI client display name that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The client display name.
		/// </returns>
		public static async Task<string> GetWorkspaceClientDisplayNameAsync(TapiBridgeParameters parameters)
		{
			ITransferClient transferClient = await GetWorkspaceClientAsync(parameters);
			return transferClient.DisplayName;
		}

		/// <summary>
		/// Asynchronously gets the TAPI client Id name that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The client display name.
		/// </returns>
		public static async Task<Guid> GetWorkspaceClientIdAsync(TapiBridgeParameters parameters)
		{
			ITransferClient transferClient = await GetWorkspaceClientAsync(parameters);
			return transferClient.Id;
		}

		private static async Task<ITransferClient> GetWorkspaceClientAsync(TapiBridgeParameters parameters)
		{
			var configuration = new ClientConfiguration
			{
				CookieContainer = parameters.WebCookieContainer,
				ClientId = GetClientId(parameters)
			};
			try
			{
				var connectionInfo = CreateRelativityConnectionInfo(parameters);
				using (var transferHost = new RelativityTransferHost(connectionInfo))
				{
					if (configuration.ClientId != Guid.Empty)
					{
						using (var client = transferHost.CreateClient(configuration))
						{
							var supportCheck = await client.SupportCheckAsync();
							if (supportCheck.IsSupported)
							{
								return client;
							}
						}
					}

					var clientStrategy = string.IsNullOrEmpty(parameters.ForceClientCandidates)
											 ? new TransferClientStrategy()
											 : new TransferClientStrategy(parameters.ForceClientCandidates);
					using (var forcedClient = await transferHost.CreateClientAsync(configuration, clientStrategy)
												  .ConfigureAwait(false))
					{
						return forcedClient;
					}
				}
			}
			catch (Exception ex)
			{
				Relativity.Logging.Tools.InternalLogger.WriteTokCuraEventLog(
						"Unexpected error occurred inside TAPI layer. Exception: " + ex, "WinEDDS");
				throw;
			}
		}
	}
}