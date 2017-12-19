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
    /// Represents a class to create <see cref="TapiBridgeBase"/> instances.
    /// </summary>
    public static class TapiBridgeFactory
    {
		/// <summary>
		/// Creates a <see cref="UploadTapiBridge"/> instance that supports native file upload transfers.
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
		/// The <see cref="UploadTapiBridge"/> instance.
		/// </returns>
		public static UploadTapiBridge CreateUploadBridge(UploadTapiBridgeParameters parameters, ILog log, CancellationToken token)
        {
            var transferLog = GetTransferLog(log);
            return new UploadTapiBridge(parameters, transferLog, token);
        }

		/// <summary>
		/// Creates a <see cref="DownloadTapiBridge"/> instance that supports download transfers.
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
		/// The <see cref="DownloadTapiBridge"/> instance.
		/// </returns>
		public static DownloadTapiBridge CreateDownloadBridge(TapiBridgeParameters parameters, ILog log, CancellationToken token)
		{
			var transferLog = GetTransferLog(log);
			return new DownloadTapiBridge(parameters, transferLog, token);
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
                // For legacy code and performance considerations, disable automated statistics logging.
                GlobalSettings.Instance.StatisticsLogEnabled = false;
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