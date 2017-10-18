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
    using System;
    using System.Threading;

    using Relativity.Logging;
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
        /// <param name="log">
        /// The Relativity logging instance.
        /// </param>
        /// <param name="token">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="TapiBridge"/> instance.
        /// </returns>
        public static TapiBridge CreateUploadBridge(TapiBridgeParameters parameters, ILog log, CancellationToken token)
        {
            var transferLog = GetTransferLog(log);
            return new TapiBridge(parameters, TransferDirection.Upload, transferLog, token);
        }

        /// <summary>
        /// Gets the transfer log instance.
        /// </summary>
        /// <param name="log">
        /// The Relativity logging instance.
        /// </param>
        /// <returns>
        /// The <see cref="ITransferLog"/> instance.
        /// </returns>
        private static ITransferLog GetTransferLog(ILog log)
        {
            try
            {
#if DEBUG
                LogSettings.Instance.MinimumLogLevel = LoggingLevel.Debug;
#else
                LogSettings.Instance.MinimumLogLevel = LoggingLevel.Information;
#endif
                return new RelativityTransferLog(log, false);
            }
            catch (Exception e)
            {
                try
                {
                    Relativity.Logging.Tools.InternalLogger.WriteTokCuraEventLog(
                        "Failed to setup TAPI logging. Exception: " + e,
                        "WinEDDS");
                }
                catch (Exception)
                {
                    // Being overly cautious to ensure no fatal errors occur due to logging.
                }

                return new NullTransferLog();
            }
        }
    }
}