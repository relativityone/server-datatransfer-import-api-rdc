// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiServiceBase.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a class object to provide a Transfer API bridge to existing WinEDDS code for downloading.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
	using System;
	using System.Net;

	using Relativity.Logging;

	/// <summary>
	/// Represents an abstract class service object for WebAPI-based wrappers.
	/// </summary>
	internal abstract class WebApiServiceBase
	{
		/// <summary>
		/// The logger
		/// </summary>
		private readonly Relativity.Logging.ILog logger;

		/// <summary>
		/// The initialized backing.
		/// </summary>
		private bool initialized;

		/// <summary>
		/// Initializes a new instance of the <see cref="T:kCura.WinEDDS.TApi.WebApiService2"/> class.
		/// </summary>
		/// <param name="parameters">
		/// The TAPI bridge parameters.
		/// </param>
		protected WebApiServiceBase(TapiBridgeParameters parameters)
		{
			if (parameters == null)
			{
				throw new ArgumentNullException(nameof(parameters));
			}

			if (string.IsNullOrEmpty(parameters.WebServiceUrl))
			{
				throw new ArgumentException("The WebServiceUrl must be non-null or empty.", nameof(parameters));
			}

			if (parameters.WorkspaceId < 1)
			{
				throw new ArgumentException("The WorkspaceId must be non-zero.", nameof(parameters));
			}

			if (parameters.Credentials == null)
			{
				throw new ArgumentException("The Credentials must be non-null.", nameof(parameters));
			}

			if (parameters.WebCookieContainer == null)
			{
				throw new ArgumentException("The WebCookieContainer must be non-null.", nameof(parameters));
			}

			this.Parameters = parameters;

			// This is assumed throughout WinEDDS.
			this.logger = Relativity.Logging.Log.Logger ?? new NullLogger();
		}

		/// <summary>
		/// The WebAPI credential.
		/// </summary>
		/// <returns>
		/// The <see cref="NetworkCredential"/> instance.
		/// </returns>
		protected NetworkCredential Credential
		{
			get
			{
				var value = this.Parameters.Credentials;
				return value;
			}
		}

		/// <summary>
		/// The cookie container.
		/// </summary>
		/// <returns>
		/// The <see cref="CookieContainer"/> instance.
		/// </returns>
		protected CookieContainer CookieContainer
		{
			get
			{
				var value = this.Parameters.WebCookieContainer;
				return value;
			}
		}

		/// <summary>
		/// Gets the cache expiration in minutes.
		/// </summary>
		/// <value>
		/// The total number of minutes.
		/// </value>
		protected int ExpirationMinutes
		{
			get
			{
				// TODO: This is NOT currently exposed by config settings.
				const int Value = 60;
				return Value;
			}
		}

		/// <summary>
		/// Gets the maximum number of retry attempts.
		/// </summary>
		/// <value>
		/// The total number of retry attempts.
		/// </value>
		protected int MaxRetryAttempts
		{
			get
			{
				// TODO: This is NOT currently exposed by config settings.
				const int Value = 20;
				return Value;
			}
		}

		/// <summary>
		/// Gets the HTTP timeout in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		protected int TimeoutSeconds
		{
			get
			{
				var value = this.Parameters.TimeoutSeconds;
				if (value <= 0)
				{
					value = 600;
				}

				return value;
			}
		}

		/// <summary>
		/// Gets the wait time, in seconds, between retry attempts.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		protected int WaitTimeBetweenRetryAttempts
		{
			get
			{
				var value = this.Parameters.WaitTimeBetweenRetryAttempts;
				if (value <= 0)
				{
					value = 15;
				}

				return value;
			}
		}

		/// <summary>
		/// Gets the HTTP timeout in seconds.
		/// </summary>
		/// <value>
		/// The total number of seconds.
		/// </value>
		protected string WebServiceUrl
		{
			get
			{
				var value = this.Parameters.WebServiceUrl;
				return value;
			}
		}

		/// <summary>
		/// The TAPI bridge parameters.
		/// </summary>
		/// <value>
		/// The <see cref="TapiBridgeParameters"/> instance.
		/// </value>
		private TapiBridgeParameters Parameters
		{
			get;
		}

		/// <summary>
		/// Safely combines the specified URL and the value into a new <see cref="Uri"/> instance.
		/// </summary>
		/// <param name="url">
		/// The URL.
		/// </param>
		/// <param name="value">
		/// The value.
		/// </param>
		/// <returns>
		/// The <see cref="Uri"/> instance.
		/// </returns>
		public static string Combine(string url, string value)
		{
			if (url == null)
			{
				throw new ArgumentNullException(nameof(url));
			}

			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException(nameof(value));
			}

			// The base URL must include the slash to combine correctly.
			var urlString = url.TrimEnd('/') + "/" + value;
			return urlString;
		}

		/// <summary>
		/// Initializes this instance.
		/// </summary>
		protected void Initialize()
		{
			if (this.initialized)
			{
				return;
			}

			// Enable TLS 1.2
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11
			                                                                  | SecurityProtocolType.Tls
			                                                                  | SecurityProtocolType.Ssl3;
			this.initialized = true;
		}

		protected void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogInformation(messageTemplate, propertyValues);
		}

		protected void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogError(exception, messageTemplate, propertyValues);
		}
	}
}