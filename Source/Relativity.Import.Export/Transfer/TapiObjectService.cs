﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiObjectService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class to create Transfer API objects.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;
	using System.Linq;
	using System.Text;
	using System.Threading.Tasks;

	using Relativity.Import.Export.Resources;
	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Represents a class object to provide Transfer API object services to the transfer bridges.
	/// </summary>
	public sealed class TapiObjectService : ITapiObjectService
	{
		/// <summary>
		/// The singleton instance.
		/// </summary>
		private static readonly IFileSystemService Instance = new FileSystemService();

		/// <inheritdoc />
		public IFileSystemService CreateFileSystemService()
		{
			return Instance;
		}

		/// <inheritdoc />
		public string BuildDocText()
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
		/// Creates a Relativity connection information object.
		/// </summary>
		/// <param name="parameters">
		/// The Transfer API bridge parameters.
		/// </param>
		/// <returns>
		/// The <see cref="RelativityConnectionInfo"/> instance.
		/// </returns>
		public RelativityConnectionInfo CreateRelativityConnectionInfo(TapiBridgeParameters parameters)
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

			// REL-286484: There are several expectations on a normalized URL - especially extracted text downloads.
			var host = new Uri(relativityUrl.GetLeftPart(UriPartial.Authority));
			return new RelativityConnectionInfo(
				host,
				httpCredential,
				parameters.WorkspaceId,
				new Uri(parameters.WebServiceUrl));
		}

		/// <inheritdoc />
		public IRelativityTransferHost CreateRelativityTransferHost(RelativityConnectionInfo connectionInfo, ITransferLog log)
		{
			return new RelativityTransferHost(connectionInfo, log);
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

		/// <inheritdoc />
		public Guid GetClientId(TapiBridgeParameters parameters)
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

		/// <inheritdoc />
		public TapiClient GetTapiClient(Guid clientId)
		{
			switch (clientId.ToString("D").ToUpperInvariant())
			{
				case TransferClientConstants.AsperaClientId:
					return TapiClient.Aspera;

				case TransferClientConstants.FileShareClientId:
					return TapiClient.Direct;

				case TransferClientConstants.HttpClientId:
					return TapiClient.Web;

				default:
					return TapiClient.None;
			}
		}

		/// <inheritdoc />
		public async Task<string> GetWorkspaceClientDisplayNameAsync(TapiBridgeParameters parameters)
		{
			ITransferClient transferClient = await this.GetWorkspaceClientAsync(parameters).ConfigureAwait(false);
			return transferClient.DisplayName;
		}

		/// <inheritdoc />
		public async Task<Guid> GetWorkspaceClientIdAsync(TapiBridgeParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			ITransferClient transferClient = await this.GetWorkspaceClientAsync(parameters).ConfigureAwait(false);
			return transferClient.Id;
		}

		/// <inheritdoc />
		public void SetTapiClient(TapiBridgeParameters parameters, TapiClient targetClient)
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
		/// Asynchronously gets the Transfer API client that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The <see cref="ITransferClient"/> instance.
		/// </returns>
		private async Task<ITransferClient> GetWorkspaceClientAsync(TapiBridgeParameters parameters)
		{
			var configuration = new ClientConfiguration
			{
				CookieContainer = parameters.WebCookieContainer,
				ClientId = this.GetClientId(parameters)
			};

			try
			{
				var connectionInfo = this.CreateRelativityConnectionInfo(parameters);
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
								"Unexpected error occurred inside Transfer API layer. Exception: " + ex, new LoggerOptions() { System = "WinEDDS" });
				throw;
			}
		}
	}
}