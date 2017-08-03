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
        /// <param name="context">
        /// The transfer context.
        /// </param>
        public TransferPathIssueListener(ILog log, TransferDirection direction, string clientName, TransferContext context)
            : base(log, context)
        {
            this.transferDirection = direction;
            this.clientName = clientName;
        }

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

            // Treat warnings as errors as soon as we reach the max retry attempt.
            var fatal = fatalIssues.Any(x => e.Issue.Attributes.HasFlag(x))
                        || e.Issue.RetryAttempt == e.Issue.MaxRetryAttempts;
            if (fatal)
            {
                this.TransferLog.LogError(
                    "A serious transfer error has occurred. LineNumber={LineNumber}, SourcePath={SourcePath}, Attributes={Attributes}.",
                    e.Issue.Path != null ? e.Issue.Path.Order : TapiConstants.NoLineNumber,
                    e.Issue.Path != null ? e.Issue.Path.SourcePath : "(no path)",
                    e.Issue.Attributes);
                this.RaiseFatalError(message, e.Issue.Path != null ? e.Issue.Path.Order : TapiConstants.NoLineNumber);
            }
            else if (e.Issue.Attributes.HasFlag(IssueAttributes.Warning))
            {
                this.TransferLog.LogWarning(
                    "A transfer warning has occurred. LineNumber={LineNumber}, SourcePath={SourcePath}, Attributes={Attributes}.",
                    e.Issue.Path != null ? e.Issue.Path.Order : TapiConstants.NoLineNumber,
                    e.Issue.Path != null ? e.Issue.Path.SourcePath : "(no path)",
                    e.Issue.Attributes);
                this.RaiseWarningMessage(message, e.Issue.Path != null ? e.Issue.Path.Order : TapiConstants.NoLineNumber);
            }
        }       
    }
}