﻿// --------------------------------------------------------------------------------------------------------------------
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
	using System.Globalization;
	using System.Threading;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Resources;
	using Relativity.DataExchange.Service;
	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Represents a class object to provide Transfer API object services to the transfer bridges. This class cannot be inherited.
	/// </summary>
	internal class TapiObjectService : ITapiObjectService
	{
		/// <summary>
		/// The singleton instance.
		/// </summary>
		private static readonly Relativity.Transfer.IFileSystemService Instance = new Relativity.Transfer.FileSystemService();

		private readonly IAuthenticationTokenProvider authenticationTokenProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiObjectService"/> class.
		/// </summary>
		public TapiObjectService()
			: this(new NullAuthTokenProvider())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiObjectService"/> class.
		/// </summary>
		/// <param name="authenticationTokenProvider">Authentication token provider.</param>
		public TapiObjectService(IAuthenticationTokenProvider authenticationTokenProvider)
		{
			authenticationTokenProvider.ThrowIfNull(nameof(authenticationTokenProvider));

			this.authenticationTokenProvider = authenticationTokenProvider;
		}

		/// <inheritdoc />
		public virtual Relativity.Transfer.IFileSystemService CreateFileSystemService()
		{
			return Instance;
		}

		/// <inheritdoc />
		public virtual void ApplyUnmappedFileRepositoryParameters(TapiBridgeParameters2 parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			// Note: only direct/web modes support transfer jobs that can access files from
			//       any given file repository without any additional configuration.
			parameters.ForceClientCandidates = string.Join(
				";",
				Relativity.Transfer.WellKnownTransferClient.FileShare.ToString(),
				Relativity.Transfer.WellKnownTransferClient.Http.ToString());

			// Allow other clients to be forced - just clear Aspera.
			parameters.ForceAsperaClient = false;
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
		public virtual Relativity.Transfer.RelativityConnectionInfo CreateRelativityConnectionInfo(TapiBridgeParameters2 parameters)
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
				httpCredential = new Relativity.Transfer.BearerTokenCredential(parameters.Credentials.Password, this.authenticationTokenProvider);
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
		public virtual Relativity.Transfer.IRelativityTransferHost CreateRelativityTransferHost(Relativity.Transfer.RelativityConnectionInfo connectionInfo, ILog logger)
		{
			return new Relativity.Transfer.RelativityTransferHost(connectionInfo, new RelativityTransferLog(logger));
		}

		/// <inheritdoc />
		public virtual string GetClientDisplayName(Guid clientId)
		{
			if (clientId == Guid.Empty)
			{
				throw new ArgumentException("The client unique identifier must be non-empty.", nameof(clientId));
			}

			using (var transferLog = new RelativityTransferLog(RelativityLogger.Instance))
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
		public virtual Guid GetClientId(TapiBridgeParameters2 parameters)
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
		public virtual TapiClient GetTapiClient(Guid clientId)
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
		public virtual async Task<RelativityFileShare> GetWorkspaceDefaultFileShareAsync(TapiBridgeParameters2 parameters, ILog logger, CancellationToken token)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (ITransferLog transferLog = new RelativityTransferLog(logger))
			using (IRelativityTransferHost transferHost = new RelativityTransferHost(
				this.CreateRelativityConnectionInfo(parameters),
				transferLog))
			{
				Workspace workspace = await transferHost.GetWorkspaceAsync(parameters.WorkspaceId, token)
					                      .ConfigureAwait(false);
				if (workspace == null)
				{
					string message = string.Format(
						CultureInfo.CurrentCulture,
						Strings.WorkspaceNullExceptionMessage,
						parameters.WorkspaceId);
					throw new TransferException(message);
				}

				return workspace.DefaultFileShare;
			}
		}

		/// <inheritdoc />
		public virtual async Task<string> GetWorkspaceClientDisplayNameAsync(TapiBridgeParameters2 parameters)
		{
			Tuple<Guid, string> transferClientInfo =
				await this.GetWorkspaceTransferClientInfoAsync(parameters).ConfigureAwait(false);
			return transferClientInfo.Item2;
		}

		/// <inheritdoc />
		public virtual async Task<Guid> GetWorkspaceClientIdAsync(TapiBridgeParameters2 parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			Tuple<Guid, string> transferClientInfo =
				await this.GetWorkspaceTransferClientInfoAsync(parameters).ConfigureAwait(false);
			return transferClientInfo.Item1;
		}

		/// <inheritdoc />
		public virtual async Task<ITapiFileStorageSearchResults> SearchFileStorageAsync(
			TapiBridgeParameters2 parameters,
			ILog logger,
			CancellationToken token)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			using (ITransferLog transferLog = new RelativityTransferLog(logger))
			using (IRelativityTransferHost transferHost = new RelativityTransferHost(
				this.CreateRelativityConnectionInfo(parameters),
				transferLog))
			{
				IFileStorageSearch service = transferHost.CreateFileStorageSearch();
				FileStorageSearchContext context =
					new FileStorageSearchContext { WorkspaceId = parameters.WorkspaceId };
				FileStorageSearchResults results = await service.SearchAsync(context, token).ConfigureAwait(false);
				return new TapiFileStorageSearchResults(results);
			}
		}

		/// <inheritdoc />
		public virtual void SetTapiClient(TapiBridgeParameters2 parameters, TapiClient targetClient)
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
		/// Asynchronously gets the transfer client info that will be used for the given workspace.
		/// </summary>
		/// <param name="parameters">
		/// The bridge connection parameters.
		/// </param>
		/// <returns>
		/// The <see cref="Tuple{Guid,String}"/> containing the transfer client identifier and display name.
		/// </returns>
		private async Task<Tuple<Guid, string>> GetWorkspaceTransferClientInfoAsync(TapiBridgeParameters2 parameters)
		{
			Relativity.Transfer.ClientConfiguration configuration = new Relativity.Transfer.ClientConfiguration
				                                                        {
					                                                        CookieContainer =
						                                                        parameters.WebCookieContainer,
					                                                        ClientId = this.GetClientId(parameters),
				                                                        };

			using (RelativityTransferLog transferLog = new RelativityTransferLog(RelativityLogger.Instance))
			using (RelativityTransferHost transferHost = new RelativityTransferHost(
				this.CreateRelativityConnectionInfo(parameters),
				transferLog))
			{
				if (configuration.ClientId != Guid.Empty)
				{
					using (Relativity.Transfer.ITransferClient client = transferHost.CreateClient(configuration))
					{
						Relativity.Transfer.ISupportCheckResult supportCheck =
							await client.SupportCheckAsync().ConfigureAwait(false);
						if (supportCheck.IsSupported)
						{
							return Tuple.Create(client.Id, client.DisplayName);
						}
					}
				}

				Relativity.Transfer.TransferClientStrategy clientStrategy =
					string.IsNullOrEmpty(parameters.ForceClientCandidates)
						? new TransferClientStrategy()
						: new TransferClientStrategy(parameters.ForceClientCandidates);
				using (Relativity.Transfer.ITransferClient client =
					await transferHost.CreateClientAsync(configuration, clientStrategy).ConfigureAwait(false))
				{
					return Tuple.Create(client.Id, client.DisplayName);
				}
			}
		}
	}
}