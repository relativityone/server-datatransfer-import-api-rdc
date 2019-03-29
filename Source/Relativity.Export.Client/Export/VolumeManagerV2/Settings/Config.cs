namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	using global::Relativity.Import.Export;

	public class Config
	{
		public int NumberOfIORetries => AppSettings.Instance.IoErrorNumberOfRetries;
		public int WaitTimeBetweenIORetryAttempts => AppSettings.Instance.IoErrorWaitTimeInSeconds;
	}
}