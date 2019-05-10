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

			if (exception is FileInfoInvalidPathException || ExceptionHelper.IsIllegalCharactersInPathException(exception))
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

			if (ExceptionHelper.IsOutOfDiskSpaceException(exception))
			{
				return options.HasFlag(RetryOptions.DiskFull);
			}

			if (exception is System.IO.IOException)
			{
				return options.HasFlag(RetryOptions.Io);
			}

			return false;
		}
	}
}