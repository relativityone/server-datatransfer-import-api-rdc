// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferPathListener.cs" company="kCura Corp">
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
        protected override void OnTransferPathProgress(object sender, TransferPathProgressEventArgs e)
        {
            if (!this.transferLines.ContainsKey(e.Path.Order))
            {
                this.transferLines[e.Path.Order] = new TransferLine(e.Path);
            }

            this.transferLines[e.Path.Order].Update(e);

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
