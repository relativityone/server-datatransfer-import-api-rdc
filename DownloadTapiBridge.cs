// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadTapiBridge.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for downloading.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System;

namespace kCura.WinEDDS.TApi
{
	using System.Threading;
	using Relativity.Transfer;
	using kCura.WinEDDS.TApi.Resources;
	using Relativity.Transfer.Aspera;

	/// <summary>
	///     Represents a class object to provide a download bridge from the Transfer API to existing WinEDDS code.
	/// </summary>
	public sealed class DownloadTapiBridge : TapiBridgeBase
	{
		private readonly TapiBridgeParameters parameters;

		/// <summary>
		///     Initializes a new instance of the <see cref="DownloadTapiBridge" /> class.
		/// </summary>
		/// <param name="parameters">
		///     The native file transfer parameters.
		/// </param>
		/// <param name="log">
		///     The transfer log.
		/// </param>
		/// <param name="token">
		///     The cancellation token.
		/// </param>
		/// <remarks>
		///     Don't expose TAPI objects to WinEDDS - at least not yet. This is reserved for integration tests.
		/// </remarks>
		public DownloadTapiBridge(TapiBridgeParameters parameters, ITransferLog log, CancellationToken token) : base(parameters, TransferDirection.Download, log, token)
		{
			this.parameters = parameters;
		}

		/// <summary>
		/// Fatal error message for downloading files
		/// </summary>
		/// <returns></returns>
		protected override string TransferFileFatalMessage()
		{
			return Strings.TransferFileDownloadFatalMessage;
		}

		/// <summary>
		/// Creates transfer request for download job
		/// </summary>
		/// <returns></returns>
		protected override TransferRequest CreateTransferRequestForJob(TransferContext transferContext)
		{
			return TransferRequest.ForDownloadJob(this.TargetPath, transferContext);
		}

		/// <summary>
		/// Setup the customer resolvers for both source and target paths for upload job.
		/// </summary>
		/// <remarks>
		/// This provides backwards compatibility with IAPI.
		/// </remarks>
		protected override void SetupRemotePathResolvers(ITransferRequest jobTransferRequest)
		{
			if (jobTransferRequest == null)
			{
				throw new ArgumentNullException(nameof(jobTransferRequest));
			}
			IRemotePathResolver resolver = null;
			switch (this.ClientId.ToString().ToUpperInvariant())
			{
				case TransferClientConstants.AsperaClientId:
					resolver = new AsperaUncPathResolver(
							this.parameters.FileShare,
							this.parameters.AsperaDocRootLevels);
					break;
				case TransferClientConstants.HttpClientId:
					resolver = new HttpClientDownloadPathResolver();
					break;
			}
			jobTransferRequest.SourcePathResolver = resolver;
		}
	}
}