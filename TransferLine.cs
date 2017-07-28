// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferLine.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;

    using Relativity.Transfer;

    /// <summary>
    /// Class to keep track of transfer statistics for each artifact.
    /// </summary>
    public class TransferLine
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferLine"/> class.
        /// </summary>
        /// <param name="transferPath">
        /// The transfer path.
        /// </param>
        public TransferLine(TransferPath transferPath)
        {
            this.Path = transferPath;
            this.TransferStatus = TransferPathStatus.None;
        }

        /// <summary>
        /// Gets or sets the file time.
        /// </summary>
        public TimeSpan? FileTime { get; set; }

        /// <summary>
        /// Gets or sets the transfer path status.
        /// </summary>
        public TransferPathStatus TransferStatus { get; set; }

        /// <summary>
        /// Gets or sets the transfer path.
        /// </summary>
        public TransferPath Path { get; set; }

        /// <summary>
        /// Get the transfer stats for the line.
        /// </summary>
        /// <returns>
        /// The stats in key-value pairs.
        /// </returns>
        public IDictionary ToDictionary()
        {
            var retval = new HybridDictionary();
            retval.Add("Source Path", this.Path.SourcePath);
            retval.Add("Transfer Status", this.TransferStatus);
            if (this.FileTime.HasValue)
            {
                retval.Add("File Time", this.FileTime.Value);
            }

            return retval;
        }
    }
}
