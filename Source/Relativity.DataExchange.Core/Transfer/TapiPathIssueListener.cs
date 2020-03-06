// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TapiPathIssueListener.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Text;

	using Relativity.DataExchange.Logger;
	using Relativity.DataExchange.Resources;
	using Relativity.Logging;
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
		/// <param name="logger">
		/// The Relativity logger instance.
		/// </param>
		/// <param name="direction">
		/// The transfer direction.
		/// </param>
		/// <param name="context">
		/// The transfer context.
		/// </param>
		public TapiPathIssueListener(ILog logger, TransferDirection direction, TransferContext context)
			: base(logger, context)
		{
			this.transferDirection = direction;
		}

		/// <summary>
		/// Formats a job-level or file-level message using the supplied issue parameters.
		/// </summary>
		/// <param name="clientDisplayName">
		/// The client name.
		/// </param>
		/// <param name="message">
		/// The issue message.
		/// </param>
		/// <param name="direction">
		/// The transfer direction.
		/// </param>
		/// <param name="jobLevelIssue">
		/// Specify whether this is a job-level or file-level issue.
		/// </param>
		/// <param name="retryable">
		/// Specify whether this is a retryable issue.
		/// </param>
		/// <param name="retriesLeft">
		/// The number of retries left.
		/// </param>
		/// <param name="retryTimeSpan">
		/// The wait period between retry attempts.
		/// </param>
		/// <returns>
		/// The overall message.
		/// </returns>
		internal static string FormatIssueMessage(
			string clientDisplayName,
			string message,
			TransferDirection direction,
			bool jobLevelIssue,
			bool retryable,
			int retriesLeft,
			TimeSpan retryTimeSpan)
		{
			if (!string.IsNullOrEmpty(message))
			{
				message = message.TrimEnd('.');
			}

			StringBuilder sb = new StringBuilder();
			if (jobLevelIssue)
			{
				sb.AppendFormat(
					direction == TransferDirection.Upload
						? Strings.TransferTransferUploadJobWarningMessage
						: Strings.TransferTransferDownloadJobWarningMessage,
					clientDisplayName,
					message);
			}
			else
			{
				sb.AppendFormat(
					direction == TransferDirection.Upload
						? Strings.TransferTransferUploadFileWarningMessage
						: Strings.TransferTransferDownloadFileWarningMessage,
					clientDisplayName,
					message);
			}

			sb.Append(".");
			if (!retryable)
			{
				return sb.ToString();
			}

			return ErrorMessageFormatter.AppendRetryDetails(sb.ToString(), retryTimeSpan, retriesLeft);
		}

		/// <inheritdoc />
		protected override void OnTransferPathIssue(object sender, TransferPathIssueEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			int triesLeft = CalculateTriesLeft(e);
			TimeSpan retryTimeSpan = CalculateRetryTimeSpan(e);

			this.HandleFileLevelIssue(e.Issue, triesLeft, retryTimeSpan);
		}

		/// <inheritdoc />
		protected override void OnTransferJobIssue(object sender, TransferPathIssueEventArgs e)
		{
			if (e == null)
			{
				throw new ArgumentNullException(nameof(e));
			}

			int triesLeft = CalculateTriesLeft(e);
			TimeSpan retryTimeSpan = CalculateRetryTimeSpan(e);

			this.HandleJobLevelIssue(e.Issue, triesLeft, retryTimeSpan);
		}

		private static int CalculateTriesLeft(TransferPathIssueEventArgs e)
		{
			return e.Issue.MaxRetryAttempts - e.Issue.RetryAttempt - 1;
		}

		private static TimeSpan CalculateRetryTimeSpan(TransferPathIssueEventArgs e)
		{
			Func<int, TimeSpan> retryCalculation = e.Request.RetryStrategy.Calculation;
			TimeSpan retryTimeSpan = retryCalculation(e.Issue.RetryAttempt);
			return retryTimeSpan;
		}

		private void HandleJobLevelIssue(ITransferIssue issue, int triesLeft, TimeSpan retryTimeSpan)
		{
			if (issue.Attributes.HasFlag(IssueAttributes.Error))
			{
				this.Logger.LogError(
					"A serious transfer job-level error has occurred. Message={Message}, Code={Code}, Attributes={Attributes}, IsRetryable={IsRetryable}",
					issue.Message,
					issue.Code,
					issue.Attributes,
					issue.IsRetryable);
			}
			else
			{
				const bool JobLevelIssue = true;
				string message = FormatIssueMessage(
					this.ClientDisplayName,
					issue.Message,
					this.transferDirection,
					JobLevelIssue,
					issue.IsRetryable,
					triesLeft,
					retryTimeSpan);
				this.PublishWarningMessage(message, TapiConstants.NoLineNumber);
				this.Logger.LogWarning(
					"A transfer job-level warning has occurred. Message={Message}, Code={Code}, Attributes={Attributes}, IsRetryable={IsRetryable}.",
					issue.Message,
					issue.Code,
					issue.Attributes,
					issue.IsRetryable);
			}
		}

		private void HandleFileLevelIssue(ITransferIssue issue, int triesLeft, TimeSpan retryTimeSpan)
		{
			int lineNumber = issue.Path.Order;
			if (issue.Attributes.HasFlag(IssueAttributes.Error))
			{
				// Note: paths containing fatal errors force the transfer to terminate
				//       and error handling is already addressed. Log it here just in case.
				this.Logger.LogError(
					"A serious transfer file-level error has occurred on line {LineNumber}. Message={Message}, Code={Code}, SourcePath={SourcePath}, Attributes={Attributes}, IsRetryable={IsRetryable}",
					lineNumber,
					issue.Message,
					issue.Code,
					issue.Path.SourcePath.Secure(),
					issue.Attributes,
					issue.IsRetryable);
			}
			else
			{
				const bool JobLevelIssue = false;
				string message = FormatIssueMessage(
					this.ClientDisplayName,
					issue.Message,
					this.transferDirection,
					JobLevelIssue,
					issue.IsRetryable,
					triesLeft,
					retryTimeSpan);
				this.PublishWarningMessage(message, issue.Path.Order);
				this.Logger.LogWarning(
					"A transfer file-level warning has occurred on line {LineNumber}. Message={Message}, Code={Code}, SourcePath={SourcePath}, Attributes={Attributes}, IsRetryable={IsRetryable}",
					lineNumber,
					issue.Message,
					issue.Code,
					issue.Path.SourcePath.Secure(),
					issue.Attributes,
					issue.IsRetryable);
			}
		}
	}
}