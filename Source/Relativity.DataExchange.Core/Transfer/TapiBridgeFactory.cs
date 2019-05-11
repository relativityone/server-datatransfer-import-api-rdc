// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiBridgeFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class to create <see cref="TransferClientBridge"/> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Threading;

	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Represents a class to create <see cref="TapiBridgeBase2"/> instances.
	/// </summary>
	internal static class TapiBridgeFactory
	{
		/// <summary>
		/// Creates a <see cref="UploadTapiBridge2"/> instance that supports native file upload transfers.
		/// </summary>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="log">
		/// The Relativity logging instance.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The <see cref="UploadTapiBridge2"/> instance.
		/// </returns>
		public static UploadTapiBridge2 CreateUploadBridge(UploadTapiBridgeParameters2 parameters, ILog log, CancellationToken token)
		{
			var transferLog = GetTransferLog(log);
			return new UploadTapiBridge2(parameters, transferLog, token);
		}

		/// <summary>
		/// Creates a <see cref="DownloadTapiBridge2"/> instance that supports download transfers.
		/// </summary>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="log">
		/// The Relativity logging instance.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The <see cref="DownloadTapiBridge2"/> instance.
		/// </returns>
		public static DownloadTapiBridge2 CreateDownloadBridge(TapiBridgeParameters2 parameters, ILog log, CancellationToken token)
		{
			var transferLog = GetTransferLog(log);
			return new DownloadTapiBridge2(parameters, transferLog, token);
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
					Relativity.Logging.Tools.InternalLogger.WriteFromExternal(
						"Failed to setup Transfer API logging. Exception: " + e,
						new LoggerOptions() { System = "WinEDDS" });
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