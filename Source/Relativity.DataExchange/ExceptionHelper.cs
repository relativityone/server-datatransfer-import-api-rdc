// ----------------------------------------------------------------------------
// <copyright file="ExceptionHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Net;
	using System.Security;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Web.Services.Protocols;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// Defines static helper methods to perform common exception handling.
	/// </summary>
	internal static class ExceptionHelper
	{
		/// <summary>
		/// The disk full HResult value.
		/// </summary>
		public const int DiskFullHResultHResult = unchecked((int)0x80070070);

		/// <summary>
		/// The handle disk full HResult value.
		/// </summary>
		public const int HandleDiskFullHResult = unchecked((int)0x80070027);

		/// <summary>
		/// The illegal characters in path constant message.
		/// </summary>
		public const string IllegalCharactersInPathMessage = "Illegal characters in path.";

		/// <summary>
		/// A fatal exception message that tells user to try again and contact an admin if the problem persists.
		/// </summary>
		/// <remarks>
		/// This generic message suffix was provided by the docs team.
		/// </remarks>
		public const string TryAgainAdminFatalMessage = "Try again. If the problem persists please contact your system administrator for assistance.";

		/// <summary>
		/// The list of fatal exception candidates.
		/// </summary>
		public static readonly List<Type> FatalExceptionCandidates = new List<Type>(
			new[]
				{
					typeof(AccessViolationException), typeof(ApplicationException), typeof(BadImageFormatException),
					typeof(DivideByZeroException), typeof(DllNotFoundException), typeof(EntryPointNotFoundException),
					typeof(InsufficientMemoryException), typeof(NullReferenceException), typeof(OutOfMemoryException),
					typeof(OverflowException), typeof(SecurityException), typeof(StackOverflowException),
					typeof(ThreadAbortException),
				});

		/// <summary>
		/// The list of path exception candidates.
		/// </summary>
		public static readonly List<Type> PathExceptionCandidates = new List<Type>(
			new[]
				{
					typeof(DirectoryNotFoundException), typeof(PathTooLongException), typeof(UnauthorizedAccessException),
				});

		/// <summary>
		/// Represents the thread synchronization object.
		/// </summary>
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// Gets the default HTTP status codes that are considered fatal [BadRequest,Forbidden,Unauthorized].
		/// </summary>
		private static IEnumerable<HttpStatusCode> DefaultFatalHttpStatusCodes =>
			new List<HttpStatusCode>(
				new[]
					{
						HttpStatusCode.BadRequest,
						HttpStatusCode.NotFound,
						HttpStatusCode.Forbidden,
						HttpStatusCode.Unauthorized,
					});

		/// <summary>
		/// Gets the default web exception status codes that are considered fatal [TrustFailure].
		/// </summary>
		private static IEnumerable<WebExceptionStatus> DefaultFatalWebExceptionStatusCodes =>
			new List<WebExceptionStatus>(
				new[]
					{
						WebExceptionStatus.TrustFailure,
					});

		/// <summary>
		/// Appends <see cref="TryAgainAdminFatalMessage"/> to the fatal exception message and returns a new message.
		/// </summary>
		/// <param name="message">
		/// The exception message to append.
		/// </param>
		/// <returns>
		/// The appended exception message.
		/// </returns>
		public static string AppendTryAgainAdminFatalMessage(string message)
		{
			if (!string.IsNullOrEmpty(message))
			{
				message = message.TrimEnd(' ', '.') + ". ";
			}

			return $"{message}{TryAgainAdminFatalMessage}";
		}

		/// <summary>
		/// Gets the detailed fatal message for the specified HTTP status code.
		/// </summary>
		/// <param name="statusCode">
		/// The status code used to retrieve the fatal message.
		/// </param>
		/// <returns>
		/// The fatal error message.
		/// </returns>
		public static string GetDetailedFatalMessage(HttpStatusCode statusCode)
		{
			switch (statusCode)
			{
				case HttpStatusCode.BadRequest:
					return Strings.HttpBadRequestFatalMessage;
				case HttpStatusCode.NotFound:
					return Strings.HttpNotFoundFatalMessage;
				case HttpStatusCode.Forbidden:
					return Strings.HttpForbiddenFatalMessage;
				case HttpStatusCode.Unauthorized:
					return Strings.HttpUnauthorizedFatalMessage;
				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Gets the detailed fatal message for the specified web exception status code.
		/// </summary>
		/// <param name="status">
		/// The status used to retrieve the fatal message.
		/// </param>
		/// <returns>
		/// The fatal error message.
		/// </returns>
		public static string GetDetailedFatalMessage(WebExceptionStatus status)
		{
			switch (status)
			{
				case WebExceptionStatus.TrustFailure:
					return Strings.WebExceptionTrustFailureMessage;
				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Determines whether the SOAP exception is thrown when an endpoint is not found.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the endpoint is not found; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsEndpointNotFound(SoapException exception)
		{
			return IsEndpointNotFound(exception, string.Empty);
		}

		/// <summary>
		/// Determines whether the SOAP exception is thrown when an endpoint is not found.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="name">
		/// The optional web-service method name. When specified, the check only returns <see langword="true" /> if the name appears within the exception.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the endpoint is not found; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsEndpointNotFound(SoapException exception, string name)
		{
			return IsSoapEndpointNotFound(exception, name);
		}

		/// <summary>
		/// Determines whether the <paramref name="exception"/> is considered fatal.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>
		/// <see langword="true"/> indicates the exception is fatal; otherwise, <see langword="false"/>.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="exception"/> is <see langword="null"/>.
		/// </exception>
		public static bool IsFatalException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			lock (SyncRoot)
			{
				var exceptionType = exception.GetType();
				return FatalExceptionCandidates.Any(exceptionCandidateType => exceptionType == exceptionCandidateType)
				       || IsFatalWebException(exception) || IsOutOfDiskSpaceException(exception);
			}
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
		/// Gets a value indicating whether the specified exception is considered fatal.
		/// </summary>
		/// <param name="exception">
		/// The exception to check.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the exception is considered fatal; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsFatalWebException(Exception exception)
		{
			return IsFatalWebException(exception as WebException);
		}

		/// <summary>
		/// Gets a value indicating whether the specified web exception is considered fatal.
		/// </summary>
		/// <param name="exception">
		/// The web exception to check.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the exception is considered fatal; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsFatalWebException(WebException exception)
		{
			HttpStatusCode? status = (exception?.Response as HttpWebResponse)?.StatusCode;
			bool result = status != null && IsHttpStatusCodeFatalError(status.Value);
			return result;
		}

		/// <summary>
		/// Gets a value indicating whether the specified HTTP status code is a fatal error.
		/// </summary>
		/// <param name="statusCode">
		/// The HTTP status code to check.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the HTTP status code is a fatal error; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsHttpStatusCodeFatalError(HttpStatusCode statusCode)
		{
			return DefaultFatalHttpStatusCodes.Any(x => x == statusCode);
		}

		/// <summary>
		/// Determines whether the exception is a Path exception.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>
		///   <c>true</c> if the exception is a Path exception; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsPathException(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			lock (SyncRoot)
			{
				var exceptionType = exception.GetType();
				return PathExceptionCandidates.Any(exceptionCandidateType => exceptionType == exceptionCandidateType);
			}
		}

		/// <summary>
		/// Determines whether the SOAP exception is thrown when an endpoint is not found.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the endpoint is not found; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsSoapEndpointNotFound(Exception exception)
		{
			return IsSoapEndpointNotFound(exception, string.Empty);
		}

		/// <summary>
		/// Determines whether the SOAP exception is thrown when an endpoint is not found.
		/// </summary>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="name">
		/// The optional web-service method name. When specified, the check only returns <see langword="true" /> if the name appears within the exception.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the endpoint is not found; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsSoapEndpointNotFound(Exception exception, string name)
		{
			if (string.IsNullOrWhiteSpace(exception.Message))
			{
				return false;
			}

			bool headerResult = exception.Message.IndexOf(
									"Server did not recognize the value of HTTP Header",
									StringComparison.CurrentCultureIgnoreCase) >= 0;
			bool methodResult = string.IsNullOrEmpty(name)
								|| exception.Message.IndexOf(name, StringComparison.CurrentCultureIgnoreCase) >= 0;
			return headerResult && methodResult;
		}

		/// <summary>
		/// Determines whether the exception indicates an HTTP timeout.
		/// </summary>
		/// <param name="exception">The exception.</param>
		/// <returns>
		/// <see langword="true"/> indicates the exception is a suspected timeout; otherwise, <see langword="false"/>.
		/// </returns>
		public static bool IsSuspectedHttpTimeoutException(Exception exception)
		{
			var e = exception as AggregateException;
			if (e == null || !(e.InnerException is TaskCanceledException))
			{
				return false;
			}

			return !((TaskCanceledException)e.InnerException).CancellationToken.IsCancellationRequested;
		}

		/// <summary>
		/// Gets a value indicating whether the specified web exception status code is a fatal error.
		/// </summary>
		/// <param name="status">
		/// The web exception status code to check.
		/// </param>
		/// <returns>
		/// <see langword="true" /> when the web exception status code is a fatal error; otherwise, <see langword="false" />.
		/// </returns>
		public static bool IsWebExceptionStatusCodeFatalError(WebExceptionStatus status)
		{
			return DefaultFatalWebExceptionStatusCodes.Any(x => x == status);
		}
	}
}