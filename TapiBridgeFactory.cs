// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeFactory.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class to create <see cref="TransferClientBridge"/> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System.Threading;

    using Relativity.Transfer;

    /// <summary>
    /// Represents a class to create <see cref="TapiBridge"/> instances.
    /// </summary>
    public static class TapiBridgeFactory
    {
        /// <summary>
        /// Creates a <see cref="TapiBridge"/> instance that supports native file upload transfers.
        /// </summary>
        /// <param name="parameters">
        /// The native file transfer parameters
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="TapiBridge"/> instance.
        /// </returns>
        public static TapiBridge CreateUploadBridge(
            TapiBridgeParameters parameters,
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

            return new TapiBridge(parameters, TransferDirection.Upload, log, token);
        }

        /// <summary>
        /// Creates a <see cref="TapiBridge"/> instance that supports native file upload transfers.
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
        /// The <see cref="TapiBridge"/> instance.
        /// </returns>
        public static TapiBridge CreateUploadBridge(
            TapiBridgeParameters parameters,
            ILog log,
            CancellationToken token)
        {
            return new TapiBridge(parameters, TransferDirection.Upload, log, token);
        }
    }
}