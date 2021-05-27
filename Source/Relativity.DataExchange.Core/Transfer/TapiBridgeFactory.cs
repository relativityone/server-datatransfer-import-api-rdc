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
		/// <param name="logger">
		/// The Relativity logging instance.
		/// </param>
		/// <param name="authenticationTokenProvider">Auth token provider.</param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The <see cref="UploadTapiBridge2"/> instance.
		/// </returns>
		public static UploadTapiBridge2 CreateUploadBridge(
			UploadTapiBridgeParameters2 parameters,
			ILog logger,
			IAuthenticationTokenProvider authenticationTokenProvider,
			CancellationToken token)
		{
			return new UploadTapiBridge2(parameters, logger, authenticationTokenProvider, token);
		}

		/// <summary>
		/// Creates a <see cref="DownloadTapiBridge2"/> instance that supports download transfers.
		/// </summary>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="logger">
		/// The Relativity logging instance.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The <see cref="DownloadTapiBridge2"/> instance.
		/// </returns>
		public static DownloadTapiBridge2 CreateDownloadBridge(DownloadTapiBridgeParameters2 parameters, ILog logger, CancellationToken token)
		{
			return new DownloadTapiBridge2(parameters, logger, token);
		}
	}
}