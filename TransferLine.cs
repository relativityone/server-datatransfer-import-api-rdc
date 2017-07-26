// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferLine.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

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
        public void Print()
        {
            var line = String.Format(
                "Line: {0}, Source: {1}, Status: {2}",
                this.Path.Order,
                this.Path.SourcePath,
                this.TransferStatus);

            if (this.TransferStatus == TransferPathStatus.Successful)
            {
                line = String.Concat(line, $", Duration: {this.End - this.Start}");
            }

            if (this.RetryCount > 0)
            {
                line = String.Concat(line, $" Retries: {this.RetryCount}");
            }

            Console.WriteLine(
                line,
                this.Path.Order,
                this.Path.SourcePath,
                this.TransferStatus,
                this.End - this.Start,
                this.RetryCount);
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
