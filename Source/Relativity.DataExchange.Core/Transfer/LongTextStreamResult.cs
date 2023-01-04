// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LongTextStreamResult.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object with long text stream result details.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;

	/// <summary>
	/// Represents a class object with long text stream result details.
	/// </summary>
	internal class LongTextStreamResult
	{
		private LongTextStreamResult(
			LongTextStreamRequest request,
			string file,
			long length,
			int retryCount,
			TimeSpan duration,
			Exception issue)
		{
			this.Request = request.ThrowIfNull(nameof(request));
			this.File = file;
			this.Length = length;
			this.RetryCount = retryCount;
			this.Duration = duration;
			this.Issue = issue;
		}

		/// <summary>
		/// Gets the duration to retrieve the long text data.
		/// </summary>
		/// <value>
		/// The <see cref="TimeSpan"/> value.
		/// </value>
		public TimeSpan Duration
		{
			get;
		}

		/// <summary>
		/// Gets the full path to the written extracted text file.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string File
		{
			get;
		}

		/// <summary>
		/// Gets the written extracted text file name.
		/// </summary>
		/// <value>
		/// The file name.
		/// </value>
		public string FileName =>
			!string.IsNullOrWhiteSpace(this.File) ? System.IO.Path.GetFileName(this.File) : string.Empty;

		/// <summary>
		/// Gets the non-fatal issue for the specified request.
		/// </summary>
		/// <value>
		/// The <see cref="Exception"/> instance.
		/// </value>
		public Exception Issue
		{
			get;
		}

		/// <summary>
		/// Gets the length, in bytes, of the target file.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		public long Length
		{
			get;
		}

		/// <summary>
		/// Gets the long text stream request.
		/// </summary>
		/// <value>
		/// The <see cref="LongTextStreamRequest"/> instance.
		/// </value>
		public LongTextStreamRequest Request
		{
			get;
		}

		/// <summary>
		/// Gets the total number of retry attempts.
		/// </summary>
		/// <value>
		/// The retry count.
		/// </value>
		public int RetryCount
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether the long text stream transfer is successful.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when successful; otherwise, <see langword="false" />.
		/// </value>
		public bool Successful => this.Issue == null;

		/// <summary>
		/// Creates a new long text result for non-fatal errors.
		/// </summary>
		/// <param name="request">
		/// The long text request.
		/// </param>
		/// <param name="retryCount">
		/// The total number of retry attempts.
		/// </param>
		/// <param name="issue">
		/// The non-fatal issue.
		/// </param>
		/// <returns>
		/// The <see cref="LongTextStreamResult"/> instance.
		/// </returns>
		public static LongTextStreamResult CreateNonFatalIssueResult(
			LongTextStreamRequest request,
			int retryCount,
			Exception issue)
		{
			return new LongTextStreamResult(request, null, 0, retryCount, TimeSpan.Zero, issue);
		}

		/// <summary>
		/// Creates a new long text result for successful transfers.
		/// </summary>
		/// <param name="request">
		/// The long text request.
		/// </param>
		/// <param name="file">
		/// The full path to the target file.
		/// </param>
		/// <param name="length">
		/// The length in bytes of the target file.
		/// </param>
		/// <param name="retryCount">
		/// The total number of retry attempts.
		/// </param>
		/// <param name="duration">
		/// The duration to retrieve the long text data.
		/// </param>
		/// <returns>
		/// The <see cref="LongTextStreamResult"/> instance.
		/// </returns>
		public static LongTextStreamResult CreateSuccessfulResult(
			LongTextStreamRequest request,
			string file,
			long length,
			int retryCount,
			TimeSpan duration)
		{
			return new LongTextStreamResult(request, file, length, retryCount, duration, null);
		}
	}
}