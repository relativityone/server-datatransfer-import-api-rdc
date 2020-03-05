// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AggregatingLoggerDecorator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Aggregate two Relativity.Logging.ILog instances. Decorator design pattern is used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Logger
{
	using System;
	using System.Collections.Generic;
	using System.Reactive.Disposables;

	using Relativity.Logging;

	/// <summary>
	/// Aggregate two Relativity.Logging.ILog instances. Decorator design pattern is used.
	/// </summary>
	internal class AggregatingLoggerDecorator : LoggerDecoratorBase
	{
		private readonly ILog additionalLogger;

		/// <summary>
		/// Initializes a new instance of the <see cref="AggregatingLoggerDecorator"/> class.
		/// </summary>
		/// <param name="logger"> Instance of Relativity.Logging.ILog to decorate.</param>
		/// <param name="additionalLogger"> Additional instance of Relativity.Logging.ILog.</param>
		public AggregatingLoggerDecorator(ILog logger, ILog additionalLogger)
			: base(logger)
		{
			this.additionalLogger = additionalLogger;
		}

		/// <inheritdoc/>
		public override void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogVerbose(messageTemplate, propertyValues);
			base.LogVerbose(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogVerbose(exception, messageTemplate, propertyValues);
			base.LogVerbose(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogDebug(messageTemplate, propertyValues);
			base.LogDebug(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogDebug(exception, messageTemplate, propertyValues);
			base.LogDebug(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogInformation(messageTemplate, propertyValues);
			base.LogInformation(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogInformation(exception, messageTemplate, propertyValues);
			base.LogInformation(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogWarning(messageTemplate, propertyValues);
			base.LogWarning(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogWarning(exception, messageTemplate, propertyValues);
			base.LogWarning(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogError(string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogError(messageTemplate, propertyValues);
			base.LogError(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogError(exception, messageTemplate, propertyValues);
			base.LogError(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogFatal(messageTemplate, propertyValues);
			base.LogFatal(messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			this.additionalLogger?.LogFatal(exception, messageTemplate, propertyValues);
			base.LogFatal(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc/>
		public override ILog ForContext<T>()
		{
			return new AggregatingLoggerDecorator(base.ForContext<T>(), this.additionalLogger?.ForContext<T>());
		}

		/// <inheritdoc/>
		public override ILog ForContext(Type forContext)
		{
			return new AggregatingLoggerDecorator(base.ForContext(forContext), this.additionalLogger?.ForContext(forContext));
		}

		/// <inheritdoc/>
		public override ILog ForContext(string propertyName, object value, bool destructureObjects)
		{
			return new AggregatingLoggerDecorator(base.ForContext(propertyName, value, destructureObjects), this.additionalLogger?.ForContext(propertyName, value, destructureObjects));
		}

		/// <inheritdoc/>
		public override IDisposable LogContextPushProperty(string propertyName, object obj)
		{
			if (this.additionalLogger == null)
			{
				return base.LogContextPushProperty(propertyName, obj);
			}

			return new CompositeDisposable(base.LogContextPushProperty(propertyName, obj), this.additionalLogger?.LogContextPushProperty(propertyName, obj));
		}
	}
}