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
	using System.Collections.Generic;

	using Relativity.Services.Exceptions;

	/// <summary>
	/// Defines a static helper method to inspect common Object Manager exceptions.
	/// </summary>
	/// <remarks>
	/// Unfortunately, both Kepler and Object Manager don't provide clear error details to determine whether an exception is
	/// both expected and retryable. The <see cref="IsNonFatalError"/> method is designed to inspect a limited number of
	/// well-known exception types, traverse inner exceptions, and search within the deserialized <see cref="ServiceException.ErrorDetails"/>
	/// to find expected .NET exception types. The primary goal is to improve UX by not retrying errors that can never succeed and prevent
	/// the export from spinning unnecessarily.
	/// </remarks>
	internal static class ObjectManagerExceptionHelper
	{
		/// <summary>
		/// Defines the max depth to search for exceptions.
		/// </summary>
		/// <remarks>
		/// As a sanity check, limit the number of levels. In practice, the target exceptions types are found within the first 2-3 levels.
		/// </remarks>
		public const int MaxInnerExceptionDepth = 16;

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

			bool result = IsInvalidParametersError(exception) || IsServerSideFileNotFoundError(exception)
			                                                  || IsServerSideFilePermissionsError(exception)
			                                                  || IsServerSideDirectoryNotFoundError(exception);
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
			bool result = exception is ValidationException || IsMatch(exception, Pattern);
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

			bool result = TrySearchServiceExceptionErrorDetails(
				exception,
				errorDetailsDictionary => IsInnerExceptionClassName(
					errorDetailsDictionary,
					typeof(System.IO.DirectoryNotFoundException)));
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

			bool result = TrySearchServiceExceptionErrorDetails(
				exception,
				errorDetailsDictionary => IsInnerExceptionClassName(
					errorDetailsDictionary,
					typeof(System.IO.FileNotFoundException)));
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

			bool result = TrySearchServiceExceptionErrorDetails(
				exception,
				errorDetailsDictionary => IsInnerExceptionClassName(
					errorDetailsDictionary,
					typeof(System.UnauthorizedAccessException)));
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

		/// <summary>
		/// Searches the exception and all inner exceptions up to the max depth using the supplied inner exception function.
		/// </summary>
		/// <param name="exception">
		/// The exception to inspect.
		/// </param>
		/// <param name="innerExceptionFunc">
		/// The function called that determines whether the search criteria has been met.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the search is successful; otherwise, <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// This method was originally using <see cref="Newtonsoft"/> to serialize the exception to a JSON string and perform a basic
		/// string search. Unfortunately, performance was incredibly slow and may be due to using such a legacy version - almost 5 years old as of this writing.
		/// The implementation now leverages from the dictionary that's already supplied when the object has been serialized by Kepler.
		/// </remarks>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "By design, this method will never fail.")]
		private static bool TrySearchServiceExceptionErrorDetails(Exception exception, Func<IDictionary<string, object>, bool> innerExceptionFunc)
		{
			try
			{
				// Note: Kepler serializes the ServiceException.ErrorDetails property with an ExpandoObject
				ServiceException serviceException = exception as ServiceException;
				if (serviceException == null)
				{
					return false;
				}

				// The dictionary cast is for easy fetches to KV pairs.
				IDictionary<string, object> currentErrorDetailsDictionary = serviceException.ErrorDetails;
				const string InnerExceptionKey = "InnerException";
				int innerExceptionLevel = 0;
				while (currentErrorDetailsDictionary != null && innerExceptionLevel < MaxInnerExceptionDepth)
				{
					innerExceptionLevel++;
					if (innerExceptionFunc(currentErrorDetailsDictionary))
					{
						return true;
					}

					if (!currentErrorDetailsDictionary.ContainsKey(InnerExceptionKey))
					{
						break;
					}

					currentErrorDetailsDictionary = currentErrorDetailsDictionary[InnerExceptionKey] as IDictionary<string, object>;
				}

				return false;
			}
			catch
			{
				return false;
			}
		}

		private static bool IsInnerExceptionClassName(IDictionary<string, object> errorDetailsDictionary, Type expectedType)
		{
			const string ClassNameKey = "ClassName";
			if (!errorDetailsDictionary.ContainsKey(ClassNameKey))
			{
				return false;
			}

			string value = errorDetailsDictionary.GetStringValue(ClassNameKey, string.Empty);
			bool result = !string.IsNullOrWhiteSpace(value) && value.IndexOf(
				              expectedType.Name,
				              StringComparison.OrdinalIgnoreCase) >= 0;
			return result;
		}
	}
}