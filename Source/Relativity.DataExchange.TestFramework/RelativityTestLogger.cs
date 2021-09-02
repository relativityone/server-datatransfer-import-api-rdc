// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelativityTestLogger.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents an object to write debug, information, warning, and error logs to the console and the Relativity logger.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Collections;
	using System.Linq;
	using System.Text;

	using Relativity.Logging;

	using Serilog.Events;
	using Serilog.Parsing;

	/// <summary>
	/// Represents an object to write debug, information, warning, and error logs to the console and the Relativity logger.
	/// </summary>
	/// <remarks>
	/// Serilog is used to decode the message templates and properties into a complete message string that
	/// can be easily viewed by developers or Jenkins pipelines.
	/// </remarks>
	internal sealed class RelativityTestLogger : Relativity.Logging.ILog
	{
		/// <summary>
		/// The logger instance.
		/// </summary>
		private readonly Relativity.Logging.ILog logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityTestLogger"/> class.
		/// </summary>
		/// <param name="logger">
		/// The logger.
		/// </param>
		public RelativityTestLogger(Relativity.Logging.ILog logger)
		{
			if (logger == null)
			{
				throw new ArgumentNullException(nameof(logger));
			}

			this.logger = logger;
		}

		/// <inheritdoc />
		public bool IsEnabled => this.logger.IsEnabled;

		/// <inheritdoc />
		public string Application => this.logger.Application;

		/// <inheritdoc />
		public string SubSystem => this.logger.SubSystem;

		/// <inheritdoc />
		public string System => this.logger.System;

		/// <inheritdoc />
		public Relativity.Logging.ILog ForContext(Type source)
		{
			Relativity.Logging.ILog newRelLogger = this.logger.ForContext(source);
			return new RelativityTestLogger(newRelLogger);
		}

		/// <inheritdoc />
		public Relativity.Logging.ILog ForContext(string propertyName, object value, bool destructureObjects)
		{
			Relativity.Logging.ILog newRelLogger = this.logger.ForContext(propertyName, value, destructureObjects);
			return new RelativityTestLogger(newRelLogger);
		}

		/// <inheritdoc />
		public Relativity.Logging.ILog ForContext<T>()
		{
			Relativity.Logging.ILog newRelLogger = this.logger.ForContext<T>();
			return new RelativityTestLogger(newRelLogger);
		}

		/// <inheritdoc />
		public IDisposable LogContextPushProperty(string propertyName, object obj)
		{
			return this.logger.LogContextPushProperty(propertyName, obj);
		}

		/// <inheritdoc />
		public void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Debug, null, messageTemplate, propertyValues);
			this.logger.LogDebug(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Debug, exception, messageTemplate, propertyValues);
			this.logger.LogDebug(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogError(string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Error, null, messageTemplate, propertyValues);
			this.logger.LogError(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Error, exception, messageTemplate, propertyValues);
			this.logger.LogError(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Fatal, null, messageTemplate, propertyValues);
			this.logger.LogFatal(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Fatal, exception, messageTemplate, propertyValues);
			this.logger.LogFatal(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Information, null, messageTemplate, propertyValues);
			this.logger.LogInformation(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Information, exception, messageTemplate, propertyValues);
			this.logger.LogInformation(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Verbose, null, messageTemplate, propertyValues);
			this.logger.LogVerbose(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Verbose, exception, messageTemplate, propertyValues);
			this.logger.LogVerbose(exception, messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Warning, null, messageTemplate, propertyValues);
			this.logger.LogWarning(messageTemplate, propertyValues);
		}

		/// <inheritdoc />
		public void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ConsoleWriteLine(LoggingLevel.Warning, exception, messageTemplate, propertyValues);
			this.logger.LogWarning(exception, messageTemplate, propertyValues);
		}

		/// <summary>
		/// Writes the log entry to the system console.
		/// </summary>
		/// <param name="level">
		/// The logging level.
		/// </param>
		/// <param name="exception">
		/// The logged exception.
		/// </param>
		/// <param name="messageTemplate">
		/// The message template.
		/// </param>
		/// <param name="propertyValues">
		/// The property values.
		/// </param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Logging should never fail.")]
		private static void ConsoleWriteLine(LoggingLevel level, Exception exception, string messageTemplate, params object[] propertyValues)
		{
			var parser = new MessageTemplateParser();
			var template = parser.Parse(messageTemplate);
			var properties = template.Tokens.OfType<PropertyToken>().Distinct().Zip(propertyValues, Tuple.Create)
				.ToDictionary(p => p.Item1.PropertyName, p => new ScalarValue(GetItemValue(p.Item2)) as LogEventPropertyValue);
			var message = template.Render(properties);

			try
			{
				if (exception != null)
				{
					message += $" - Exception: {exception}";
				}

				Console.WriteLine($"[{level}] - {message}");
			}
			catch (Exception e)
			{
				Console.WriteLine("Failed to format the message template to a standard Console message. Error: " + e);
			}
		}

		private static object GetItemValue(object propertyValue)
		{
			if (propertyValue is IDictionary dictionary)
			{
				var stringBuilder = new StringBuilder();
				foreach (DictionaryEntry entry in dictionary)
				{
					stringBuilder.AppendFormat("{0}={1}, ", entry.Key, entry.Value);
				}

				return stringBuilder.ToString();
			}

			return propertyValue;
		}
	}
}