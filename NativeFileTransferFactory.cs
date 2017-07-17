// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NativeFileTransferFactory.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the core file transfer class object to support native files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Net;
    using System.Threading;

    using Relativity.Logging;
    using Relativity.Services.ServiceProxy;
    using Relativity.Transfer;

    /// <summary>
    /// Represents a class to create <see cref="NativeFileTransfer"/> instances.
    /// </summary>
    public static class NativeFileTransferFactory
    {
        /// <summary>
        /// Creates a <see cref="NativeFileTransfer"/> instance that supports native file upload transfers.
        /// </summary>
        /// <param name="webServiceUrl">
        /// The Relativity web service URL.
        /// </param>
        /// <param name="credentials">
        /// The Relativity network credentials.
        /// </param>
        /// <param name="workspaceId">
        /// The workspace artifact identifier.
        /// </param>
        /// <param name="targetPath">
        /// The target path.
        /// </param>
        /// <param name="container">
        /// The cookie container.
        /// </param>
        /// <param name="isBulkEnabled">
        /// Specify whether the bulk feature is enabled.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="NativeFileTransfer"/> instance.
        /// </returns>
        public static NativeFileTransfer CreateUploadFileTransfer(
            string webServiceUrl,
            NetworkCredential credentials,
            int workspaceId,
            string targetPath,
            CookieContainer container,
            bool isBulkEnabled,
            CancellationToken token)
        {
            return CreateUploadFileTransfer(
                CreateRelativityConnectionInfo(webServiceUrl, credentials, workspaceId),
                workspaceId,
                targetPath,
                container,
                isBulkEnabled,
                null,
                token);
        }

        /// <summary>
        /// Creates a <see cref="NativeFileTransfer"/> instance that supports native file upload transfers.
        /// </summary>
        /// <param name="connectionInfo">
        /// The Relativity connection information.
        /// </param>
        /// <param name="workspaceId">
        /// The workspace artifact identifier.
        /// </param>
        /// <param name="targetPath">
        /// The target path.
        /// </param>
        /// <param name="container">
        /// The cookie container.
        /// </param>
        /// <param name="isBulkEnabled">
        /// Specify whether the bulk feature is enabled.
        /// </param>
        /// <param name="log">
        /// The Relativity transfer log.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="NativeFileTransfer"/> instance.
        /// </returns>
        internal static NativeFileTransfer CreateUploadFileTransfer(
            RelativityConnectionInfo connectionInfo,
            int workspaceId,
            string targetPath,
            CookieContainer container,
            bool isBulkEnabled,
            ILog log,
            CancellationToken token)
        {
            var instance = new NativeFileTransfer(
                connectionInfo,
                workspaceId,
                targetPath,
                container,
                isBulkEnabled,
                TransferDirection.Upload,
                token,
                log ?? new NullLogger());
            return instance;
        }

        /// <summary>
        /// Creates a Relativity connection information object.
        /// </summary>
        /// <param name="webServiceUrl">
        /// The Relativity web service URL.
        /// </param>
        /// <param name="credentials">
        /// The network credentials.
        /// </param>
        /// <param name="workspaceId">
        /// The workspace identifier.
        /// </param>
        /// <returns>
        /// The <see cref="RelativityConnectionInfo"/> instance.
        /// </returns>
        private static RelativityConnectionInfo CreateRelativityConnectionInfo(
            string webServiceUrl,
            NetworkCredential credentials,
            int workspaceId)
        {
            var baseUri = new Uri(webServiceUrl);
            var host = new Uri(baseUri.GetLeftPart(UriPartial.Authority));
            return new RelativityConnectionInfo(
                host,
                new UsernamePasswordCredentials(credentials.UserName, credentials.Password),
                workspaceId);
        }
    }
}