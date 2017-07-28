// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferPathIssueListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using System.Globalization;
    using System.Linq;

    using kCura.WinEDDS.TApi.Resources;

    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Listens for transfer path issue events.
    /// </summary>
    public class TransferPathIssueListener : TransferListenerBase
    {
        /// <summary>
        /// The transfer direction.
        /// </summary>
        private readonly TransferDirection transferDirection;

        /// <summary>
        /// The client name.
        /// </summary>
        private readonly string clientName;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferPathIssueListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="direction">
        /// The transfer direction.
        /// </param>
        /// <param name="clientName">
        /// The client name.
        /// </param>
        public TransferPathIssueListener(ILog log, TransferDirection direction, string clientName) : base(log)
        {
            this.transferDirection = direction;
            this.clientName = clientName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferPathIssueListener"/> class. 
        /// </summary>
        /// <param name="log">
        /// The transfer log.
        /// </param>
        /// <param name="direction">
        /// The transfer direction.
        /// </param>
        /// <param name="clientName">
        /// The client name.
        /// </param>
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferPathIssueListener(ILog log, TransferDirection direction, string clientName, TransferContext context) : base(log, context)
        {
            this.transferDirection = direction;
            this.clientName = clientName;
        }

        /// <summary>
        /// Occurs when there is a fatal error in the transfer job.
        /// </summary>
        public event EventHandler<TransferMessageEventArgs> FatalError = delegate { };

        /// <inheritdoc />
        protected override void OnTransferPathIssue(object sender, TransferPathIssueEventArgs e)
        {
            var formattedMessage = this.transferDirection == TransferDirection.Download
                                       ? Strings.TransferFileDownloadIssueMessage
                                       : Strings.TransferFileUploadIssueMessage;
            var message = string.Format(CultureInfo.CurrentCulture, formattedMessage, this.clientName, e.Issue.Message);
            var fatalIssues = new[]
                                  {
                                      IssueAttributes.Error,
                                      IssueAttributes.StorageOutOfSpace,
                                      IssueAttributes.Licensing
                                  };
            var fatal = fatalIssues.Any(x => e.Issue.Attributes.HasFlag(x));
            if (fatal)
            {
                this.TransferLog.LogError("A serious transfer error has occurred. Issue={Issue}.", e.Issue);
                this.RaiseFatalError($"A serious transfer error has occurred. Issue={e.Issue}.", e.Issue.Path != null ? e.Issue.Path.Order : TApiConstants.NO_LINE);
            }
            else if (e.Issue.Attributes.HasFlag(IssueAttributes.Warning))
            {
                this.TransferLog.LogWarning("A transfer warning has occurred. Issue={Issue}.", e.Issue);
                this.RaiseWarningMessage(message, e.Issue.Path != null ? e.Issue.Path.Order : TApiConstants.NO_LINE);
            }
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
        private void RaiseFatalError(string message, int lineNumber)
        {
            this.FatalError.Invoke(this, new TransferMessageEventArgs(message, lineNumber));
        }
    }
}
