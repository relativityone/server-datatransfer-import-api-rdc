using System;
using System.Data.SqlClient;

namespace SQLDataComparer.Log
{
	public class ConsoleLog : ILog
	{
		private readonly bool _traceEnabled;

		public ConsoleLog(bool traceEnabled)
		{
			this._traceEnabled = traceEnabled;
		}

		private void Log(LogLevelEnum level, string message)
		{
			var color = Console.ForegroundColor;

			switch (level)
			{
				case LogLevelEnum.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;

				case LogLevelEnum.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
					
				default:
					Console.ForegroundColor = ConsoleColor.White;
					break;
			}

			Console.WriteLine(message);

			Console.ForegroundColor = color;
		}

		public void LogQuery(string query, SqlParameterCollection parameters)
		{
			if (_traceEnabled)
			{
				string queryWithParameters = query;

				foreach (SqlParameter parameter in parameters)
				{
					queryWithParameters = query.Replace(parameter.ParameterName, parameter.Value.ToString());
				}

				Log(LogLevelEnum.Info, queryWithParameters);
			}
		}

		public void LogInfo(string message)
		{
			Log(LogLevelEnum.Info, message);
		}

		public void LogWarning(string message)
		{
			Log(LogLevelEnum.Warning, message);
		}

		public void LogError(string message)
		{
			Log(LogLevelEnum.Error, message);
		}
	}
}
