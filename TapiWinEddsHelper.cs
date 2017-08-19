// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClientDocs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Linq;

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Net;
    using System.Text;

    using kCura.WinEDDS.TApi.Resources;

    using Relativity.Transfer;
    using Relativity.Services.ServiceProxy;
    
    /// <summary>
    /// Defines helper methods to provide WinEDDS compatibility functionality.
    /// </summary>
    public static class TapiWinEddsHelper
    {
        /// <summary>
        /// Use TAPI to obtain the best-fit client for the specified workspace and retrieve the client name.
        /// </summary>
        /// <param name="webServicesUrl">
        /// The web services URL.
        /// </param>
        /// <param name="credential">
        /// The credential.
        /// </param>
        /// <param name="workspaceId">
        /// The workspace identifier.
        /// </param>
        /// <returns>
        /// The task.
        /// </returns>
        /// <remarks>
        /// This will reduce the amount of time it takes to construct a client for a given transfer because the client information is cached for a given Relativity instance and workspace.
        /// </remarks>
        public static string GetWorkspaceClientName(string webServicesUrl, NetworkCredential credential, int workspaceId)
        {
            var baseUri = new Uri(webServicesUrl);
            var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
            var connectionInfo = new RelativityConnectionInfo(
                host,
                new UsernamePasswordCredentials(credential.UserName, credential.Password),
                workspaceId);
            using (var transferHost = new RelativityTransferHost(connectionInfo))
            {
                try
                {
                    var client = transferHost.CreateClientAsync();
                    client.Wait();
                    return client.Result.Name;
                }
                catch (Exception)
                {
                    return "Web";
                }
            }
        }

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
        public static RelativityConnectionInfo CreateRelativityConnectionInfo(string webServiceUrl, int workspaceId, string userName, string password)
        {
            var baseUri = new Uri(webServiceUrl);
            var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
            return new RelativityConnectionInfo(host, new BearerTokenCredentials(password), workspaceId);
        }
    }
}