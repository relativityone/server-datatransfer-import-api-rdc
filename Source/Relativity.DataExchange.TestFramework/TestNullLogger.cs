// <copyright file="TestNullLogger.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.TestFramework
{
	using System;
	using System.Linq;

	using Moq;

	using NUnit.Framework;

	using Relativity.Logging;

	public class TestNullLogger : ILog
	{
#pragma warning disable SA1401 // Fields should be private
		public readonly Mock<ILog> NullLoggerMock = new Mock<ILog>();
#pragma warning restore SA1401 // Fields should be private

		public ILog NullLogger => NullLoggerMock.Object;

		public bool IsEnabled => NullLogger.IsEnabled;

		public string Application => NullLogger.Application;

		public string SubSystem => NullLogger.SubSystem;

		public string System => NullLogger.System;

		public void LogVerbose(string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogVerbose(messageTemplate, propertyValues);
		}

		public void LogVerbose(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogVerbose(exception, messageTemplate, propertyValues);
		}

		public void LogDebug(string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogDebug(messageTemplate, propertyValues);
		}

		public void LogDebug(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogDebug(exception, messageTemplate, propertyValues);
		}

		public void LogInformation(string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogInformation(messageTemplate, propertyValues);
		}

		public void LogInformation(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogInformation(exception, messageTemplate, propertyValues);
		}

		public void LogWarning(string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogWarning(messageTemplate, propertyValues);
		}

		public void LogWarning(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogWarning(exception, messageTemplate, propertyValues);
		}

		public void LogError(string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogError(messageTemplate, propertyValues);
		}

		public void LogError(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogError(exception, messageTemplate, propertyValues);
		}

		public void LogFatal(string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogFatal(messageTemplate, propertyValues);
		}

		public void LogFatal(Exception exception, string messageTemplate, params object[] propertyValues)
		{
			ValidateFormat(messageTemplate, propertyValues);
			NullLogger.LogFatal(exception, messageTemplate, propertyValues);
		}

		public ILog ForContext<T>()
		{
			return NullLogger.ForContext<T>();
		}

		public ILog ForContext(Type forContext)
		{
			return NullLogger.ForContext(forContext);
		}

		public ILog ForContext(string propertyName, object value, bool destructureObjects)
		{
			return NullLogger.ForContext(propertyName, value, destructureObjects);
		}

		public IDisposable LogContextPushProperty(string propertyName, object obj)
		{
			return NullLogger.LogContextPushProperty(propertyName, obj);
		}

		private static void ValidateFormat(string format, params object[] args)
		{
			try
			{
				Assert.IsTrue(IsBalanced(format), "String has a weird number of brackets.");
				int numberOfClosingBrackets = format.Count(a => a == '}');
				if (args == null)
				{
					Assert.IsTrue(numberOfClosingBrackets == 0);
					return;
				}

				Assert.IsTrue(numberOfClosingBrackets == args.Length);
			}
			catch (Exception ex)
			{
				throw new ArgumentException(format + "<<<<< See " + nameof(TestNullLogger) + ".cs >>>>>" + string.Join(",", args.Select(a => a.ToString())), ex);
			}
		}

		private static bool IsBalanced(string input)
		{
			int count = 0;
			foreach (var ch in input)
			{
				if (ch == '{')
				{
					count++;
				}

				if (ch == '}')
				{
					count--;
				}

				if (count < 0 || count > 1)
				{
					return false;
				}
			}

			return count == 0;
		}
	}
}
