// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ObjectManagerExceptionHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Defines a static helper method to inspect common Object Manager exceptions.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;

	using Relativity.Services.Exceptions;

	/// <summary>
	/// Defines a static helper method to inspect common Object Manager exceptions.
	/// </summary>
	internal static class ObjectManagerExceptionHelper
	{
		/// <summary>
		/// Inspects the Object Manager exception to determine if the error is non-fatal including invalid request artifacts and server-side directory/path/permission issues.
		/// </summary>
		/// <param name="exception">
		/// The exception to evaluate.
		/// </param>
		/// <returns>
		/// <see langword="true"/> indicates the exception is non-fatal and expected; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsNonFatalError(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			bool result = IsInvalidParametersError(exception) || IsServerSideDirectoryNotFoundError(exception)
			                                                  || IsServerSideFileNotFoundError(exception)
			                                                  || IsServerSideFilePermissionsError(exception);
			return result;
		}

		/// <summary>
		/// Inspects the Object Manager exception to determine if the error is due to invalid request parameters such as non-existent artifacts.
		/// </summary>
		/// <param name="exception">
		/// The exception to evaluate.
		/// </param>
		/// <returns>
		/// <see langword="true"/> indicates the exception is the expected error; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsInvalidParametersError(Exception exception)
		{
			if (exception == null)
			{
				return false;
			}

			// Be careful with invalid artifacts because DG/SQL timeouts can contain this message within a larger message and those should be retried.
			const string Pattern = "^Read Failed$";
			bool result = exception.GetType() == typeof(InvalidInputException) ||
			              exception.GetType() == typeof(ValidationException) ||
			              IsMatch(exception, Pattern);
			return result;
		}

		/// <summary>
		/// Inspects the Object Manager exception to determine if the error is due to the server-side directory not found.
		/// </summary>
		/// <param name="exception">
		/// The exception to evaluate.
		/// </param>
		/// <returns>
		/// <see langword="true"/> indicates the exception is the expected error; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsServerSideDirectoryNotFoundError(Exception exception)
		{
			if (exception == null)
			{
				return false;
			}

			const string Pattern = @"Could not find a part of the path '.+'";
			bool result = IsMatch(exception, Pattern);
			return result;
		}

		/// <summary>
		/// Inspects the Object Manager exception to determine if the error is due to the server-side file not found.
		/// </summary>
		/// <param name="exception">
		/// The exception to evaluate.
		/// </param>
		/// <returns>
		/// <see langword="true"/> indicates the exception is the expected error; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsServerSideFileNotFoundError(Exception exception)
		{
			if (exception == null)
			{
				return false;
			}

			const string Pattern = @"Could not find file '.+'";
			bool result = IsMatch(exception, Pattern);
			return result;
		}

		/// <summary>
		/// Inspects the Object Manager exception to determine if the error is due to server-side file permissions.
		/// </summary>
		/// <param name="exception">
		/// The exception to evaluate.
		/// </param>
		/// <returns>
		/// <see langword="true"/> indicates the exception is the expected error; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsServerSideFilePermissionsError(Exception exception)
		{
			if (exception == null)
			{
				return false;
			}

			const string Pattern = @"Access to the path '.+' is denied";
			bool result = IsMatch(exception, Pattern);
			return result;
		}

		private static bool IsMatch(Exception exception, string pattern)
		{
			if (string.IsNullOrWhiteSpace(exception.Message))
			{
				return false;
			}

			bool match = System.Text.RegularExpressions.Regex.IsMatch(
				exception.Message,
				pattern,
				System.Text.RegularExpressions.RegexOptions.IgnoreCase);
			return match;
		}
	}
}