// -----------------------------------------------------------------------------------------------------
// <copyright file="TapiIssueMessageTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents transfer issue message tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Transfer;
	using Relativity.Transfer;

	/// <summary>
	/// Represents transfer issue message tests.
	/// </summary>
	[TestFixture]
	public class TapiIssueMessageTests
	{
		[Test]
		[TestCase("The file does not exist")]
		[TestCase("The file does not exist.")]
		public void ShouldFormatTheNonRetryableFileLevelDownloadIssueMessage(string message)
		{
			string issueMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Download,
				false,
				false,
				0,
				TimeSpan.Zero);
			Assert.That(issueMessage, Is.EqualTo("Direct download file issue: The file does not exist."));
		}

		[Test]
		[TestCase("The filename is invalid")]
		[TestCase("The filename is invalid.")]
		public void ShouldFormatTheRetryableFileLevelDownloadIssueMessage(string message)
		{
			string issueWithAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Download,
				false,
				true,
				5,
				TimeSpan.FromSeconds(10));
			string issueWithoutAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Download,
				false,
				true,
				0,
				TimeSpan.FromSeconds(10));
			Assert.That(issueWithAttemptsMessage, Is.EqualTo("Direct download file issue: The filename is invalid. Retrying in 10 second(s) - 5 retry(s) left."));
			Assert.That(issueWithoutAttemptsMessage, Is.EqualTo("Direct download file issue: The filename is invalid. No more retry attempts left."));
		}

		[Test]
		[TestCase("The file is in use by another process")]
		[TestCase("The file is in use by another process.")]
		public void ShouldFormatTheNonRetryableFileLevelUploadIssueMessage(string message)
		{
			string issueMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Upload,
				false,
				false,
				0,
				TimeSpan.Zero);
			Assert.That(issueMessage, Is.EqualTo("Direct upload file issue: The file is in use by another process."));
		}

		[Test]
		[TestCase("The file path is too long")]
		[TestCase("The file path is too long.")]
		public void ShouldFormatTheRetryableFileLevelUploadIssueMessage(string message)
		{
			string issueWithAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Upload,
				false,
				true,
				15,
				TimeSpan.FromSeconds(3));
			string issueWithoutAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Upload,
				false,
				true,
				0,
				TimeSpan.FromSeconds(3));
			Assert.That(issueWithAttemptsMessage, Is.EqualTo("Direct upload file issue: The file path is too long. Retrying in 3 second(s) - 15 retry(s) left."));
			Assert.That(issueWithoutAttemptsMessage, Is.EqualTo("Direct upload file issue: The file path is too long. No more retry attempts left."));
		}

		[Test]
		[TestCase("The server is not responding")]
		[TestCase("The server is not responding.")]
		public void ShouldFormatTheNonRetryableJobLevelDownloadIssueMessage(string message)
		{
			string issueMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Download,
				true,
				false,
				0,
				TimeSpan.Zero);
			Assert.That(issueMessage, Is.EqualTo("Direct download transfer job issue: The server is not responding."));
		}

		[Test]
		[TestCase("The server host is invalid")]
		[TestCase("The server host is invalid.")]
		public void ShouldFormatTheRetryableJobLevelDownloadIssueMessage(string message)
		{
			string issueWithAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Download,
				true,
				true,
				4,
				TimeSpan.FromSeconds(1));
			string issueWithoutAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Download,
				true,
				true,
				0,
				TimeSpan.FromSeconds(1));
			Assert.That(issueWithAttemptsMessage, Is.EqualTo("Direct download transfer job issue: The server host is invalid. Retrying in 1 second(s) - 4 retry(s) left."));
			Assert.That(issueWithoutAttemptsMessage, Is.EqualTo("Direct download transfer job issue: The server host is invalid. No more retry attempts left."));
		}

		[Test]
		[TestCase("The 30 second timeout has been exceeded")]
		[TestCase("The 30 second timeout has been exceeded.")]
		public void ShouldFormatTheNonRetryableJobLevelUploadIssueMessage(string message)
		{
			string issueMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Upload,
				true,
				false,
				0,
				TimeSpan.Zero);
			Assert.That(issueMessage, Is.EqualTo("Direct upload transfer job issue: The 30 second timeout has been exceeded."));
		}

		[Test]
		[TestCase("The server handshake failed")]
		[TestCase("The server handshake failed.")]
		public void ShouldFormatTheRetryableJobLevelUploadIssueMessage(string message)
		{
			string issueWithAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Upload,
				true,
				true,
				1,
				TimeSpan.Zero);
			string issueWithoutAttemptsMessage = TapiPathIssueListener.FormatIssueMessage(
				"Direct",
				message,
				TransferDirection.Upload,
				true,
				true,
				0,
				TimeSpan.Zero);
			Assert.That(issueWithAttemptsMessage, Is.EqualTo("Direct upload transfer job issue: The server handshake failed. Retrying in 0 second(s) - 1 retry(s) left."));
			Assert.That(issueWithoutAttemptsMessage, Is.EqualTo("Direct upload transfer job issue: The server handshake failed. No more retry attempts left."));
		}
	}
}