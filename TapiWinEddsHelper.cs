// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiClientDocs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    
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
            foreach (var clientMetadata in TransferClientHelper.SearchClientMetadata())
            {
                if (sb.Length > 0)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
            
                sb.AppendFormat(" • {0} • ", clientMetadata.Name);
                sb.AppendLine();
                sb.Append(clientMetadata.Description);
            }

            return sb.ToString();
        }
    }
}