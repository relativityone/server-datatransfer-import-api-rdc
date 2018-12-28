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
	using Relativity.Logging;
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
			using (var transferLog = new RelativityTransferLog())
			{
				var sb = new StringBuilder();
				foreach (var clientMetadata in TransferClientHelper.SearchAvailableClients(transferLog)
					.OrderBy(x => x.DisplayName))
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

			using (var transferLog = new RelativityTransferLog())
			{
				foreach (var clientMetadata in TransferClientHelper.SearchAvailableClients(transferLog))
				{
					if (new Guid(clientMetadata.Id) == clientId)
					{
						return clientMetadata.DisplayName;
					}
				}

				throw new ArgumentException(Strings.ClientIdNotFoundExceptionMessage);
			}
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
					
			if (string.IsNullOrEmpty(parameters.WebServiceUrl))
			{
				throw new ArgumentException("The WebServiceUrl must be non-null or empty.", nameof(parameters));
			}

			if (parameters.WorkspaceId < 1)
			{
				throw new ArgumentException("The WorkspaceId must be non-zero.", nameof(parameters));
			}

			if (parameters.Credentials == null)
			{
				throw new ArgumentException("The Credentials must be non-null.", nameof(parameters));
			}

			if (parameters.WebCookieContainer == null)
			{
				throw new ArgumentException("The WebCookieContainer must be non-null.", nameof(parameters));
			}

			IHttpCredential httpCredential;
			if (string.Compare(parameters.Credentials.UserName, BearerTokenCredential.OAuth2UserName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				httpCredential = new BearerTokenCredential(parameters.Credentials.Password);
			}
			else
			{
				httpCredential = new BasicAuthenticationCredential(parameters.Credentials.UserName, parameters.Credentials.Password);
			}

			// REL-281370: Due to high SOI, this method takes on more responsibility
			//             than it should but it limits the URL fetch to a single method.
			RelativityManagerService service = new RelativityManagerService(parameters);
			Uri relativityUrl = service.GetRelativityUrl();
			return new RelativityConnectionInfo(
				relativityUrl,
				httpCredential,
				parameters.WorkspaceId,
				new Uri(parameters.WebServiceUrl));
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
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

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
			ITransferClient transferClient = await GetWorkspaceClientAsync(parameters).ConfigureAwait(false);
			return transferClient.DisplayName;
		}

		/// <summary>
		/// Asynchronously gets the TAPI client Id that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The client display name.
		/// </returns>
		public static async Task<Guid> GetWorkspaceClientIdAsync(TapiBridgeParameters parameters)
		{
			ITransferClient transferClient = await GetWorkspaceClientAsync(parameters).ConfigureAwait(false);
			return transferClient.Id;
		}

		/// <summary>
		/// Asynchronously gets the TAPI client that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The <see cref="ITransferClient"/> instance.
		/// </returns>
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
				using (var transferLog = new RelativityTransferLog())
				using (var transferHost = new RelativityTransferHost(connectionInfo, transferLog))
				{
					if (configuration.ClientId != Guid.Empty)
					{
						using (var client = transferHost.CreateClient(configuration))
						{
							var supportCheck = await client.SupportCheckAsync().ConfigureAwait(false);
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
				Relativity.Logging.Tools.InternalLogger.WriteFromExternal(
								"Unexpected error occurred inside TAPI layer. Exception: " + ex, new LoggerOptions() { System = "WinEDDS" });
				throw;
			}
		}
	}
}