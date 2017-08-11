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
        /// <param name="context">
        /// The transfer context.
        /// </param>
        protected TransferListenerBase(ILog log, TransferContext context)
        {
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            this.TransferLog = log;
            this.Context = context;
            this.Context.TransferPathProgress += this.OnTransferPathProgress;
            this.Context.TransferPathIssue += this.OnTransferPathIssue;
            this.Context.TransferRequest += this.OnTransferRequestEvent;
            this.Context.TransferJobRetry += this.OnTransferJobRetryEvent;
            this.Context.TransferStatistics += this.OnTransferStatisticsEvent;
        }

        /// <summary>
        /// Occurs when a fatal error is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> FatalError = delegate { };

        /// <summary>
        /// Occurs when a non-fatal error message is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> ErrorMessage = delegate { };

        /// <summary>
        /// Occurs when a warning message is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> WarningMessage = delegate { };

        /// <summary>
        /// Occurs when a status message is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> StatusMessage = delegate { };

        /// <summary>
        /// Gets or sets the transfer context. 
        /// </summary>
        protected TransferContext Context
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the transfer log.
        /// </summary>
        protected ILog TransferLog
        {
            get;
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

            this.Context.TransferPathProgress -= this.OnTransferPathProgress;
            this.Context.TransferPathIssue -= this.OnTransferPathIssue;
            this.Context.TransferRequest -= this.OnTransferRequestEvent;
            this.Context.TransferJobRetry -= this.OnTransferJobRetryEvent;
            this.Context.TransferStatistics -= this.OnTransferStatisticsEvent;
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
        protected void RaiseStatusMessage(string message, int lineNumber)
        {
            this.StatusMessage.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Raises an error message event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        protected void RaiseErrorMessage(string message, int lineNumber)
        {
            this.ErrorMessage.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
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
        protected void RaiseWarningMessage(string message, int lineNumber)
        {
            this.WarningMessage.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Raises a fatal error event.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        protected void RaiseFatalError(string message, int lineNumber)
        {
            this.FatalError.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Occurs when path transfer progress starts or ends.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferPathProgressEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnTransferPathProgress(object sender, TransferPathProgressEventArgs e)
        {
        }

        /// <summary>
        /// Occurs when an issue occurs transferring a file.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="TransferPathIssueEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnTransferPathIssue(object sender, TransferPathIssueEventArgs e)
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