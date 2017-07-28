// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferPathListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Listens for transfer path events.
    /// </summary>
    public class TransferPathListener : TransferListenerBase
    {
        /// <summary>
        /// The transfer lines.
        /// </summary>
        private readonly ConcurrentDictionary<int, TransferLine> transferLines;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferPathListener"/> class.
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        public TransferPathListener(ILog log) : base(log)
        {
            this.transferLines = new ConcurrentDictionary<int, TransferLine>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferPathListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferPathListener(ILog log, TransferContext context)
            : base(log, context)
        {
            this.transferLines = new ConcurrentDictionary<int, TransferLine>();
        }

        /// <summary>
        /// Occurs when a file has finished transferring.
        /// </summary>
        public event EventHandler<TransferProgressEventArgs> ProgressEvent = delegate { };

        /// <summary>
        /// Gets stats for line.
        /// </summary>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <returns>
        /// The <see cref="IDictionary"/>.
        /// </returns>
        public IDictionary PullStatsForLine(int lineNumber)
        {
            TransferLine line;
            this.transferLines.TryRemove(lineNumber, out line);
            return line.ToDictionary();
        }

        /// <inheritdoc />
        protected override void OnTransferPathProgress(object sender, TransferPathProgressEventArgs e)
        {
            if (!this.transferLines.ContainsKey(e.Path.Order))
            {
                this.transferLines[e.Path.Order] = new TransferLine(e.Path);
            }

            var line = this.transferLines[e.Path.Order];
            line.TransferStatus = e.Status;

            if (e.StartTime.HasValue && e.EndTime.HasValue)
            {
                line.FileTime = e.EndTime - e.StartTime;
                this.RaiseProgressEvent(e.Path.Order, e.BytesTransferred, line.FileTime.Value.Ticks);
            }

            switch (e.Status)
            {
                case TransferPathStatus.Failed:
                    this.TransferLog.LogError($"Failed to transfer file. Path={e.Path.SourcePath}.", e.Path.Order);
                    break;

                case TransferPathStatus.FailedRetryable:
                    this.TransferLog.LogError(
                        $"Failed to transfer file. Path={e.Path.SourcePath} and will re-queue after the current job is complete.",
                        e.Path.Order);
                    break;

                case TransferPathStatus.Started:
                    this.TransferLog.LogInformation($"Starting file transfer. Path={e.Path.SourcePath}.", e.Path.Order);
                    break;

                case TransferPathStatus.Successful:
                    this.TransferLog.LogInformation(
                        $"Successfully transferred file. Path={e.Path.SourcePath}.",
                        e.Path.Order);
                    break;
            }
        }

        /// <summary>
        /// Raises a progress event.
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
        private void RaiseProgressEvent(int lineNumber, long fileBytes, long fileTime)
        {
            this.ProgressEvent.Invoke(this, new TransferProgressEventArgs(lineNumber, fileBytes, fileTime));
        }
    }
}
