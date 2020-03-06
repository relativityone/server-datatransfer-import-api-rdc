// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashingLoggerDecorator.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Hash sensitive data in Relativity.Logging.ILog instance. Decorator design pattern is used.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.Logger
{
	using System;
	using System.Linq;
	using System.Security.Cryptography;
	using System.Text;

	using Relativity.DataExchange.Helpers;
	using Relativity.Logging;

	/// <summary>
	/// Hash sensitive data in Relativity.Logging.ILog instance. Decorator design pattern is used.
	/// </summary>
	internal class HashingLoggerDecorator : LoggerDecoratorBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HashingLoggerDecorator"/> class.
		/// </summary>
		/// <param name="logger"> Instance of Relativity.Logging.ILog to decorate.</param>
		public HashingLoggerDecorator(ILog logger)
			: base(logger)
		{
		}

		/// <inheritdoc/>
		public override void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			base.LogVerbose(messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			base.LogVerbose(exception, messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			base.LogDebug(messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			base.LogDebug(exception, messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			base.LogInformation(messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			base.LogInformation(exception, messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			base.LogWarning(messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			base.LogWarning(exception, messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogError(string messageTemplate, params object[] propertyValues)
		{
			base.LogError(messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			base.LogError(exception, messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			base.LogFatal(messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			base.LogFatal(exception, messageTemplate, HashPropertyValues(propertyValues));
		}

		/// <inheritdoc/>
		public override ILog ForContext<T>()
		{
			return new HashingLoggerDecorator(base.ForContext<T>());
		}

		/// <inheritdoc/>
		public override ILog ForContext(Type forContext)
		{
			return new HashingLoggerDecorator(base.ForContext(forContext));
		}

		/// <inheritdoc/>
		public override ILog ForContext(string propertyName, object value, bool destructureObjects)
		{
			var newValue = this.HashSensitivePropertyValue(value);
			return new HashingLoggerDecorator(base.ForContext(propertyName, newValue, destructureObjects));
		}

		private object[] HashPropertyValues(params object[] propertyValues)
		{
			if (propertyValues == null || !propertyValues.Any())
			{
				return null;
			}

			object[] newPropertyValues = propertyValues
				.Select(this.HashSensitivePropertyValue)
				.ToArray();

			if (newPropertyValues.Last() is int[] sensitiveKeys)
			{
				ValidateSensitiveKeys(propertyValues.Length - 1, sensitiveKeys);

				newPropertyValues = newPropertyValues
					.Take(propertyValues.Length - 1)
					.Select((propertyValue, index) => sensitiveKeys.Contains(index) && propertyValue != null ?
						                          HashingHelper.CalculateSHA256Hash(propertyValue.ToString()) :
						                          propertyValue)
					.ToArray();
			}

			return newPropertyValues;
		}

		private object HashSensitivePropertyValue(object propertyValue)
		{
			return propertyValue is SecurePropertyValueBase securePropertyValue
				       ? HashingHelper.CalculateSHA256Hash(securePropertyValue.ToString())
				       : propertyValue;
		}

		private void ValidateSensitiveKeys(int propertyValuesCount, int[] sensitiveKeys)
		{
			if (sensitiveKeys.Any() && (sensitiveKeys.Max() >= propertyValuesCount || sensitiveKeys.Min() < 0))
			{
				this.LogError("Sensitive data index is out of range");
			}
		}
	}
}