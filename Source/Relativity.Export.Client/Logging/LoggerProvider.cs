namespace kCura.WinEDDS.Core.Logging
{
	using global::Relativity;
	using global::Relativity.Logging;

	public class LoggerProvider
	{
		private static ILog _log;

		private static readonly object _lock = new object();

		public static ILog GetLogger()
		{
			if (_log == null)
			{
				lock (_lock)
				{
					if (_log == null)
					{
						_log = LoggerFactory.Create(ExecutionSource.Unknown);
					}
				}
			}
			return _log;
		}
	}
}