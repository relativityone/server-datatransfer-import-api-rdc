// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferStatisticsEventArgs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines TAPI statistics event arguments data.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    /// <summary>
    /// Represents TAPI statistics event arguments data.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TapiStatisticsEventArgs : EventArgs
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
        public TapiStatisticsEventArgs(
            long totalBytes,
            long totalFiles,
            long totalTransferTicks)
        {
            this.TotalBytes = totalBytes;
            this.TotalFiles = totalFiles;
            this.TotalTransferTicks = totalTransferTicks;
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
    }
}