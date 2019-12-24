// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ILongTextStreamService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract service to retrieve long text data through a streaming API.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Represents an abstract service to retrieve long text data through a streaming API.
	/// </summary>
	internal interface ILongTextStreamService : IDisposable
	{
		/// <summary>
		/// Asynchronously saves the long text field and performs auto-retry on non-fatal exceptions.
		/// </summary>
		/// <param name="request">
		/// The long text stream request.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <param name="progress">
		/// The stream progress.
		/// </param>
		/// <returns>
		/// The <see cref="LongTextStreamResult"/> instance.
		/// </returns>
		/// <exception cref="OperationCanceledException">
		/// Thrown when cancellation is requested.
		/// </exception>
		Task<LongTextStreamResult> SaveLongTextStreamAsync(
			LongTextStreamRequest request,
			CancellationToken token,
			IProgress<LongTextStreamProgressEventArgs> progress);
	}
}