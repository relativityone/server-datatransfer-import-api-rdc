// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridge2.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for uploading.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Threading;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Resources;
	using Relativity.Logging;
	using Relativity.Transfer;
	using Relativity.Transfer.Aspera;

	/// <summary>
	///     Represents a class object to provide a upload bridge from the Transfer API to existing import/export components. This class cannot be inherited, backwards compatibility isn't guaranteed, and should never be consumed by API users.
	/// </summary>
	public sealed class UploadTapiBridge2 : TapiBridgeBase2
	{
		/// <summary>
		/// The manager used to limit the maximum number of files per folder.
		/// </summary>
		private readonly FileSharePathManager pathManager;

		/// <summary>
		/// The upload specific parameters.
		/// </summary>
		private readonly UploadTapiBridgeParameters2 parameters;

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridge2"/> class.
		/// </summary>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger instance.
		/// </param>
		/// <param name="authTokenProvider">Authentication token provider.</param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public UploadTapiBridge2(UploadTapiBridgeParameters2 parameters, ILog logger, IAuthenticationTokenProvider authTokenProvider, CancellationToken token)
			: this(new TapiObjectService(authTokenProvider), parameters, logger, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridge2"/> class.
		/// </summary>
		/// <param name="factory">
		/// The Transfer API object factory.
		/// </param>
		/// <param name="parameters">
		/// The native file transfer parameters.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger instance.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public UploadTapiBridge2(
			ITapiObjectService factory,
			UploadTapiBridgeParameters2 parameters,
			ILog logger,
			CancellationToken token)
			: this(factory, parameters, null, logger, token)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridge2"/> class.
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
		/// The Relativity logger instance.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		public UploadTapiBridge2(
			ITapiObjectService factory,
			UploadTapiBridgeParameters2 parameters,
			TransferContext context,
			ILog logger,
			CancellationToken token)
			: base(factory, parameters, TransferDirection.Upload, context, logger, token)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			this.parameters = parameters;
			this.pathManager = new FileSharePathManager(parameters.MaxFilesPerFolder);
		}

		/// <summary>
		/// Gets the target folder name.
		/// </summary>
		/// <value>
		/// The folder name.
		/// </value>
		public string TargetFolderName => this.pathManager.CurrentTargetFolderName;

		/// <summary>
		/// Adds the path to a transfer job.
		/// </summary>
		/// <param name="sourceFile">
		/// The full path to the source file.
		/// </param>
		/// <param name="targetFileName">
		/// The optional target filename.
		/// </param>
		/// <param name="order">
		/// The order the path is added to the transfer job.
		/// </param>
		/// <returns>
		/// The file name.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="sourceFile" /> is <see langword="null" /> or empty.
		/// </exception>
		public string AddPath(string sourceFile, string targetFileName, int order)
		{
			if (string.IsNullOrEmpty(sourceFile))
			{
				throw new ArgumentNullException(nameof(sourceFile));
			}

			var transferPath = new TransferPath
			{
				SourcePath = sourceFile,
				TargetPath =
					this.parameters.SortIntoVolumes
						? this.pathManager.GetNextTargetPath(this.TargetPath)
						: this.TargetPath,
				TargetFileName = targetFileName,
				Order = order,
			};

			return this.AddPath(transferPath);
		}

		/// <summary>
		/// Dump the transfer bridge parameter.
		/// </summary>
		public override void LogTransferParameters()
		{
			base.LogTransferParameters();
			this.Logger.LogInformation("BCP file transfer: {BcpFileTransfer}", this.parameters.BcpFileTransfer);
			this.Logger.LogInformation("Aspera BCP root folder: {AsperaBcpRootFolder}", this.parameters.AsperaBcpRootFolder);
			this.Logger.LogInformation("Sort into volume: {SortIntoVolumes}", this.parameters.SortIntoVolumes);
			this.Logger.LogInformation("Max file per folder: {MaxFilesPerFolder}", this.parameters.MaxFilesPerFolder);
		}

		/// <summary>
		/// Fatal error message for uploading files.
		/// </summary>
		/// <returns>
		/// The error message.
		/// </returns>
		protected override string TransferFileFatalMessage()
		{
			return Strings.TransferFileUploadFatalMessage;
		}

		/// <summary>
		/// Creates transfer request for upload job.
		/// </summary>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		/// <returns>
		/// The <see cref="TransferRequest"/> instance.
		/// </returns>
		protected override TransferRequest CreateTransferRequestForJob(TransferContext context)
		{
			return TransferRequest.ForUploadJob(this.TargetPath, context);
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

			switch (this.ClientId.ToString().ToUpperInvariant())
			{
				case TransferClientConstants.AsperaClientId:
					IRemotePathResolver resolver;
					if (this.parameters.BcpFileTransfer)
					{
						resolver = new AsperaUncBcpPathResolver(
							this.parameters.FileShare,
							this.parameters.AsperaBcpRootFolder);
					}
					else
					{
						resolver = new AsperaUncPathResolver(
							this.parameters.FileShare,
							this.parameters.AsperaDocRootLevels);
					}

					request.TargetPathResolver = resolver;
					break;
			}
		}

		/// <inheritdoc />
		protected override ClientConfiguration CreateClientConfiguration()
		{
			// Note: providing this hint is enough to choose the appropriate credential.
			var clientConfiguration = base.CreateClientConfiguration();
			clientConfiguration.FileTransferHint =
				this.parameters.BcpFileTransfer ? FileTransferHint.BulkLoad : FileTransferHint.Natives;
			return clientConfiguration;
		}
	}
}