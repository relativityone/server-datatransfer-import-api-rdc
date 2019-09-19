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
	using Relativity.Transfer;
	using Relativity.Transfer.Aspera;

	/// <summary>
	///     Represents a class object to provide a download bridge from the Transfer API to existing import/export components. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	public sealed class DownloadTapiBridge2 : TapiBridgeBase2
	{
		private readonly TapiBridgeParameters2 parameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="DownloadTapiBridge2"/> class.
		/// </summary>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public DownloadTapiBridge2(TapiBridgeParameters2 parameters, ITransferLog log, CancellationToken token)
			: this(new TapiObjectService(new NullAuthTokenProvider()), parameters, log, token)
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
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public DownloadTapiBridge2(
			ITapiObjectService factory,
			TapiBridgeParameters2 parameters,
			ITransferLog log,
			CancellationToken token)
			: this(factory, parameters, null, log, token)
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
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public DownloadTapiBridge2(
			ITapiObjectService factory,
			TapiBridgeParameters2 parameters,
			TransferContext context,
			ITransferLog log,
			CancellationToken token)
			: base(factory, parameters, TransferDirection.Download, context, log, token)
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
							this.parameters.AsperaDocRootLevels);
					break;
			}

			request.SourcePathResolver = resolver;
		}

		/// <inheritdoc />
		protected override ClientConfiguration CreateClientConfiguration()
		{
			var clientConfiguration = base.CreateClientConfiguration();
			clientConfiguration.FileNotFoundErrorsDisabled = true;
			clientConfiguration.FileNotFoundErrorsRetry = false;
			clientConfiguration.PermissionErrorsRetry = false;
			return clientConfiguration;
		}
	}
}