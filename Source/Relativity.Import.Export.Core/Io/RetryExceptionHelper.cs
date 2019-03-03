// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetryExceptionHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Defines commonly used static retry helper functions to decide whether an exception should be retried.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
    using System;

	/// <summary>
	/// Defines commonly used static retry helper functions to decide whether an exception should be retried.
	/// </summary>
	internal static class RetryExceptionHelper
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
		/// Creates a retry predicate that uses the specified options and exception to determine whether to retry the operation.
		/// </summary>
		/// <param name="options">
		/// The retry options.
		/// </param>
		/// <returns>
		/// The predicate.
		/// </returns>
		public static Func<Exception, bool> CreateRetryPredicate(RetryOptions options)
		{
			Func<Exception, bool> retryPredicate = (exception) => IsRetryable(exception, options);
			return retryPredicate;
		}

		/// <summary>
		/// Determines whether the specified exception can be retried.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="options">
		/// The retry options.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the exception can be retried; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="exception"/> is <see langword="null" />.
		/// </exception>
		public static bool IsRetryable(Exception exception, RetryOptions options)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			if (exception is System.UnauthorizedAccessException || exception is System.Security.SecurityException)
			{
				return options.HasFlag(RetryOptions.Permissions);
			}

			if (exception is FileInfoInvalidPathException || IsIllegalCharactersInPathException(exception))
			{
				// This is not configurable because it can never succeed.
				return false;
			}

			if (exception is System.IO.FileNotFoundException)
			{
				return options.HasFlag(RetryOptions.FileNotFound);
			}

			if (exception is System.IO.DirectoryNotFoundException)
			{
				return options.HasFlag(RetryOptions.DirectoryNotFound);
			}

			if (exception is System.IO.PathTooLongException)
			{
				// This is not configurable because it can never succeed.
				return false;
			}

			if (IsOutOfDiskSpaceException(exception))
			{
				return options.HasFlag(RetryOptions.DiskFull);
			}

			if (exception is System.IO.IOException)
			{
				return options.HasFlag(RetryOptions.Io);
			}

			return false;
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
		public static bool IsIllegalCharactersInPathException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			return exception is ArgumentException && exception.Message.Contains(IllegalCharactersInPathMessage);
		}
	}
}