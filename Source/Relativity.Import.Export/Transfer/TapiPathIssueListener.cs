// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiPathIssueListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Transfer
{
	using System;
	using System.Globalization;

	using Relativity.Import.Export.Resources;
	using Relativity.Transfer;

	/// <summary>
	/// Represents an object that listens for Transfer API path issue events. This class cannot be inherited.
	/// </summary>
	internal sealed class TapiPathIssueListener : TapiListenerBase
	{
		/// <summary>
		/// The transfer direction.
		/// </summary>
		private readonly TransferDirection transferDirection;

		/// <summary>
		/// Initializes a new instance of the <see cref="TapiPathIssueListener"/> class.
		/// </summary>
		/// <param name="log">
		/// The transfer log.
		/// </param>
		/// <param name="direction">
		/// The transfer direction.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		public TapiPathIssueListener(ITransferLog log, TransferDirection direction, TransferContext context)
			: base(log, context)
		{
			this.transferDirection = direction;
		}

		/// <inheritdoc />
		protected override void OnTransferPathIssue(object sender, TransferPathIssueEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			var triesLeft = e.Issue.MaxRetryAttempts - e.Issue.RetryAttempt - 1;
			var retryCalculation = e.Request.RetryStrategy.Calculation;
			var retryTimeSpan = retryCalculation(e.Issue.RetryAttempt);

			// Note: this issue is indicative of a job-level issue - especially with Aspera.
			if (e.Issue.Path == null)
			{
				var formattedMessage = this.transferDirection == TransferDirection.Download
					? Strings.TransferJobDownloadWarningMessage
					: Strings.TransferJobUploadWarningMessage;
				var message = string.Format(
					CultureInfo.CurrentCulture,
					formattedMessage,
					this.ClientDisplayName,
					e.Issue.Message,
					retryTimeSpan.TotalSeconds,
					triesLeft);
				this.PublishWarningMessage(message, TapiConstants.NoLineNumber);
				this.TransferLog.LogWarning(
					"A transfer warning has occurred. LineNumber={LineNumber}, SourcePath={SourcePath}, Attributes={Attributes}.",
					TapiConstants.NoLineNumber,
					"(no path)",
					e.Issue.Attributes);
				return;
			}

			var lineNumber = e.Issue.Path.Order;
			if (e.Issue.Attributes.HasFlag(IssueAttributes.Error))
			{
				// Note: paths containing fatal errors force the transfer to terminate
				//       and error handling is already addressed. Log it here just in case.
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
					this.ClientDisplayName,
					e.Issue.Message,
					retryTimeSpan.TotalSeconds,
					triesLeft);
				this.PublishWarningMessage(message, e.Issue.Path.Order);
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