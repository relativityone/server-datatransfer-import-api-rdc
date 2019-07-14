namespace Relativity.DataExchange.Export.VolumeManagerV2.Download.TapiHelpers
{
	using System;
	using System.Threading;

	/// <summary>
	/// Represents an abstract object used to dynamically create and manage pooled transfer bridges.
	/// Implements the <see cref="System.IDisposable" />
	/// </summary>
	/// <seealso cref="System.IDisposable" />
	public interface IFileTapiBridgePool : IDisposable
	{
		/// <summary>
		/// Gets the total number of pooled transfer bridges.
		/// </summary>
		/// <value>
		/// The total number of transfer bridges.
		/// </value>
		int Count
		{
			get;
		}

		/// <summary>
		/// Requests the transfer bridge associated with the specified file share. If one doesn't exist, a new bridge is pooled.
		/// </summary>
		/// <param name="settings">
		/// The file share settings.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The <see cref="IDownloadTapiBridge"/> instance.
		/// </returns>
		IDownloadTapiBridge Request(IRelativityFileShareSettings settings, CancellationToken token);
	}
}