namespace kCura.WinEDDS.TApi
{
	using System.Threading;
	using Relativity.Transfer;

	/// <summary>
	///     Represents a class object to provide a upload bridge from the Transfer API to existing WinEDDS code.
	/// </summary>
	public class UploadTapiBridge : TapiBridge
	{
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
		public UploadTapiBridge(TapiBridgeParameters parameters, ITransferLog log, CancellationToken token) : base(parameters, TransferDirection.Upload, log, token)
		{
		}

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
	}
}