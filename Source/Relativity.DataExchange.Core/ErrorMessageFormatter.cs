// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessageFormatter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines static methods to format error messages.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Globalization;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// Defines static methods to format error messages.
	/// </summary>
	internal static class ErrorMessageFormatter
	{
		/// <summary>
		/// Builds a web service specific retry message.
		/// </summary>
		/// <param name="serviceOperation">
		/// The web service operation that failed.
		/// </param>
		/// <param name="errorMessage">
		/// The error message.
		/// </param>
		/// <param name="waitDuration">
		/// The time period between each retry attempt.
		/// </param>
		/// <param name="retryAttempt">
		/// The retry attempt number.
		/// </param>
		/// <param name="maxRetries">
		/// The maximum number of retries.
		/// </param>
		/// <returns>
		/// The formatted message.
		/// </returns>
		public static string FormatWebServiceRetryMessage(
			string serviceOperation,
			string errorMessage,
			TimeSpan waitDuration,
			int retryAttempt,
			int maxRetries)
		{
			string message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.WebServiceNotFatalRetryMessage,
				serviceOperation,
				errorMessage);
			return AppendRetryDetails(message, waitDuration, maxRetries - retryAttempt);
		}

		/// <summary>
		/// Appends retry details to the supplied <paramref name="message"/>.
		/// </summary>
		/// <param name="message">
		/// The message to append error details.
		/// </param>
		/// <param name="waitDuration">
		/// The time period between each retry attempt.
		/// </param>
		/// <param name="retriesLeft">
		/// The total number of retries left.
		/// </param>
		/// <returns>
		/// The appended message.
		/// </returns>
		public static string AppendRetryDetails(string message, TimeSpan waitDuration, int retriesLeft)
		{
			message = message.TrimEnd(' ', '.');
			message += ". ";
			if (retriesLeft > 0)
			{
				message += string.Format(
					CultureInfo.CurrentCulture,
					Strings.RetryIssueAppendMessage,
					waitDuration.TotalSeconds,
					retriesLeft);
			}
			else
			{
				message += Strings.RetryIssueNoAttemptsLeftAppendMessage;
			}

			return message;
		}
	}
}