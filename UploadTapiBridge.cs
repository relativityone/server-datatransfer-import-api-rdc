// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UploadTapiBridge.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for uploading.
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
	///     Represents a class object to provide a upload bridge from the Transfer API to existing WinEDDS code.
	/// </summary>
	public sealed class UploadTapiBridge : TapiBridgeBase
	{
		/// <summary>
		/// The manager used to limit the maximum number of files per folder.
		/// </summary>
		private readonly FileSharePathManager pathManager;

		private readonly UploadTapiBridgeParameters parameters;

		/// <summary>
		/// Gets the workspace artifact unique identifier.
		/// </summary>
		/// <value>
		/// The unique identifier.
		/// </value>
		public int WorkspaceId => this.parameters.WorkspaceId;

		/// <summary>
		/// Initializes a new instance of the <see cref="UploadTapiBridge"/> class.
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
		/// <remarks>
		/// Don't expose TAPI objects to WinEDDS - at least not yet. This is reserved for integration tests.
		/// </remarks>
		public UploadTapiBridge(UploadTapiBridgeParameters parameters, ITransferLog log, CancellationToken token) : base(parameters, TransferDirection.Upload, log, token)
		{
			this.parameters = parameters;
			this.pathManager = new FileSharePathManager(parameters.MaxFilesPerFolder);
		}

		/// <summary>
		/// Gets or sets the target folder name.
		/// </summary>
		/// <value>
		/// The folder name.
		/// </value>
		public string TargetFolderName => this.pathManager.CurrentTargetFolderName;

		/// <summary>
		/// Gets a value indicating whether there are transfers pending.
		/// </summary>
		/// <remarks>
		/// Be careful here. The PathCount property was added to avoid costly hits to the repository.
		/// </remarks>
		public bool TransfersPending => this.TransferJob != null && this.TransferJob.PathCount > 0;

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
		public string AddPath(string sourceFile, string targetFileName, int order)
		{
			var transferPath = new TransferPath
			{
				SourcePath = sourceFile,
				TargetPath =
					this.parameters.SortIntoVolumes
						? this.pathManager.GetNextTargetPath(this.TargetPath)
						: this.TargetPath,
				TargetFileName = targetFileName,
				Order = order
			};
			return AddPath(transferPath);
		}
		
		/// <summary>
		/// Dump the transfer bridge parameter.
		/// </summary>
		public override void DumpInfo()
		{
			base.DumpInfo();

			this.TransferLog.LogInformation("BCP file transfer: {BcpFileTransfer}", parameters.BcpFileTransfer);
			this.TransferLog.LogInformation("Aspera BCP root folder: {AsperaBcpRootFolder}", parameters.AsperaBcpRootFolder);
			this.TransferLog.LogInformation("Sort into volume: {SortIntoVolumes}", parameters.SortIntoVolumes);
			this.TransferLog.LogInformation("Max file per folder: {MaxFilesPerFolder}", parameters.MaxFilesPerFolder);
		}

		/// <summary>
		/// Fatal error message for uploading files
		/// </summary>
		/// <returns></returns>
		protected override string TransferFileFatalMessage()
		{
			return Strings.TransferFileUploadFatalMessage;
		}

		/// <summary>
		/// Creates transfer request for upload job
		/// </summary>
		/// <returns></returns>
		protected override TransferRequest CreateTransferRequestForJob(TransferContext transferContext)
		{
			return TransferRequest.ForUploadJob(this.TargetPath, transferContext);
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
					jobTransferRequest.TargetPathResolver = resolver;
					break;
			}
		}
	}
}