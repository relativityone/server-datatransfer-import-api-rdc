// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferListenerBase.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Base class for transfer event listeners.
    /// </summary>
    public abstract class TransferListenerBase : IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TransferListenerBase"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        protected TransferListenerBase(ILog log)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            this.TransferLog = log;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferListenerBase"/> class. 
        /// </summary>
         /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        protected TransferListenerBase(ILog log, TransferContext context) : this(log)
        {
            this.RegisterContext(context);
        }

        /// <summary>
        /// Occurs when a status message is available.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> StatusMessage = delegate { };

        /// <summary>
        /// Occurs when a warning message is available.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> WarningMessage = delegate { };

        /// <summary>
        /// Gets or sets the transfer context. 
        /// </summary>
        protected TransferContext Context { get; set; }

        /// <summary>
        /// Gets the transfer log.
        /// </summary>
        protected ILog TransferLog { get; }

        /// <summary>
        /// Registers events for the transfer context.
        /// </summary>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public void RegisterContext(TransferContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.Context = context;
            this.Context.TransferFile += this.OnTransferFileEvent;
            this.Context.TransferFileIssue += this.OnTransferFileIssueEvent;
            this.Context.TransferRequest += this.OnTransferRequestEvent;
            this.Context.TransferJobRetry += this.OnTransferJobRetryEvent;
            this.Context.TransferStatistics += this.OnTransferStatisticsEvent;
        }

        /// <summary>
        /// Raises a status message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        public void RaiseStatusMessage(string message, int lineNumber)
        {
            this.StatusMessage.Invoke(this, new TransferMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Raises a warning message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        public void RaiseWarningMessage(string message, int lineNumber)
        {
            this.WarningMessage.Invoke(this, new TransferMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Unsubscribe to events and dispose.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.Context == null)
            {
                return;
            }

            this.Context.TransferFile -= this.OnTransferFileEvent;
            this.Context.TransferFileIssue -= this.OnTransferFileIssueEvent;
            this.Context.TransferRequest -= this.OnTransferRequestEvent;
            this.Context.TransferJobRetry -= this.OnTransferJobRetryEvent;
            this.Context.TransferStatistics -= this.OnTransferStatisticsEvent;
        }

        /// <summary>
        /// Transfer File event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferFileEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnTransferFileEvent(object sender, TransferFileEventArgs e)
        {
        }

        /// <summary>
        /// Transfer File Issue event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferFileIssueEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnTransferFileIssueEvent(object sender, TransferFileIssueEventArgs e)
        {
        }

        /// <summary>
        /// Transfer Request event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferRequestEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnTransferRequestEvent(object sender, TransferRequestEventArgs e)
        {
        }

        /// <summary>
        /// Transfer Job Retry event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferJobRetryEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnTransferJobRetryEvent(object sender, TransferJobRetryEventArgs e)
        {
        }

        /// <summary>
        /// Transfer Statistics event handler.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferStatisticsEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnTransferStatisticsEvent(object sender, TransferStatisticsEventArgs e)
        {
        }
    }
}
