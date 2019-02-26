// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiListenerBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
    using System;

    using Relativity.Import.Export.Resources;
	using Relativity.Transfer;

    /// <summary>
    /// Base class for transfer event listeners.
    /// </summary>
    public abstract class TapiListenerBase : IDisposable
    {
        /// <summary>
        /// The disposed backing.
        /// </summary>
        private bool disposed;

		/// <summary>
		/// The client display name backing.
		/// </summary>
		private string clientDisplayName;

	    /// <summary>
        /// Initializes a new instance of the <see cref="TapiListenerBase"/> class.
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        protected TapiListenerBase(ITransferLog log, TransferContext context)
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
            this.Context.LargeFileProgress += this.OnLargeFileProgress;
            this.Context.TransferPathProgress += this.OnTransferPathProgress;
            this.Context.TransferPathIssue += this.OnTransferPathIssue;
            this.Context.TransferRequest += this.OnTransferRequestEvent;
            this.Context.TransferJobRetry += this.OnTransferJobRetryEvent;
            this.Context.TransferStatistics += this.OnTransferStatisticsEvent;
        }

        /// <summary>
        /// Occurs when a fatal error is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> FatalError;

        /// <summary>
        /// Occurs when a non-fatal error message is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> ErrorMessage;

        /// <summary>
        /// Occurs when a warning message is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> WarningMessage;

        /// <summary>
        /// Occurs when a status message is registered.
        /// </summary>
        public event EventHandler<TapiMessageEventArgs> StatusMessage;

	    /// <summary>
	    /// Gets or sets the transfer client display name.
	    /// </summary>
	    public string ClientDisplayName
	    {
		    get
		    {
			    return !string.IsNullOrEmpty(this.clientDisplayName) ? this.clientDisplayName : Strings.ClientInitializing;
		    }

		    set
		    {
			    this.clientDisplayName = value;
			}
	    }

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
        protected ITransferLog TransferLog
        {
            get;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
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
            this.StatusMessage?.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
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
            this.ErrorMessage?.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
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
            this.WarningMessage?.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
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
            this.FatalError?.Invoke(this, new TapiMessageEventArgs(message, lineNumber));
        }

        /// <summary>
        /// Occurs when large path transfer progress occurs.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="LargeFileProgressEventArgs"/> instance containing the event data.
        /// </param>
        protected virtual void OnLargeFileProgress(object sender, LargeFileProgressEventArgs e)
        {
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

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.
        /// </param>
        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing && this.Context != null)
            {
                this.Context.LargeFileProgress -= this.OnLargeFileProgress;
                this.Context.TransferPathProgress -= this.OnTransferPathProgress;
                this.Context.TransferPathIssue -= this.OnTransferPathIssue;
                this.Context.TransferRequest -= this.OnTransferRequestEvent;
                this.Context.TransferJobRetry -= this.OnTransferJobRetryEvent;
                this.Context.TransferStatistics -= this.OnTransferStatisticsEvent;
            }

            this.disposed = true;
        }
    }
}