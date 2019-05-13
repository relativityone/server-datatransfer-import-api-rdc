namespace Relativity.DataExchange.Export.VolumeManagerV2.Settings
{
	using Relativity.DataExchange;

	public class Config
	{
		public int NumberOfIORetries => AppSettings.Instance.IoErrorNumberOfRetries;
		public int WaitTimeBetweenIORetryAttempts => AppSettings.Instance.IoErrorWaitTimeInSeconds;
	}
}