// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferStatisticsEventArgs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the transfer statistics event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    using Relativity.Transfer;

    /// <summary>
    /// Represents the transfer message event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TransferStatisticsAvailableEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferStatisticsAvailableEventArgs"/> class.
        /// </summary>
        /// <param name="statistics">
        /// The statistics.
        /// </param>
        public TransferStatisticsAvailableEventArgs(ITransferStatistics statistics)
        {
            this.TotalBytes = statistics.TotalTransferredBytes;
            this.TotalFiles = statistics.TotalTransferredFiles;
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
    }
}