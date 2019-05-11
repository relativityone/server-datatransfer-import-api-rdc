namespace Relativity.DataExchange.Logging
{
	using System.IO;
	using System.Reflection;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Service;
	using Relativity.Logging;
	using Relativity.Logging.Factory;

	internal class LoggerFactory
	{
		public static ILog Create(ExecutionSource executionSource)
		{
			var loggerOptions = new LoggerOptions
			{
				System = executionSource.ToString(),
				ConfigurationFileLocation = GetLoggerConfigFilePath()
			};

			return LogFactory.GetLogger(loggerOptions);
		}

		private static string GetLoggerConfigFilePath()
		{
			var path = AppSettings.Instance.LogConfigXmlFileName;
			if (Path.IsPathRooted(path))
			{
				return path;
			}
			return Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName, "LogConfig.xml");
		}
	}
}