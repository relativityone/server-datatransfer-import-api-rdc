// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiObjectService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class to create Transfer API objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Resources;
	using Relativity.DataExchange.Service;
	using Relativity.Logging;

	/// <summary>
	/// Represents a class object to provide Transfer API object services to the transfer bridges. This class cannot be inherited.
	/// </summary>
	internal sealed class TapiObjectService : ITapiObjectService
	{
		/// <summary>
		/// The singleton instance.
		/// </summary>
		private static readonly Relativity.Transfer.IFileSystemService Instance = new Relativity.Transfer.FileSystemService();

		/// <inheritdoc />
		public Relativity.Transfer.IFileSystemService CreateFileSystemService()
		{
			return Instance;
		}

		/// <inheritdoc />
		public string BuildFileTransferModeDocText(bool includeBulk)
		{
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.AppendLine("FILE TRANSFER MODES:");
			sb.Append(BuildDocText());
			sb.AppendLine();
			sb.AppendLine();
			if (includeBulk)
			{
				sb.AppendLine("SQL INSERT MODES:");
				sb.AppendLine(" • Bulk • ");
				sb.Append("The upload process has access to the SQL share on the appropriate case database.  This ensures the fastest transfer of information between the desktop client and the relativity servers.");
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine(" • Single •");
				sb.Append("The upload process has NO access to the SQL share on the appropriate case database.  This is a slower method of import. If the process is using single mode, contact your Relativity Database Administrator to see if a SQL share can be opened for the desired case.");
			}

			return sb.ToString();
		}

		/// <summary>
		/// Creates a Relativity connection information object.
		/// </summary>
		/// <param name="parameters">
		/// The Transfer API bridge parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Transfer.RelativityConnectionInfo"/> instance.
		/// </returns>
		public Relativity.Transfer.RelativityConnectionInfo CreateRelativityConnectionInfo(TapiBridgeParameters2 parameters)
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

			Relativity.Transfer.IHttpCredential httpCredential;
			if (string.Compare(parameters.Credentials.UserName, Relativity.Transfer.BearerTokenCredential.OAuth2UserName, StringComparison.OrdinalIgnoreCase) == 0)
			{
				httpCredential = new Relativity.Transfer.BearerTokenCredential(parameters.Credentials.Password);
			}
			else
			{
				httpCredential = new Relativity.Transfer.BasicAuthenticationCredential(parameters.Credentials.UserName, parameters.Credentials.Password);
			}

			// REL-281370: Due to high SOI, this method takes on more responsibility
			//             than it should but it limits the URL fetch to a single method.
			RelativityInstanceInfo instanceInfo = new RelativityInstanceInfo
				                                      {
					                                      CookieContainer = parameters.WebCookieContainer,
					                                      Credentials = parameters.Credentials,
					                                      WebApiServiceUrl = new Uri(parameters.WebServiceUrl),
				                                      };

			RelativityManagerService service = new RelativityManagerService(instanceInfo);
			Uri relativityUrl = service.GetRelativityUrl();

			// REL-286484: There are several expectations on a normalized URL - especially extracted text downloads.
			var host = new Uri(relativityUrl.GetLeftPart(UriPartial.Authority));
			return new Relativity.Transfer.RelativityConnectionInfo(
				host,
				httpCredential,
				parameters.WorkspaceId,
				new Uri(parameters.WebServiceUrl));
		}

		/// <inheritdoc />
		public Relativity.Transfer.IRelativityTransferHost CreateRelativityTransferHost(Relativity.Transfer.RelativityConnectionInfo connectionInfo, Relativity.Transfer.ITransferLog log)
		{
			return new Relativity.Transfer.RelativityTransferHost(connectionInfo, log);
		}

		/// <inheritdoc />
		public string GetClientDisplayName(Guid clientId)
		{
			if (clientId == Guid.Empty)
			{
				throw new ArgumentException("The client unique identifier must be non-empty.", nameof(clientId));
			}

			using (var transferLog = new RelativityTransferLog())
			{
				foreach (var clientMetadata in Relativity.Transfer.TransferClientHelper.SearchAvailableClients(transferLog))
				{
					if (new Guid(clientMetadata.Id) == clientId)
					{
						return clientMetadata.DisplayName;
					}
				}

				throw new ArgumentException(Strings.ClientIdNotFoundExceptionMessage);
			}
		}

		/// <inheritdoc />
		public Guid GetClientId(TapiBridgeParameters2 parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			var clientId = Guid.Empty;
			if (parameters.ForceAsperaClient)
			{
				clientId = new Guid(Relativity.Transfer.TransferClientConstants.AsperaClientId);
			}
			else if (parameters.ForceHttpClient)
			{
				clientId = new Guid(Relativity.Transfer.TransferClientConstants.HttpClientId);
			}
			else if (parameters.ForceFileShareClient)
			{
				clientId = new Guid(Relativity.Transfer.TransferClientConstants.FileShareClientId);
			}

			return clientId;
		}

		/// <inheritdoc />
		public TapiClient GetTapiClient(Guid clientId)
		{
			switch (clientId.ToString("D").ToUpperInvariant())
			{
				case Relativity.Transfer.TransferClientConstants.AsperaClientId:
					return TapiClient.Aspera;

				case Relativity.Transfer.TransferClientConstants.FileShareClientId:
					return TapiClient.Direct;

				case Relativity.Transfer.TransferClientConstants.HttpClientId:
					return TapiClient.Web;

				default:
					return TapiClient.None;
			}
		}

		/// <inheritdoc />
		public async Task<string> GetWorkspaceClientDisplayNameAsync(TapiBridgeParameters2 parameters)
		{
			Relativity.Transfer.ITransferClient transferClient = await this.GetWorkspaceClientAsync(parameters).ConfigureAwait(false);
			return transferClient.DisplayName;
		}

		/// <inheritdoc />
		public async Task<Guid> GetWorkspaceClientIdAsync(TapiBridgeParameters2 parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			Relativity.Transfer.ITransferClient transferClient = await this.GetWorkspaceClientAsync(parameters).ConfigureAwait(false);
			return transferClient.Id;
		}

		/// <inheritdoc />
		public void SetTapiClient(TapiBridgeParameters2 parameters, TapiClient targetClient)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			switch (targetClient)
			{
				case TapiClient.Aspera:
					parameters.ForceAsperaClient = true;
					break;

				case TapiClient.Direct:
					parameters.ForceFileShareClient = true;
					break;

				case TapiClient.Web:
					parameters.ForceHttpClient = true;
					break;
			}
		}

		/// <summary>
		/// Searches for all available clients and builds the documentation text from the discovered metadata.
		/// </summary>
		/// <returns>
		/// The documentation text.
		/// </returns>
		private static string BuildDocText()
		{
			using (var transferLog = new RelativityTransferLog())
			{
				var sb = new StringBuilder();
				foreach (var clientMetadata in Relativity.Transfer.TransferClientHelper.SearchAvailableClients(transferLog)
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
		/// Asynchronously gets the Transfer API client that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Relativity.Transfer.ITransferClient"/> instance.
		/// </returns>
		private async Task<Relativity.Transfer.ITransferClient> GetWorkspaceClientAsync(TapiBridgeParameters2 parameters)
		{
			var configuration = new Relativity.Transfer.ClientConfiguration
			{
				CookieContainer = parameters.WebCookieContainer,
				ClientId = this.GetClientId(parameters),
			};

			try
			{
				var connectionInfo = this.CreateRelativityConnectionInfo(parameters);
				using (var transferLog = new RelativityTransferLog())
				using (var transferHost = new Relativity.Transfer.RelativityTransferHost(connectionInfo, transferLog))
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
																	 ? new Relativity.Transfer.TransferClientStrategy()
																	 : new Relativity.Transfer.TransferClientStrategy(parameters.ForceClientCandidates);
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
								"Unexpected error occurred inside Transfer API layer. Exception: " + ex, new LoggerOptions() { System = "WinEDDS" });
				throw;
			}
		}
	}
}