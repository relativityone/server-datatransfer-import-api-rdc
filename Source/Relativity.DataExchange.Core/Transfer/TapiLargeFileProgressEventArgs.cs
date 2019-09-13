// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiLargeFileProgressEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines the Transfer API path completed event arguments data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;

	using Relativity.Transfer;

	/// <summary>
	/// Represents a Transfer API transfer large file progress event arguments data. This class cannot be inherited.
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public sealed class TapiLargeFileProgressEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiLargeFileProgressEventArgs"/> class.
		/// </summary>
		/// <param name="path">
		/// The path that is being transferred.
		/// </param>
		/// <param name="totalRequestBytes">
		/// The total number of request bytes.
		/// </param>
		/// <param name="totalTransferredBytes">
		/// The total number of transferred bytes.
		/// </param>
		/// <param name="progress">
		/// The transfer progress.
		/// </param>
		public TapiLargeFileProgressEventArgs(
			TransferPath path,
			long totalRequestBytes,
			long totalTransferredBytes,
			double progress)
		{
			this.Path = path;
			this.Progress = progress;
			this.TotalRequestBytes = totalRequestBytes;
			this.TotalTransferredBytes = totalTransferredBytes;
		}

		/// <summary>
		/// Gets the transfer progress.
		/// </summary>
		public double Progress
		{
			get;
		}

		/// <summary>
		/// Gets the total number of request bytes.
		/// </summary>
		public long TotalRequestBytes
		{
			get;
		}

		/// <summary>
		/// Gets the total number of transferred bytes.
		/// </summary>
		public long TotalTransferredBytes
		{
			get;
		}

		/// <summary>
		/// Gets the line number.
		/// </summary>
		public int LineNumber => this.Path.Order;

		/// <summary>
		/// Gets the transfer path.
		/// </summary>
		public TransferPath Path
		{
			get;
		}
	}
}