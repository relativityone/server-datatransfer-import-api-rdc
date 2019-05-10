// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiStatisticsEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines Transfer API statistics event arguments data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;

	/// <summary>
	/// Represents Transfer API statistics event arguments data. This class cannot be inherited.
	/// </summary>
	/// <seealso cref="System.EventArgs" />
	public sealed class TapiStatisticsEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TapiStatisticsEventArgs"/> class.
		/// </summary>
		/// <param name="totalBytes">
		/// The total number of bytes.
		/// </param>
		/// <param name="totalFiles">
		/// The total number of files.
		/// </param>
		/// <param name="totalTransferTicks">
		/// The total transfer time in ticks.
		/// </param>
		/// <param name="transferRateBytes">
		/// The active transfer rate.
		/// </param>
		public TapiStatisticsEventArgs(
			long totalBytes,
			long totalFiles,
			long totalTransferTicks,
			double transferRateBytes)
		{
			this.TotalBytes = totalBytes;
			this.TotalFiles = totalFiles;
			this.TotalTransferTicks = totalTransferTicks;
			this.TransferRateBytes = transferRateBytes;
		}

		/// <summary>
		/// Gets the total transferred bytes.
		/// </summary>
		/// <value>
		/// The total transferred bytes.
		/// </value>
		public long TotalBytes
		{
			get;
		}

		/// <summary>
		/// Gets the total transferred files.
		/// </summary>
		/// <value>
		/// The total transferred files.
		/// </value>
		public long TotalFiles
		{
			get;
		}

		/// <summary>
		/// Gets the total transfer time in ticks.
		/// </summary>
		/// <value>
		/// The transfer time in ticks.
		/// </value>
		public long TotalTransferTicks
		{
			get;
		}

		/// <summary>
		/// Gets active transfer rate.
		/// </summary>
		public double TransferRateBytes
		{
			get;
		}
	}
}