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
            // Once WinEDDS supports logging, add ILog to this method signature.
            var options = new LoggerOptions { Application = "WinEDDS" };
            var log = parameters.LogEnabled
                          ? new RelativityTransferLog(Relativity.Logging.Factory.LogFactory.GetLogger(options), false)
                          : new RelativityTransferLog(Relativity.Logging.Factory.LogFactory.GetNullLogger(), false);
#if DEBUG
            LogSettings.Instance.MinimumLogLevel = LoggingLevel.Debug;
#endif
            return new TapiBridge(parameters, TransferDirection.Upload, log, token);
        }
    }
}