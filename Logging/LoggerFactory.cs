using System.IO;
using System.Reflection;
using Relativity;
using Relativity.Logging;
using Relativity.Logging.Factory;

namespace kCura.WinEDDS.Core.Logging
{
	public class LoggerFactory
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
			var path = Config.LogConfigFile;
			if (Path.IsPathRooted(path))
			{
				return path;
			}
			return Path.Combine(Directory.GetParent(Assembly.GetEntryAssembly().Location).FullName, "LoggerConfig.xml");
		}
	}
}