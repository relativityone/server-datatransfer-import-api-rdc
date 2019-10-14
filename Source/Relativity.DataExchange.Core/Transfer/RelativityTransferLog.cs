// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityTransferLog.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a thread-safe class object to write debug, information, warning, and error logs using Relativity Logging.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;

	using Relativity.Logging;
	using Relativity.Transfer;

	/// <summary>
	/// Represents a thread-safe class object to write debug, information, warning, and error logs using Relativity Logging. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// This is an alternative implementation of Relativity Logging <see cref="ITransferLog"/> and can be used in client-side scenarios.
	/// </remarks>
	internal sealed class RelativityTransferLog : ITransferLog
	{
		/// <summary>
		/// The Relativity log backing.
		/// </summary>
		private readonly ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityTransferLog"/> class.
		/// </summary>
		/// <param name="logger">
		/// The Relativity logger instance.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="logger"/> is <see langword="null"/>.
		/// </exception>
		public RelativityTransferLog(ILog logger)
		{
			this.logger = logger.ThrowIfNull(nameof(logger));
			this.IsEnabled = logger.IsEnabled;
		}

		/// <inheritdoc />
		public bool IsEnabled
		{
			get;
			set;
		}

		/// <inheritdoc />
		public void Dispose()
		{
		}

		/// <inheritdoc />
		public void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogInformation(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogInformation(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogDebug(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogDebug(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogWarning(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogWarning(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogError(string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogError(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogError(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogVerbose(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogVerbose(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogFatal(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			if (!this.IsEnabled)
			{
				return;
			}

			this.logger.LogFatal(exception, messageTemplate, propertyValues);
		}
	}
}