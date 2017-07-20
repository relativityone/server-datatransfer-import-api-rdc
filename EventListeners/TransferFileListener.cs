// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferFileListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Listens for transfer file events.
    /// </summary>
    public class TransferFileListener : TransferListenerBase
    {
        /// <summary>
        /// The transfer lines.
        /// </summary>
        private readonly ConcurrentDictionary<int, TransferLine> transferLines;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferFileListener"/> class.
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        public TransferFileListener(ILog log) : base(log)
        {
            this.transferLines = new ConcurrentDictionary<int, TransferLine>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferFileListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferFileListener(ILog log, TransferContext context)
            : base(log, context)
        {
            this.transferLines = new ConcurrentDictionary<int, TransferLine>();
        }

        /// <summary>
        /// Occurs when a file has finished transferring.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> ProgressEvent = delegate { };

        /// <summary>
        /// Dumps transfer stats for each line.
        /// </summary>
        public void Dump()
        {
            foreach (KeyValuePair<int, TransferLine> entry in this.transferLines.OrderBy(entry => entry.Key))
            {
                entry.Value.Print();
            }

            this.transferLines.Clear();
        }

        /// <inheritdoc />
        protected override void OnTransferFileEvent(object sender, TransferFileEventArgs e)
        {
            if (!this.transferLines.ContainsKey(e.Path.Order))
            {
                this.transferLines[e.Path.Order] = new TransferLine(e.Path);
            }

            this.transferLines[e.Path.Order].Update(e);

            switch (e.Status)
            {
                case TransferFileStatus.Failed:
                    this.TransferLog.LogError($"Failed to transfer file. Path={e.Path.SourcePath}.", e.Path.Order);
                    break;

                case TransferFileStatus.FailedRetryable:
                    this.TransferLog.LogError(
                        $"Failed to transfer file. Path={e.Path.SourcePath} and will re-queue after the current job is complete.",
                        e.Path.Order);
                    break;

                case TransferFileStatus.Started:
                    this.TransferLog.LogInformation($"Starting file transfer. Path={e.Path.SourcePath}.", e.Path.Order);
                    break;

                case TransferFileStatus.Successful:
                    this.TransferLog.LogInformation(
                        $"Successfully transferred file. Path={e.Path.SourcePath}.",
                        e.Path.Order);
                    this.RaiseProgressEvent(string.Empty, e.Path.Order);
                    break;
            }
        }

        /// <summary>
        /// Raises a progress event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        private void RaiseProgressEvent(string message, int lineNumber)
        {
            this.ProgressEvent.Invoke(this, new TransferMessageEventArgs(message, lineNumber));
        }
    }
}
