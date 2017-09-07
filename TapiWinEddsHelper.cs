// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiWinEddsHelper.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Text;

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
    }
}