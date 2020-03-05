// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiServiceBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an abstract class service object for WebAPI-based wrappers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Service
{
	using System;
	using System.Net;

	using Relativity.DataExchange.Logger;
	using Relativity.Logging;

	/// <summary>
	/// Represents an abstract class service object for WebAPI-based wrappers.
	/// </summary>
	internal abstract class WebApiServiceBase
	{
		private static readonly IObjectCacheRepository DefaultObjectCacheRepository = new MemoryCacheRepository();

		/// <summary>
		/// The logger instance.
		/// </summary>
		private readonly ILog logger;

		/// <summary>
		/// The initialized backing.
		/// </summary>
		private bool initialized;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebApiServiceBase"/> class.
		/// </summary>
		/// <param name="instanceInfo">
		/// The Relativity instance information.
		/// </param>
		protected WebApiServiceBase(RelativityInstanceInfo instanceInfo)
			: this(instanceInfo, DefaultObjectCacheRepository, DataExchange.AppSettings.Instance)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WebApiServiceBase"/> class.
		/// </summary>
		/// <param name="instanceInfo">
		/// The Relativity instance information.
		/// </param>
		/// <param name="repository">
		/// The object cache repository.
		/// </param>
		/// <param name="appSettings">
		/// The application settings.
		/// </param>
		protected WebApiServiceBase(RelativityInstanceInfo instanceInfo, IObjectCacheRepository repository, IAppSettings appSettings)
		{
			if (instanceInfo == null)
			{
				throw new ArgumentNullException(nameof(instanceInfo));
			}

			if (repository == null)
			{
				throw new ArgumentNullException(nameof(repository));
			}

			if (appSettings == null)
			{
				throw new ArgumentNullException(nameof(appSettings));
			}

			if (instanceInfo.WebApiServiceUrl == null)
			{
				throw new ArgumentException("The WebApiServiceUrl must be non-null.", nameof(instanceInfo));
			}

			if (instanceInfo.Credentials == null)
			{
				throw new ArgumentException("The Credentials must be non-null.", nameof(instanceInfo));
			}

			if (instanceInfo.CookieContainer == null)
			{
				throw new ArgumentException("The CookieContainer must be non-null.", nameof(instanceInfo));
			}

			this.AppSettings = appSettings;
			this.CacheRepository = repository;
			this.InstanceInfo = instanceInfo;
			this.logger = RelativityLogger.Instance;
		}

		/// <summary>
		/// Gets the application settings.
		/// </summary>
		/// <returns>
		/// The <see cref="IAppSettings"/> instance.
		/// </returns>
		protected IAppSettings AppSettings
		{
			get;
		}

		/// <summary>
		/// Gets the object cache repository.
		/// </summary>
		/// <returns>
		/// The <see cref="IObjectCacheRepository"/> instance.
		/// </returns>
		protected IObjectCacheRepository CacheRepository
		{
			get;
		}

		/// <summary>
		/// Gets the Relativity instance information.
		/// </summary>
		/// <value>
		/// The <see cref="RelativityInstanceInfo"/> instance.
		/// </value>
		protected RelativityInstanceInfo InstanceInfo
		{
			get;
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

		/// <summary>
		/// Logs an informational log entry.
		/// </summary>
		/// <param name="messageTemplate">
		/// The message template.
		/// </param>
		/// <param name="propertyValues">
		/// The optional property values.
		/// </param>
		protected void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogInformation(messageTemplate, propertyValues);
		}

		/// <summary>
		/// Logs an error log entry.
		/// </summary>
		/// <param name="exception">
		/// The exception to log.
		/// </param>
		/// <param name="messageTemplate">
		/// The message template.
		/// </param>
		/// <param name="propertyValues">
		/// The optional property values.
		/// </param>
		protected void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogError(exception, messageTemplate, propertyValues);
		}
	}
}