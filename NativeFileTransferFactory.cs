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
    using System.Threading;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Represents a class to create <see cref="NativeFileTransfer"/> instances.
    /// </summary>
    public static class NativeFileTransferFactory
    {
        /// <summary>
        /// Creates a <see cref="NativeFileTransfer"/> instance that supports native file upload transfers.
        /// </summary>
        /// <param name="parameters">
        /// The native file transfer parameters
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="NativeFileTransfer"/> instance.
        /// </returns>
        public static NativeFileTransfer CreateUploadFileTransfer(
            NativeFileTransferParameters parameters,
            CancellationToken token)
        {
            ILog log = new NullLogger();
            if (parameters.LogEnabled)
            {
                // Making a number of assumptions in the interest of testing and testability.
                LogSettings.Instance.LogEnabled = true;
                LogSettings.Instance.MinimumLogLevel = LoggingLevel.Debug;
                LogSettings.Instance.Sinks = LogSinks.Console | LogSinks.Seq;
                log = new TransferLog();
            }

            return new NativeFileTransfer(parameters, TransferDirection.Upload, log, token);
        }

        /// <summary>
        /// Creates a <see cref="NativeFileTransfer"/> instance that supports native file upload transfers.
        /// </summary>
        /// <param name="parameters">
        /// The native file transfer parameters
        /// </param>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="NativeFileTransfer"/> instance.
        /// </returns>
        public static NativeFileTransfer CreateUploadFileTransfer(
            NativeFileTransferParameters parameters,
            ILog log,
            CancellationToken token)
        {
            return new NativeFileTransfer(parameters, TransferDirection.Upload, log, token);
        }
    }
}