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
            this.RetryCount = 0;
        }

        /// <summary>
        /// Gets or sets the start.
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// Gets or sets the end.
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// Gets or sets the retry count.
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets the transfer path status.
        /// </summary>
        public TransferPathStatus TransferStatus { get; internal set; }

        /// <summary>
        /// Gets or sets the transfer path.
        /// </summary>
        public TransferPath Path { get; set; }

        /// <summary>
        /// Print transfer statistics to console.
        /// </summary>
        public IDictionary ToDictionary()
        {
            var retval = new HybridDictionary();
            retval.Add("Source Path", this.Path.SourcePath);
            retval.Add("Transfer Status", this.TransferStatus);
            retval.Add("Retry Count", this.RetryCount);
            if (this.End != null)
            {
                retval.Add("File Time", this.End.Ticks - this.Start.Ticks);
            }

            return retval;
        }

        /// <summary>
        /// Updates statistics for the artifact.
        /// </summary>
        /// <param name="e">
        /// The <see cref="TransferPathProgressEventArgs"/> instance containing the event data.
        /// </param>
        public void Update(TransferPathProgressEventArgs e)
        {
            switch (e.Status)
            {
                case TransferPathStatus.Failed:
                    this.End = DateTime.Now;
                    break;

                case TransferPathStatus.FailedRetryable:
                    this.RetryCount++;
                    break;

                case TransferPathStatus.Started:
                    this.Start = e.StartTime ?? DateTime.Now;
                    break;

                case TransferPathStatus.Successful:
                    this.End = e.EndTime ?? DateTime.Now;
                    break;
            }

            this.TransferStatus = e.Status;
        }
    }
}
