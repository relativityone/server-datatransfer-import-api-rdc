// --------------------------------------------------------------------------------------------------------------------

// <copyright file="RetryExceptionPolicies.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
// Defines commonly used static function properties that decide whether an exception should be retried.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;

    /// <summary>
    /// Defines commonly used static function properties that decide whether an exception should be retried.
    /// </summary>
    public static class RetryExceptionPolicies
	{
		/// <summary>
		/// The handle disk full HResult value.
		/// </summary>
		public const int HandleDiskFullHResult = unchecked((int)0x80070027);

		/// <summary>
		/// The disk full HResult value.
		/// </summary>
		public const int DiskFullHResultHResult = unchecked((int)0x80070070);

		/// <summary>
		/// The illegal characters in path constant message.
		/// </summary>
		public const string IllegalCharactersInPathMessage = "Illegal characters in path.";

		/// <summary>
		/// Defines a standard exception retry function that returns <see langword="true" /> for <see cref="FileInfoInvalidPathException"/> and all 
		/// <see cref="System.IO.IOException"/> and derived exception types except <see cref="System.IO.FileNotFoundException"/>, 
		/// <see cref="System.IO.PathTooLongException"/>, and disk full errors.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Usage",
			"CA2211:NonConstantFieldsShouldNotBeVisible",
			Justification = "This is acceptable for defining predicates.")]
		public static Func<Exception, bool> IoStandardPolicy = IsRetryableException;

		/// <summary>
		/// Determines whether the specified exception can be retried.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the exception can be retried; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="exception"/> is <see langword="null" />.
		/// </exception>
		public static bool IsRetryableException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			// Skip to preserve existing IoReporter behavior.
			if (exception is FileInfoInvalidPathException || IsInvalidPathCharactersException(exception))
			{
				return false;
			}

			// Skip to preserve existing IAPI/RDC behavior.
			if (exception is System.IO.FileNotFoundException)
			{
				return false;
			}

			// Skip until the code base fully supports long paths.
			if (exception is System.IO.PathTooLongException)
			{
				return false;
			}

			// Exclude all non I/O errors.
			bool ioException = exception is System.IO.IOException;
			if (!ioException)
			{
				return false;
			}

			// Retry all other I/O errors except the disk-full scenario.
			return !IsOutOfDiskSpaceException(exception);
		}

		/// <summary>
		/// Determines whether the specified exception is due to running out of disk space.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the exception is due to running out of disk space; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="exception"/> is <see langword="null" />.
		/// </exception>
		public static bool IsOutOfDiskSpaceException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			bool ioException = exception is System.IO.IOException;
			if (!ioException)
			{
				return false;
			}

			// Retry all other I/O errors except the disk-full scenario.
			const int HandleDiskFull = unchecked((int)0x80070027);
			const int DiskFullHResult = unchecked((int)0x80070070);
			return exception.HResult == HandleDiskFull || exception.HResult == DiskFullHResult;
		}

		/// <summary>
		/// Determines whether the specified exception is due to illegal characters in the path.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the exception is is due to illegal characters in the path; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="exception"/> is <see langword="null" />.
		/// </exception>
		public static bool IsInvalidPathCharactersException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			return exception is ArgumentException && exception.Message.Contains(IllegalCharactersInPathMessage);
		}
	}
}