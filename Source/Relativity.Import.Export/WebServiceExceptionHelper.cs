// ----------------------------------------------------------------------------
// <copyright file="WebServiceExceptionHelper.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Web.Services.Protocols;

	using Relativity.Import.Export.Resources;

	/// <summary>
	/// Defines static helper methods to perform common web-service exception handling.
	/// </summary>
	internal static class WebServiceExceptionHelper
	{
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
	}
}