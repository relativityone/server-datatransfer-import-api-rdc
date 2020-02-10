// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogExtensions.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents static methods to create Relativity log instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Relativity.DataExchange.Helpers;
	using Relativity.Logging;

	/// <summary>
	/// Defines extension methods for <see cref="Relativity.Logging.ILog"/>.
	/// </summary>
	internal static class LogExtensions
	{
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
				logger.LogInformation(messageTemplate, dictionary);
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
		/// <param name="jwtEncodedToken">encoded relativity auth token.</param>
		public static void LogInformationForAuthToken(this ILog logger, string messageTemplate, string jwtEncodedToken)
		{
			if (JwtTokenHelper.TryParse(jwtEncodedToken, out var jwtAuthToken))
			{
				logger.LogInformation(messageTemplate, jwtAuthToken);
			}
			else
			{
				logger.LogInformation(messageTemplate, $"Can't parse token: {jwtEncodedToken}");
			}
		}
	}
}