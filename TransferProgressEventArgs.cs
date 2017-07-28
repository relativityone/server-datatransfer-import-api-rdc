// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferStatisticsEventArgs.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Defines the transfer progress event arguments.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    /// <summary>
    /// Represents the transfer progress event arguments.
    /// </summary>
    /// <seealso cref="System.EventArgs" />
    public class TransferProgressEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferProgressEventArgs"/> class.
        /// </summary>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <param name="fileBytes">
        /// The file bytes.
        /// </param>
        /// <param name="fileTime">
        /// The file time.
        /// </param>
        public TransferProgressEventArgs(int lineNumber, long fileBytes, long fileTime)
        {
            this.LineNumber = lineNumber;
            this.FileBytes = fileBytes;
            this.FileTime = fileTime;
        }

        /// <summary>
        /// Gets the line number.
        /// </summary>
        public int LineNumber
        {
            get;
        }

        /// <summary>
        /// Gets the total transferred bytes.
        /// </summary>
        public long FileBytes
        {
            get;
        }

        /// <summary>
        /// Gets the file time.
        /// </summary>
        public long FileTime
        {
            get;
        }
    }
}