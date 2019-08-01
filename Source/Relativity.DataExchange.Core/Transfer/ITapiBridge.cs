// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITapiBridge.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to provide Transfer API object services to the transfer bridges.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;

	using Relativity.Transfer;

	/// <summary>
	/// Represents an abstract object to provide Transfer API object services to the transfer bridges.
	/// </summary>
	public interface ITapiBridge : IDisposable
	{
		/// <summary>
		/// Occurs when a status message is available.
		/// </summary>
		event EventHandler<TapiMessageEventArgs> TapiStatusMessage;

		/// <summary>
		/// Occurs when a non-fatal error message is available.
		/// </summary>
		event EventHandler<TapiMessageEventArgs> TapiErrorMessage;

		/// <summary>
		/// Occurs when a warning message is available.
		/// </summary>
		event EventHandler<TapiMessageEventArgs> TapiWarningMessage;

		/// <summary>
		/// Occurs when the transfer client is changed.
		/// </summary>
		event EventHandler<TapiClientEventArgs> TapiClientChanged;

		/// <summary>
		/// Occurs when a path transfer progress has started or completed.
		/// </summary>
		/// <remarks>
		/// This event is never raised as the file is being transferred.
		/// </remarks>
		event EventHandler<TapiProgressEventArgs> TapiProgress;

		/// <summary>
		/// Occurs when a large file transfer is in progress.
		/// </summary>
		event EventHandler<TapiLargeFileProgressEventArgs> TapiLargeFileProgress;

		/// <summary>
		/// Occurs when transfer statistics are available.
		/// </summary>
		event EventHandler<TapiStatisticsEventArgs> TapiStatistics;

		/// <summary>
		/// Occurs when there is a fatal error in the transfer.
		/// </summary>
		event EventHandler<TapiMessageEventArgs> TapiFatalError;

		/// <summary>
		/// Gets the current transfer client.
		/// </summary>
		/// <value>
		/// The <see cref="TapiClient"/> value.
		/// </value>
		TapiClient Client
		{
			get;
		}

		/// <summary>
		/// Gets the transfer bridge instance unique identifier. This value is assigned for the lifecycle of each instance, regardless of whether jobs are batched or in web fallback mode. It's recommended to include this value with verbose logs where per-instance details may provide added insight.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		Guid InstanceId
		{
			get;
		}

		/// <summary>
		/// Gets the transfer totals for the current job.
		/// </summary>
		/// <value>
		/// The <see cref="TapiTotals"/> instance.
		/// </value>
		TapiTotals JobTotals
		{
			get;
		}

		/// <summary>
		/// Gets the transfer parameters.
		/// </summary>
		/// <value>
		/// The <see cref="TapiBridgeParameters2"/> instance.
		/// </value>
		TapiBridgeParameters2 Parameters
		{
			get;
		}

		/// <summary>
		/// Adds the path to a transfer job.
		/// </summary>
		/// <param name="path">
		/// The path to add to the job.
		/// </param>
		/// <returns>
		/// The file name.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="path" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the transfer job wasn't successfully created.
		/// </exception>
		/// <exception cref="OperationCanceledException">
		/// Thrown when cancellation is requested.
		/// </exception>
		/// <exception cref="TransferException">
		/// Thrown when a fatal transfer exception occurs.
		/// </exception>
		string AddPath(TransferPath path);

		/// <summary>
		/// Waits for all pending transfers in the queue to complete and returns the transfer totals.
		/// </summary>
		/// <param name="waitMessage">
		/// The message that's published when the wait begins.
		/// </param>
		/// <param name="successMessage">
		/// The message that's published when the pending transfers are successful.
		/// </param>
		/// <param name="errorMessage">
		/// The message that's published when the pending transfers fail.
		/// </param>
		/// <param name="keepJobAlive">
		/// <see langword="true" /> to enable an optimization that keeps the transfer job alive once all file transfers have completed; otherwise, <see langword="false" /> to destroy the job once all file transfers have completed.
		/// </param>
		/// <returns>
		/// The <see cref="TapiTotals"/> instance.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown when the transfer job wasn't successfully created.
		/// </exception>
		/// <exception cref="OperationCanceledException">
		/// Thrown when cancellation is requested.
		/// </exception>
		/// <exception cref="TransferException">
		/// Thrown when a fatal transfer exception occurs.
		/// </exception>
		TapiTotals WaitForTransfers(string waitMessage, string successMessage, string errorMessage, bool keepJobAlive);
	}
}