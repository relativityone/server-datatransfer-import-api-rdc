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

	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.Service.WebApiVsKeplerSwitch;
	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Represents a class to create <see cref="TapiBridgeBase2"/> instances.
	/// </summary>
	internal static class TapiBridgeFactory
	{
		/// <summary>
		/// Gets or sets UseLegacyWebApiInTests. Do not use. It is for testing purpose only.
		/// </summary>
		internal static bool? UseLegacyWebApiInTests { get; set; }

		/// <summary>
		/// Creates a <see cref="UploadTapiBridge2"/> instance that supports native file upload transfers.
		/// </summary>
		/// <param name="parameters">
		///     The native file transfer parameters.
		/// </param>
		/// <param name="logger">
		///     The Relativity logging instance.
		/// </param>
		/// <param name="authenticationTokenProvider">Auth token provider.</param>
		/// <param name="token">
		///     The cancellation token.
		/// </param>
		/// <param name="getCorrelationId">
		///     Function to obtain correlationId.
		/// </param>
		/// <param name="webApiVsKeplerFactory">
		///     IWebApiVsKeplerFactory.
		/// </param>
		/// <param name="relativityManagerServiceFactory">
		///     Factory to create Relativity service manager.
		/// </param>
		/// <returns>
		/// The <see cref="UploadTapiBridge2"/> instance.
		/// </returns>
		public static UploadTapiBridge2 CreateUploadBridge(
			UploadTapiBridgeParameters2 parameters,
			ILog logger,
			IAuthenticationTokenProvider authenticationTokenProvider,
			CancellationToken token,
			Func<string> getCorrelationId,
			IWebApiVsKeplerFactory webApiVsKeplerFactory,
			IRelativityManagerServiceFactory relativityManagerServiceFactory)
		{
			var useLegacyWebApi = UseLegacyWebApi(parameters, webApiVsKeplerFactory, getCorrelationId);
			return new UploadTapiBridge2(parameters, logger, authenticationTokenProvider, token, useLegacyWebApi, relativityManagerServiceFactory);
		}

		/// <summary>
		/// Creates a <see cref="DownloadTapiBridge2"/> instance that supports download transfers.
		/// </summary>
		/// <param name="parameters">
		///     The native file transfer parameters.
		/// </param>
		/// <param name="logger">
		///     The Relativity logging instance.
		/// </param>
		/// <param name="token">
		///     The cancellation token.
		/// </param>
		/// <param name="getCorrelationId">
		///     Function to obtain correlationId.
		/// </param>
		/// <param name="webApiVsKeplerFactory">
		///     IWebApiVsKeplerFactory.
		/// </param>
		/// <param name="relativityManagerServiceFactory">
		///     Factory to create Relativity service manager.
		/// </param>
		/// <returns>
		/// The <see cref="DownloadTapiBridge2"/> instance.
		/// </returns>
		public static DownloadTapiBridge2 CreateDownloadBridge(
			DownloadTapiBridgeParameters2 parameters,
			ILog logger,
			CancellationToken token,
			Func<string> getCorrelationId,
			IWebApiVsKeplerFactory webApiVsKeplerFactory,
			IRelativityManagerServiceFactory relativityManagerServiceFactory)
		{
			bool useLegacyWebApi = UseLegacyWebApi(parameters, webApiVsKeplerFactory, getCorrelationId);
			return new DownloadTapiBridge2(parameters, logger, token, useLegacyWebApi, relativityManagerServiceFactory);
		}

		private static bool UseLegacyWebApi(
			TapiBridgeParameters2 parameters,
			IWebApiVsKeplerFactory webApiVsKeplerFactory,
			Func<string> getCorrelationId)
		{
			// In unit tests we want to return hardcoded value.
			if (UseLegacyWebApiInTests.HasValue)
			{
				return UseLegacyWebApiInTests.Value;
			}

			IWebApiVsKepler webApiVsKepler = webApiVsKeplerFactory.Create(
				new Uri(AppSettings.Instance.WebApiServiceUrl),
				parameters.Credentials,
				getCorrelationId);
			return !webApiVsKepler.UseKepler();
		}
	}
}