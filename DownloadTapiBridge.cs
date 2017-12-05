namespace kCura.WinEDDS.TApi
{
	using System.Threading;
	using Relativity.Transfer;

	/// <summary>
	///     Represents a class object to provide a download bridge from the Transfer API to existing WinEDDS code.
	/// </summary>
	public class DownloadTapiBridge : TapiBridge
	{
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
		}

		/// <summary>
		/// Adds the path to a transfer job.
		/// </summary>
		/// <param name="transferPath">
		/// The path to add to the job.
		/// </param>
		/// <returns></returns>
		public new string AddPath(TransferPath transferPath)
		{
			return base.AddPath(transferPath);
		}
	}
}