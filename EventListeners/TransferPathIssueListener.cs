// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransferPathIssueListener.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System.Globalization;

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
            // Note: this issue is indicative of a job-level issue - especially with Aspera.
            if (e.Issue.Path == null)
            {
                // Don't raise a warning here because you cannot associate it with a line number.
                this.TransferLog.LogWarning(
                    "A transfer warning has occurred. LineNumber={LineNumber}, SourcePath={SourcePath}, Attributes={Attributes}.",
                    TapiConstants.NoLineNumber,
                    "(no path)",
                    e.Issue.Attributes);
                return;
            }

            var lineNumber = e.Issue.Path.Order;
            var retryCalculation = e.Request.RetryStrategy.Calculation;
            var retryTimeSpan = retryCalculation(e.Issue.RetryAttempt);
            var triesLeft = e.Issue.MaxRetryAttempts - e.Issue.RetryAttempt - 1;

            // TODO: Cleanup this logic - it's way too complicated.
            if (e.Issue.Attributes.HasFlag(IssueAttributes.Error))
            {
                this.TransferLog.LogError(
                    "A transfer error has occurred. Message={Message}, LineNumber={LineNumber}, SourcePath={SourcePath}, Attributes={Attributes}.",
                    e.Issue.Message,
                    lineNumber,
                    e.Issue.Path.SourcePath,
                    e.Issue.Attributes);
            }
            else if (triesLeft > 0)
            {
                var formattedMessage = this.transferDirection == TransferDirection.Download
                    ? Strings.TransferFileDownloadWarningMessage
                    : Strings.TransferFileUploadWarningMessage;
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    formattedMessage,
                    this.clientName,
                    e.Issue.Message,
                    retryTimeSpan.TotalSeconds,
                    triesLeft);
                this.RaiseWarningMessage(message, e.Issue.Path.Order);
                this.TransferLog.LogWarning(
                    "A transfer warning has occurred. Message={Message}, LineNumber={LineNumber}, SourcePath={SourcePath}, Attributes={Attributes}.",
                    e.Issue.Message,
                    lineNumber,
                    e.Issue.Path.SourcePath,
                    e.Issue.Attributes);
            }
            else
            {
                // Avoid raising this as a warning. The request will now terminate and messaging is handled in NativeFileTransfer.
                this.TransferLog.LogError(
                    "A transfer error has occurred. Message={Message}, LineNumber={LineNumber}, SourcePath={SourcePath}, Attributes={Attributes}.",
                    e.Issue.Message,
                    lineNumber,
                    e.Issue.Path.SourcePath,
                    e.Issue.Attributes);
            }
        }       
    }
}