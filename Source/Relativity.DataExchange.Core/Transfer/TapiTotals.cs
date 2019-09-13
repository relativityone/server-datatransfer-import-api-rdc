// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiTotals.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract object to provide Transfer API object services to the transfer bridges.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System.Threading;

	/// <summary>
	/// Represents a class object that provides Transfer API totals. This class cannot be inherited.
	/// </summary>
	public sealed class TapiTotals
	{
		/// <summary>
		/// The total number of transfer request paths.
		/// </summary>
		private long totalFileTransferRequests;

		/// <summary>
		/// The total number of transfer request paths that have been completed.
		/// </summary>
		private long totalCompletedFileTransfers;

		/// <summary>
		/// The total number of transfer request paths that were successfully transferred.
		/// </summary>
		private long totalSuccessfulFileTransfers;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiTotals"/> class.
		/// </summary>
		public TapiTotals()
			: this(0, 0, 0)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiTotals"/> class.
		/// </summary>
		/// <param name="totalCompletedFileTransfers">
		/// The total number of completed file transfers.
		/// </param>
		/// <param name="totalFileTransferRequests">
		/// The total number of file transfer requests.
		/// </param>
		/// <param name="totalSuccessfulFileTransfers">
		/// The total number of successful file transfers.
		/// </param>
		public TapiTotals(
			long totalCompletedFileTransfers,
			long totalFileTransferRequests,
			long totalSuccessfulFileTransfers)
		{
			this.totalCompletedFileTransfers = totalCompletedFileTransfers;
			this.totalFileTransferRequests = totalFileTransferRequests;
			this.totalSuccessfulFileTransfers = totalSuccessfulFileTransfers;
		}

		/// <summary>
		/// Gets the total number of files contained within the job or batch request.
		/// </summary>
		/// <value>
		/// The total number of files.
		/// </value>
		public long TotalFileTransferRequests => this.totalFileTransferRequests;

		/// <summary>
		/// Gets the total number of completed file transfers. A file transfer is considered to be complete when the transfer is either successful or failed without further retry.
		/// </summary>
		/// <value>
		/// The total number of files.
		/// </value>
		public long TotalCompletedFileTransfers => this.totalCompletedFileTransfers;

		/// <summary>
		/// Gets the total number of successful file transfers.
		/// </summary>
		/// <value>
		/// The total number of files.
		/// </value>
		public long TotalSuccessfulFileTransfers => this.totalSuccessfulFileTransfers;

		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			Interlocked.Exchange(ref this.totalCompletedFileTransfers, 0);
			Interlocked.Exchange(ref this.totalFileTransferRequests, 0);
			Interlocked.Exchange(ref this.totalSuccessfulFileTransfers, 0);
		}

		/// <summary>
		/// Increments the total number of completed file transfers.
		/// </summary>
		public void IncrementTotalCompletedFileTransfers()
		{
			Interlocked.Increment(ref this.totalCompletedFileTransfers);
		}

		/// <summary>
		/// Increments the total number of file transfer requests.
		/// </summary>
		public void IncrementTotalFileTransferRequests()
		{
			Interlocked.Increment(ref this.totalFileTransferRequests);
		}

		/// <summary>
		/// Increments the total number of files that were successfully transferred.
		/// </summary>
		public void IncrementTotalSuccessfulFileTransfers()
		{
			Interlocked.Increment(ref this.totalSuccessfulFileTransfers);
		}

		/// <summary>
		/// Performs a deep copy of the current instance.
		/// </summary>
		/// <returns>
		/// The <see cref="TapiTotals"/> instance.
		/// </returns>
		public TapiTotals DeepCopy()
		{
			TapiTotals copy = new TapiTotals(
				this.TotalCompletedFileTransfers,
				this.TotalFileTransferRequests,
				this.TotalSuccessfulFileTransfers);
			return copy;
		}
	}
}