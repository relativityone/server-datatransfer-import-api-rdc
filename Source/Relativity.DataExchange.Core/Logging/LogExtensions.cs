// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents static methods to create Relativity log instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Logging
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Net;
	using System.Reflection;

	using Relativity.DataExchange.Helpers;
	using Relativity.Logging;

	using Constants = Relativity.DataExchange.Constants;

	/// <summary>
	/// Defines extension methods for <see cref="Relativity.Logging.ILog"/>.
	/// </summary>
	internal static class LogExtensions
	{
		private const string DataExchangeLogPrefix = "DataExchange";
		private const string ImportLogPrefix = "Import";

		/// <summary>
		/// Write an informational log message and transforms <paramref name="value"/> into a dictionary that's supplied to the logging framework.
		/// </summary>
		/// <param name="logger">
		/// The logger.
		/// </param>
		/// <param name="messageTemplate">
		/// The message template. Ensure the destructured operator is contained within the message.
		/// </param>
		/// <param name="value">
		/// The object to log.
		/// </param>
		[Obsolete("Please use ILog instance directly. In this method, it is impossible to hash sensitive data")]
		public static void LogObjectAsDictionary(
			this Relativity.Logging.ILog logger,
			string messageTemplate,
			object value)
		{
			LogObjectAsDictionary(logger, messageTemplate, value, null);
		}

		/// <summary>
		/// Write an informational log message and transforms the optionally filtered <paramref name="value"/> into a dictionary that's supplied to the logging framework.
		/// </summary>
		/// <param name="logger">
		/// The logger.
		/// </param>
		/// <param name="messageTemplate">
		/// The message template. Ensure the destructured operator is contained within the message.
		/// </param>
		/// <param name="value">
		/// The object to log.
		/// </param>
		/// <param name="filter">
		/// The optional filter used to exclude specific properties. This can be <see langword="null" /> if no filtering is required.
		/// </param>
		[Obsolete("Please use ILog instance directly. In this method, it is impossible to hash sensitive data")]
		public static void LogObjectAsDictionary(
			this Relativity.Logging.ILog logger,
			string messageTemplate,
			object value,
			Func<System.Reflection.PropertyInfo, bool> filter)
		{
			logger.ThrowIfNull(nameof(logger));
			if (value == null)
			{
				return;
			}

			try
			{
				// Reflection is used instead of serialization (e.g. JSON) due to potential structure size.
				BindingFlags bindingFlags =
					System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public;
				List<System.Reflection.PropertyInfo> properties = value.GetType().GetProperties(bindingFlags)
					.Where(info => filter == null || filter(info)).ToList();
				Dictionary<string, string> dictionary = properties.ToDictionary(
					info => info.Name,
					info => Convert.ToString(info.GetValue(value, null)));
				logger.LogInformation(messageTemplate, dictionary); // impossible to hash
			}
			catch (Exception e)
			{
				if (ExceptionHelper.IsFatalException(e))
				{
					throw;
				}

				logger.LogWarning(e, "Failed to log {SettingType} as a dictionary.", value.GetType());
			}
		}

		/// <summary>
		/// It logs jwt token claims on information logging level according to provided message template.
		/// </summary>
		/// <param name="logger">logger instance.</param>
		/// <param name="messageTemplate">logged message template.</param>
		/// <param name="credentials">User credentials.</param>
		public static void LogUserContextInformation(this ILog logger, string messageTemplate, ICredentials credentials)
		{
			if (credentials is NetworkCredential networkCredentials)
			{
				if (networkCredentials.UserName == Constants.OAuthWebApiBearerTokenUserName)
				{
					LogInformationForAuthToken(logger, messageTemplate, networkCredentials);
				}
				else
				{
					logger.LogInformation(messageTemplate + " with User Id: {userId}", HashingHelper.CalculateSHA256Hash(networkCredentials.UserName));
				}
			}
		}

		/// <summary>
		/// This method pushes all public properties from given object to ILog instance logging context.
		/// </summary>
		/// <param name="logger">logger instance.</param>
		/// <param name="value">logging context object.</param>
		/// <returns>instance of of the <see cref="IDisposable"/>. </returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Reliability",
			"CA2000:Dispose objects before losing scope",
			Justification = "It wraps LogContextPushProperty ILog method execution so the calling code is responsible for disposing the returned object.")]
		public static IDisposable LogImportContextPushProperties(this ILog logger, object value)
		{
			logger.ThrowIfNull(nameof(logger));
			value.ThrowIfNull(nameof(value));

			var stackOfDisposables = new StackOfDisposables();

			Type type = value.GetType();
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

			foreach (PropertyInfo property in properties)
			{
				string propertyName = property.Name;
				string propertyValue = property.GetValue(value, null)?.ToString() ?? string.Empty;

				IDisposable disposable = logger.LogContextPushProperty($"{DataExchangeLogPrefix}.{ImportLogPrefix}.{propertyName}", propertyValue);
				stackOfDisposables.Push(disposable);
			}

			return stackOfDisposables;
		}

		private static void LogInformationForAuthToken(ILog logger, string messageTemplate, NetworkCredential networkCredentials)
		{
			if (JwtTokenHelper.TryParse(networkCredentials.Password, out var jwtAuthToken))
			{
				logger.LogInformation(messageTemplate + " with {@jwtAuthToken}", jwtAuthToken);
			}
			else
			{
				logger.LogWarning("Can't parse bearer token!");
			}
		}
	}
}