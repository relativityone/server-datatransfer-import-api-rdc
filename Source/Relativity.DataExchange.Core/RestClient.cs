// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestClient.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Globalization;
	using System.Net;
	using System.Net.Http;
	using System.Net.Http.Headers;
	using System.Text;
	using System.Threading;
	using System.Threading.Tasks;

	using Polly;

	using Relativity.DataExchange.Resources;
	using Relativity.Logging;

	/// <summary>
	/// Represents a service class object to perform REST operations used by import/export API. This class cannot be inherited.
	/// </summary>
	internal class RestClient
	{
		/// <summary>
		/// The no status code.
		/// </summary>
		private const HttpStatusCode NoHttpStatusCode = 0;

		/// <summary>
		/// The HTTP GET request method.
		/// </summary>
		private const int HttpRequestGet = 1;

		/// <summary>
		/// The HTTP POST request method.
		/// </summary>
		private const int HttpRequestPost = 2;

		/// <summary>
		/// The HTTP DELETE request method.
		/// </summary>
		private const int HttpRequestDelete = 3;

		/// <summary>
		/// The Relativity instance.
		/// </summary>
		private readonly RelativityInstanceInfo instance;

		/// <summary>
		/// The logger backing.
		/// </summary>
		private readonly ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="RestClient"/> class.
		/// </summary>
		/// <param name="instance">
		/// The Relativity instance.
		/// </param>
		/// <param name="log">
		/// The logger instance.
		/// </param>
		/// <param name="timeoutSeconds">
		/// The HTTP timeout in seconds.
		/// </param>
		/// <param name="maxRetryAttempts">
		/// The max retry attempts.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when is <see langword="null"/>.
		/// </exception>
		public RestClient(RelativityInstanceInfo instance, ILog log, double timeoutSeconds, int maxRetryAttempts)
		{
			if (instance == null)
			{
				throw new ArgumentNullException(nameof(instance));
			}

			if (log == null)
			{
				throw new ArgumentNullException(nameof(log));
			}

			this.instance = instance;
			this.logger = log;
			this.TimeoutSeconds = timeoutSeconds;
			this.MaxRetryAttempts = maxRetryAttempts;
		}

		/// <summary>
		/// Gets or sets the max retry attempts.
		/// </summary>
		public int MaxRetryAttempts
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the HTTP timeout.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		public double TimeoutSeconds
		{
			get;
		}

		/// <summary>
		/// Performs an HTTP POST and returns the string response.
		/// </summary>
		/// <param name="relativeEndpoint">
		/// The relative REST endpoint.
		/// </param>
		/// <param name="content">
		/// The content to post.
		/// </param>
		/// <param name="sleepDurationProvider">
		/// The retry sleep duration provider function.
		/// </param>
		/// <param name="onRetry">
		/// The on-retry action.
		/// </param>
		/// <param name="onEndpointErrorTitle">
		/// The function called to retrieve a short message describing the endpoint purpose that gets appended to the end of the exception message.
		/// </param>
		/// <param name="onEndpointErrorMessage">
		/// The function called to retrieve an error message that gets appended to the end of the exception message.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The string value.
		/// </returns>
		/// <exception cref="HttpServiceException">
		/// The exception thrown when a serious or fatal HTTP error occurs.
		/// </exception>
		public Task<string> RequestPostStringAsync(
			string relativeEndpoint,
			string content,
			Func<int, TimeSpan> sleepDurationProvider,
			Action<Exception, TimeSpan, Context> onRetry,
			Func<HttpStatusCode, string> onEndpointErrorTitle,
			Func<HttpStatusCode, string> onEndpointErrorMessage,
			CancellationToken token)
		{
			return this.RequestAsync(
				           HttpRequestPost,
				           relativeEndpoint,
				           content,
				           sleepDurationProvider,
				           onRetry,
				           onEndpointErrorTitle,
				           onEndpointErrorMessage,
				           token);
		}

		/// <summary>
		/// Try to get the HTTP status code from the supplied web exception. This method will never throw an exception.
		/// </summary>
		/// <param name="exception">
		/// The web exception to inspect.
		/// </param>
		/// <returns>
		/// The <see cref="HttpStatusCode"/> value.
		/// </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "This is done by design for this type of method.")]
		internal static HttpStatusCode TryGetHttpStatusCode(WebException exception)
		{
			try
			{
				HttpStatusCode? status = (exception.Response as HttpWebResponse)?.StatusCode
				                         ?? HttpStatusCode.InternalServerError;
				return status.Value;
			}
			catch
			{
				return HttpStatusCode.InternalServerError;
			}
		}

		/// <summary>
		/// Retrieves the response by sending an HTTP POST or GET and return the deserialized object.
		/// </summary>
		/// <param name="method">
		/// The HTTP method.
		/// </param>
		/// <param name="endpoint">
		/// The relative REST endpoint.
		/// </param>
		/// <param name="content">
		/// The content to post.
		/// </param>
		/// <param name="sleepDurationProvider">
		/// The retry sleep duration provider function.
		/// </param>
		/// <param name="onRetry">
		/// The on-retry action.
		/// </param>
		/// <param name="onEndpointErrorTitle">
		/// The function called to retrieve a short message describing the endpoint purpose that gets appended to the end of the exception message.
		/// </param>
		/// <param name="onEndpointErrorMessage">
		/// The function called to retrieve an error message that gets appended to the end of the exception message.
		/// </param>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The deserialized object.
		/// </returns>
		private async Task<string> RequestAsync(
			int method,
			string endpoint,
			string content,
			Func<int, TimeSpan> sleepDurationProvider,
			Action<Exception, TimeSpan, Context> onRetry,
			Func<HttpStatusCode, string> onEndpointErrorTitle,
			Func<HttpStatusCode, string> onEndpointErrorMessage,
			CancellationToken token)
		{
			return await Policy.Handle<HttpServiceException>(e => !e.Fatal)
					   .WaitAndRetryAsync(this.MaxRetryAttempts, sleepDurationProvider, onRetry).ExecuteAsync(
						   async cancellationToken =>
							   {
								   // Enable TLS 1.2
								   ServicePointManager.SecurityProtocol =
									   SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11
																  | SecurityProtocolType.Tls
																  | SecurityProtocolType.Ssl3;
								   using (var client = new HttpClient())
								   {
									   client.BaseAddress = this.instance.Host;
									   client.DefaultRequestHeaders.Accept.Add(
										   new MediaTypeWithQualityHeaderValue("application/json"));
									   client.DefaultRequestHeaders.Add("X-CSRF-Header", string.Empty);
									   var authorizationHeader = this.GetAuthorizationHeader();

									   // All HTTP API's require an authorization header.
									   // Note: integrated security is not supported.
									   if (string.IsNullOrEmpty(authorizationHeader))
									   {
										   throw this.CreateCredentialNotSupportedException(endpoint, onEndpointErrorTitle);
									   }

									   client.DefaultRequestHeaders.Add("Authorization", authorizationHeader);

									   // Avoid unnecessary exceptions.
									   if (this.TimeoutSeconds > 0)
									   {
										   client.Timeout = TimeSpan.FromSeconds(this.TimeoutSeconds);
									   }

									   using (var stringContent = new StringContent(
										   !string.IsNullOrEmpty(content) ? content : string.Empty,
										   Encoding.UTF8,
										   "application/json"))
									   {
										   var endpointUri = new Uri(endpoint, UriKind.Relative);
										   string methodName = null;

										   try
										   {
											   HttpResponseMessage response;

											   switch (method)
											   {
												   case HttpRequestGet:
													   methodName = "GET";
													   this.logger.LogDebug(
														   "Preparing to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint...",
														   endpointUri,
														   methodName);
													   response = await client.GetAsync(endpointUri, token)
																	  .ConfigureAwait(false);
													   break;

												   case HttpRequestPost:
													   methodName = "POST";
													   this.logger.LogDebug(
														   "Preparing to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint...",
														   endpointUri,
														   methodName);
													   response = await client.PostAsync(
																	  endpointUri,
																	  stringContent,
																	  token).ConfigureAwait(false);
													   break;

												   case HttpRequestDelete:
													   methodName = "DELETE";
													   this.logger.LogDebug(
														   "Preparing to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint...",
														   endpointUri,
														   methodName);
													   response = await client.DeleteAsync(endpointUri, token)
																	  .ConfigureAwait(false);
													   break;

												   default:
													   throw new NotSupportedException(
														   $"The HTTP method '{method}; is not supported.");
											   }

											   var responseJson = await this.ReadResponseAsync(
																	  endpointUri,
																	  methodName,
																	  response,
																	  onEndpointErrorTitle,
																	  onEndpointErrorMessage).ConfigureAwait(false);
											   return responseJson;
										   }
										   catch (TaskCanceledException e)
										   {
											   // The HttpClient can throw this exception when a HTTP timeout is reached. Check the token to verify.
											   if (token.IsCancellationRequested)
											   {
												   throw;
											   }

											   // REL-260989: Ensure the retry is executed when a HTTP timeout is reached.
											   throw this.CreateTimeoutHttpServiceException(
												   endpoint,
												   methodName,
												   e,
												   onEndpointErrorTitle,
												   onEndpointErrorMessage);
										   }
										   catch (HttpRequestException e)
										   {
											   // Log the best possible message given the different exception types.
											   var webException = e.InnerException as WebException;
											   if (webException != null)
											   {
												   // Force a fatal exception to avoid retry attempts that can never succeed.
												   if (ExceptionHelper.IsWebExceptionStatusCodeFatalError(webException.Status))
												   {
													   const bool FatalWebException = true;
													   throw this.CreateHttpServiceException(
														   endpoint,
														   methodName,
														   string.Empty,
														   webException,
														   onEndpointErrorTitle,
														   onEndpointErrorMessage,
														   FatalWebException);
												   }

												   var response = webException.Response as HttpWebResponse;
												   if (response != null)
												   {
													   if (ExceptionHelper.IsHttpStatusCodeFatalError(response.StatusCode))
													   {
														   const bool FatalHttpStatusException = true;
														   throw this.CreateHttpServiceException(
															   endpoint,
															   methodName,
															   response.StatusCode,
															   string.Empty,
															   webException,
															   onEndpointErrorTitle,
															   onEndpointErrorMessage,
															   FatalHttpStatusException);
													   }
												   }

												   const bool FatalException = false;
												   throw this.CreateHttpServiceException(
													   endpoint,
													   methodName,
													   string.Empty,
													   webException,
													   onEndpointErrorTitle,
													   onEndpointErrorMessage,
													   FatalException);
											   }

											   const bool Fatal = false;
											   throw this.CreateHttpServiceException(
												   endpoint,
												   methodName,
												   NoHttpStatusCode,
												   string.Empty,
												   e,
												   onEndpointErrorTitle,
												   onEndpointErrorMessage,
												   Fatal);
										   }
									   }
								   }
							   },
						   token).ConfigureAwait(false);
		}

		/// <summary>
		/// Asynchronously reads the response.
		/// </summary>
		/// <param name="endpoint">
		/// The relative REST endpoint.
		/// </param>
		/// <param name="methodName">
		/// The HTTP method name.
		/// </param>
		/// <param name="response">
		/// The HTTP response.
		/// </param>
		/// <param name="onEndpointErrorTitle">
		/// The function called to retrieve a short message describing the endpoint purpose that gets appended to the end of the exception message.
		/// </param>
		/// <param name="onEndpointErrorMessage">
		/// The function called to retrieve an error message that gets appended to the end of the exception message.
		/// </param>
		/// <returns>
		/// The JSON value.
		/// </returns>
		private async Task<string> ReadResponseAsync(
			Uri endpoint,
			string methodName,
			HttpResponseMessage response,
			Func<HttpStatusCode, string> onEndpointErrorTitle,
			Func<HttpStatusCode, string> onEndpointErrorMessage)
		{
			var json = string.Empty;

			try
			{
				json =
					await response.Content.ReadAsStringAsync()
						.ConfigureAwait(false);
				response.EnsureSuccessStatusCode();
				this.logger.LogDebug(
					"Successfully called the HTTP '{Endpoint}' ({HttpMethod}) endpoint.",
					endpoint,
					methodName);
				return json;
			}
			catch (OperationCanceledException e)
			{
				this.logger.LogInformation(
					e,
					"The user cancelled the HTTP '{Endpoint}' ({HttpMethod}) endpoint operation.",
					endpoint,
					methodName);
				throw;
			}
			catch (Exception e)
			{
				var fatal = ExceptionHelper.IsHttpStatusCodeFatalError(response.StatusCode);
				throw this.CreateHttpServiceException(
					endpoint.ToString(),
					methodName,
					response.StatusCode,
					json,
					e,
					onEndpointErrorTitle,
					onEndpointErrorMessage,
					fatal);
			}
		}

		/// <summary>
		/// Creates a transfer exception with extended error information.
		/// </summary>
		/// <param name="endpoint">
		/// The HTTP endpoint.
		/// </param>
		/// <param name="methodName">
		/// The method name.
		/// </param>
		/// <param name="statusCode">
		/// The HTTP status code.
		/// </param>
		/// <param name="json">
		/// The JSON response.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="onEndpointErrorTitle">
		/// The function called to retrieve a short title describing the endpoint purpose that gets appended to the end of the exception message.
		/// </param>
		/// <param name="onEndpointErrorMessage">
		/// The function called to retrieve an error message that gets appended to the end of the exception message.
		/// </param>
		/// <param name="fatal">
		/// Specify whether the error is considered fatal.
		/// </param>
		/// <returns>
		/// The <see cref="HttpServiceException"/> instance.
		/// </returns>
		private HttpServiceException CreateHttpServiceException(
			string endpoint,
			string methodName,
			HttpStatusCode statusCode,
			string json,
			Exception exception,
			Func<HttpStatusCode, string> onEndpointErrorTitle,
			Func<HttpStatusCode, string> onEndpointErrorMessage,
			bool fatal)
		{
			this.logger.LogError(
				exception,
				fatal ? "Fatal attempt to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint operation. HTTP StatusCode={StatusCode}, Response={JsonResponse}" : "Failed to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint operation. HTTP StatusCode={StatusCode}, Response={JsonResponse}",
				endpoint,
				methodName,
				statusCode,
				json);
			var endpointTitle = onEndpointErrorTitle(statusCode);
			if (string.IsNullOrEmpty(endpointTitle))
			{
				endpointTitle = Strings.NoEndpointProvided;
			}

			var errorMessage = onEndpointErrorMessage(statusCode);
			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessage = Strings.NoMessageProvided;
			}

			var detailedErrorMessage = ExceptionHelper.GetDetailedFatalMessage(statusCode);
			if (string.IsNullOrEmpty(detailedErrorMessage))
			{
				detailedErrorMessage = exception.Message;
			}

			if (string.IsNullOrEmpty(detailedErrorMessage))
			{
				detailedErrorMessage = Strings.NoMessageProvided;
			}

			var message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.HttpExceptionMessage,
				endpointTitle,
				methodName,
				(int)statusCode,
				errorMessage,
				detailedErrorMessage);
			if (statusCode == 0)
			{
				message = string.Format(
					CultureInfo.CurrentCulture,
					Strings.HttpNoStatusExceptionMessage,
					endpointTitle,
					methodName,
					errorMessage,
					detailedErrorMessage);
			}

			message = message.TrimEnd();
			return new HttpServiceException(message, exception, statusCode, fatal);
		}

		/// <summary>
		/// Creates a transfer exception with extended error information.
		/// </summary>
		/// <param name="endpoint">
		/// The HTTP endpoint.
		/// </param>
		/// <param name="methodName">
		/// The method name.
		/// </param>
		/// <param name="json">
		/// The JSON response.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="onEndpointErrorTitle">
		/// The function called to retrieve a short title describing the endpoint purpose that gets appended to the end of the exception message.
		/// </param>
		/// <param name="onEndpointErrorMessage">
		/// The function called to retrieve an error message that gets appended to the end of the exception message.
		/// </param>
		/// <param name="fatal">
		/// Specify whether the error is considered fatal.
		/// </param>
		/// <returns>
		/// The <see cref="HttpServiceException"/> instance.
		/// </returns>
		private HttpServiceException CreateHttpServiceException(
			string endpoint,
			string methodName,
			string json,
			WebException exception,
			Func<HttpStatusCode, string> onEndpointErrorTitle,
			Func<HttpStatusCode, string> onEndpointErrorMessage,
			bool fatal)
		{
			this.logger.LogError(
				exception,
				fatal ? "Fatal attempt to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint operation. Web Response Status={WebResponseStatus}, Response={JsonResponse}" : "Failed to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint operation. Web Response Status={WebResponseStatus}, Response={JsonResponse}",
				endpoint,
				methodName,
				exception.Status,
				json);
			var endpointTitle = onEndpointErrorTitle(NoHttpStatusCode);
			if (string.IsNullOrEmpty(endpointTitle))
			{
				endpointTitle = Strings.NoEndpointProvided;
			}

			var errorMessage = onEndpointErrorMessage(NoHttpStatusCode);
			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessage = Strings.NoMessageProvided;
			}

			var detailedErrorMessage = ExceptionHelper.GetDetailedFatalMessage(exception.Status);
			if (string.IsNullOrEmpty(detailedErrorMessage))
			{
				detailedErrorMessage = exception.Message;
			}

			if (string.IsNullOrEmpty(detailedErrorMessage))
			{
				detailedErrorMessage = Strings.NoMessageProvided;
			}

			var message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.WebExceptionMessage,
				endpointTitle,
				methodName,
				(int)exception.Status,
				errorMessage,
				detailedErrorMessage);
			message = message.TrimEnd();
			HttpStatusCode statusCode = TryGetHttpStatusCode(exception);
			return new HttpServiceException(message, exception, statusCode, fatal);
		}

		/// <summary>
		/// Creates a transfer timeout exception with extended error information.
		/// </summary>
		/// <param name="endpoint">
		/// The HTTP endpoint.
		/// </param>
		/// <param name="methodName">
		/// The method name.
		/// </param>
		/// <param name="exception">
		/// The exception.
		/// </param>
		/// <param name="onEndpointErrorTitle">
		/// The function called to retrieve a short title describing the endpoint purpose that gets appended to the end of the exception message.
		/// </param>
		/// <param name="onEndpointErrorMessage">
		/// The function called to retrieve an error message that gets appended to the end of the exception message.
		/// </param>
		/// <returns>
		/// The <see cref="HttpServiceException"/> instance.
		/// </returns>
		private HttpServiceException CreateTimeoutHttpServiceException(
			string endpoint,
			string methodName,
			Exception exception,
			Func<HttpStatusCode, string> onEndpointErrorTitle,
			Func<HttpStatusCode, string> onEndpointErrorMessage)
		{
			this.logger.LogError(
				exception,
				"Failed to call the HTTP '{Endpoint}' ({HttpMethod}) endpoint operation because it exceeded the {HttpTimeoutSeconds} second timeout.",
				endpoint,
				methodName,
				this.TimeoutSeconds);
			var endpointTitle = onEndpointErrorTitle(HttpStatusCode.RequestTimeout);
			if (string.IsNullOrEmpty(endpointTitle))
			{
				endpointTitle = Strings.NoEndpointProvided;
			}

			var errorMessage = onEndpointErrorMessage(HttpStatusCode.RequestTimeout);
			if (string.IsNullOrEmpty(errorMessage))
			{
				errorMessage = Strings.NoMessageProvided;
			}

			var detailedErrorMessage = exception.Message;
			if (string.IsNullOrEmpty(detailedErrorMessage))
			{
				detailedErrorMessage = Strings.NoMessageProvided;
			}

			var message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.HttpTimeoutExceptionMessage,
				endpointTitle,
				methodName,
				this.TimeoutSeconds,
				errorMessage,
				detailedErrorMessage);
			message = message.TrimEnd();
			const bool Fatal = false;
			return new HttpServiceException(message, exception, HttpStatusCode.RequestTimeout, Fatal);
		}

		/// <summary>
		/// Creates a credential not supported exception.
		/// </summary>
		/// <param name="endpoint">
		/// The HTTP endpoint.
		/// </param>
		/// <param name="onEndpointErrorTitle">
		/// The function called to retrieve a short title describing the endpoint purpose that gets appended to the end of the exception message.
		/// </param>
		/// <returns>
		/// The <see cref="HttpServiceException"/> instance.
		/// </returns>
		private HttpServiceException CreateCredentialNotSupportedException(string endpoint, Func<HttpStatusCode, string> onEndpointErrorTitle)
		{
			this.logger.LogError(
				"Failed to call the HTTP '{Endpoint}' endpoint operation because the supplied Transfer API credential object '{CredentialType}' is not supported.",
				endpoint,
				this.instance.Credentials.GetType());
			var endpointTitle = onEndpointErrorTitle(HttpStatusCode.Unauthorized);
			if (string.IsNullOrEmpty(endpointTitle))
			{
				endpointTitle = Strings.NoEndpointProvided;
			}

			var message = string.Format(
				CultureInfo.CurrentCulture,
				Strings.HttpCredentialNotSupportedExceptionMessage,
				endpointTitle,
				this.instance.Credentials.GetType().ToString());
			message = message.TrimEnd();
			const bool Fatal = true;
			return new HttpServiceException(message, HttpStatusCode.Unauthorized, Fatal);
		}

		/// <summary>
		/// Gets an authorization header from the <see cref="ICredentials"/> object.
		/// </summary>
		/// <returns>
		/// The header string.
		/// </returns>
		private string GetAuthorizationHeader()
		{
			// TODO: This should be broken out into a separate class.
			NetworkCredential credential = this.instance.Credentials as NetworkCredential;
			if (credential == null)
			{
				return null;
			}

			if (credential.UserName == "XxX_BearerTokenCredentials_XxX")
			{
				return string.Format(CultureInfo.InvariantCulture, "Bearer {0}", credential.Password);
			}

			string pwd = string.Format(
				CultureInfo.InvariantCulture,
				"{0}:{1}",
				credential.UserName,
				credential.Password);
			return string.Format(
				CultureInfo.InvariantCulture,
				"Basic {0}",
				Convert.ToBase64String(Encoding.ASCII.GetBytes(pwd)));
		}
	}
}