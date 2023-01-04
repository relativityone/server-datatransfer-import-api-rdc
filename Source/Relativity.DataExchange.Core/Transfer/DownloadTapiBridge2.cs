// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridge2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for downloading.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Threading;

	using Relativity.DataExchange.Resources;
	using Relativity.DataExchange.Service;
	using Relativity.Logging;
	using Relativity.Transfer;
	using Relativity.Transfer.Aspera;

	/// <summary>
	///     Represents a class object to provide a download bridge from the Transfer API to existing import/export components. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	public sealed class DownloadTapiBridge2 : TapiBridgeBase2
	{
		private readonly DownloadTapiBridgeParameters2 parameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadTapiBridge2"/> class.
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
		/// <param name="useLegacyWebApi">
		/// If true use WebApi, otherwise use Kepler.
		/// </param>
		/// <param name="relativityManagerServiceFactory">
		///     Factory to create Relativity service manager.
		/// </param>
		public DownloadTapiBridge2(
			DownloadTapiBridgeParameters2 parameters,
			ILog logger,
			CancellationToken token,
			bool useLegacyWebApi,
			IRelativityManagerServiceFactory relativityManagerServiceFactory)
			: this(new TapiObjectService(relativityManagerServiceFactory, useLegacyWebApi), parameters, logger, useLegacyWebApi, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadTapiBridge2" /> class.
		/// </summary>
		/// <param name="factory">
		/// The Transfer API object factory.
		/// </param>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="logger">
		/// The Relativity logging instance.
		/// </param>
		/// <param name="useLegacyWebApi">
		/// If true use WebApi, otherwise use Kepler.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public DownloadTapiBridge2(
			ITapiObjectService factory,
			DownloadTapiBridgeParameters2 parameters,
			ILog logger,
			bool useLegacyWebApi,
			CancellationToken token)
			: this(factory, parameters, null, logger, useLegacyWebApi, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadTapiBridge2" /> class.
		/// </summary>
		/// <param name="factory">
		/// The Transfer API object factory.
		/// </param>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logging instance.
		/// </param>
		/// <param name="useLegacyWebApi">
		/// If true use WebApi, otherwise use Kepler.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public DownloadTapiBridge2(
			ITapiObjectService factory,
			DownloadTapiBridgeParameters2 parameters,
			TransferContext context,
			ILog logger,
			bool useLegacyWebApi,
			CancellationToken token)
			: base(factory, parameters, TransferDirection.Download, context, logger, useLegacyWebApi, token)
		{
			this.parameters = parameters;
		}

		/// <summary>
		/// Fatal error message for downloading files.
		/// </summary>
		/// <returns>
		/// The error message.
		/// </returns>
		protected override string TransferFileFatalMessage()
		{
			return Strings.TransferFileDownloadFatalMessage;
		}

		/// <summary>
		/// Creates transfer request for download job.
		/// </summary>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		/// <returns>
		/// The <see cref="TransferRequest"/> instance.
		/// </returns>
		protected override TransferRequest CreateTransferRequestForJob(TransferContext context)
		{
			return TransferRequest.ForDownloadJob(this.TargetPath, context);
		}

		/// <summary>
		/// Setup the customer resolvers for both source and target paths for upload job.
		/// </summary>
		/// <param name="request">
		/// The transfer request.
		/// </param>
		/// <remarks>
		/// This provides backwards compatibility with IAPI.
		/// </remarks>
		protected override void SetupRemotePathResolvers(ITransferRequest request)
		{
			if (request == null)
			{
				throw new ArgumentNullException(nameof(request));
			}

			IRemotePathResolver resolver = null;

			switch (this.ClientId.ToString().ToUpperInvariant())
			{
				case TransferClientConstants.AsperaClientId:
					resolver = new AsperaUncPathResolver(
						this.parameters.FileShare,
						this.parameters.AsperaDocRootLevels,
						this.TransferLog);
					break;
			}

			request.SourcePathResolver = resolver;
		}
	}
}