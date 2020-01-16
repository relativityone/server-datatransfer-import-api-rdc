// -----------------------------------------------------------------------------------------------------
// <copyright file="ErrorMessageFormatterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ErrorMessageFormatter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.Collections;
	using System.Text;

	using global::NUnit.Framework;

	/// <summary>
	/// Represents <see cref="ErrorMessageFormatter"/> tests.
	/// </summary>
	[TestFixture]
	public class ErrorMessageFormatterTests
	{
		private static IEnumerable WebServiceRetryTestCases
		{
			get
			{
				string expected1 =
					"A non-fatal issue occurred attempting to call the 'Ping' web service: Endpoint does not exist. Retrying in 30 second(s) - 9 retry(s) left.";
				yield return new TestCaseData("Ping", "Endpoint does not exist", 30, 1, 10, expected1);
				yield return new TestCaseData("Ping", "Endpoint does not exist.", 30, 1, 10, expected1);
				string expected2 =
					"A non-fatal issue occurred attempting to call the 'Ping' web service: Endpoint does not exist. Retrying in 10 second(s) - 1 retry(s) left.";
				yield return new TestCaseData("Ping", "Endpoint does not exist", 10, 9, 10, expected2);
				yield return new TestCaseData("Ping", "Endpoint does not exist.", 10, 9, 10, expected2);
				string expected3 =
					"A non-fatal issue occurred attempting to call the 'Ping' web service: Endpoint does not exist. No more retry attempts left.";
				yield return new TestCaseData("Ping", "Endpoint does not exist", 10, 10, 10, expected3);
				yield return new TestCaseData("Ping", "Endpoint does not exist.", 10, 10, 10, expected3);
			}
		}

		private static IEnumerable AppendRetryDetailsTestCases
		{
			get
			{
				string expected1 = "The file does not exist. Retrying in 30 second(s) - 5 retry(s) left.";
				yield return new TestCaseData("The file does not exist.", 30, 5, expected1);
				string expected2 = "The file does not exist. No more retry attempts left.";
				yield return new TestCaseData("The file does not exist.", 10, 0, expected2);
			}
		}

		[TestCaseSource(nameof(WebServiceRetryTestCases))]
		public void ShouldFormatTheWebServiceRetryMessage(
			string serviceOperation,
			string errorMessage,
			int durationSeconds,
			int retryAttempt,
			int maxRetries,
			string expectedMessage)
		{
			string formattedMessage = ErrorMessageFormatter.FormatWebServiceRetryMessage(
				serviceOperation,
				errorMessage,
				TimeSpan.FromSeconds(durationSeconds),
				retryAttempt,
				maxRetries);
			Assert.That(formattedMessage, Is.EqualTo(expectedMessage));
		}

		[TestCaseSource(nameof(AppendRetryDetailsTestCases))]
		public void ShouldAppendTheRetryDetailsToTheMessage(
			string message,
			int durationSeconds,
			int retriesLeft,
			string expectedMessage)
		{
			string appendedMessage = ErrorMessageFormatter.AppendRetryDetails(
				message,
				TimeSpan.FromSeconds(durationSeconds),
				retriesLeft);
			Assert.That(appendedMessage, Is.EqualTo(expectedMessage));
		}
	}
}