// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoggerDecoratorBase.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Base class for decorating Relativity.Logging.ILog instance. Decorator design pattern is used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Logger
{
	using System;

	using Relativity.Logging;

	/// <summary>
	/// Base class for decorating Relativity.Logging.ILog instance. Decorator design pattern is used.
	/// </summary>
	internal abstract class LoggerDecoratorBase : ILog
	{
		private readonly ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="LoggerDecoratorBase"/> class.
		/// </summary>
		/// <param name="logger"> ILog instance which should be decorated.</param>
		public LoggerDecoratorBase(ILog logger)
		{
			this.logger = logger.ThrowIfNull(nameof(logger));
		}

		/// <inheritdoc/>
		public bool IsEnabled => this.logger.IsEnabled;

		/// <inheritdoc/>
		public string Application => this.logger.Application;

		/// <inheritdoc/>
		public string SubSystem => this.logger.SubSystem;

		/// <inheritdoc/>
		public string System => this.logger.System;

		/// <inheritdoc/>
		public virtual void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogVerbose(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogVerbose(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogDebug(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogDebug(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogInformation(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogInformation(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogWarning(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogWarning(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogError(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogError(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogError(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogFatal(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.logger.LogFatal(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public virtual ILog ForContext<T>()
		{
			return this.logger.ForContext<T>();
		}

		/// <inheritdoc/>
		public virtual ILog ForContext(Type forContext)
		{
			return this.logger.ForContext(forContext);
		}

		/// <inheritdoc/>
		public virtual ILog ForContext(string propertyName, object value, bool destructureObjects)
		{
			return this.logger.ForContext(propertyName, value, destructureObjects);
		}

		/// <inheritdoc/>
		public virtual IDisposable LogContextPushProperty(string propertyName, object obj)
		{
			return this.logger.LogContextPushProperty(propertyName, obj);
		}
	}
}