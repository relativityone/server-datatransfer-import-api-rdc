namespace kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings
{
	public class Config
	{
		public int NumberOfIORetries => kCura.Utility.Config.IOErrorNumberOfRetries;
		public int WaitTimeBetweenIORetryAttempts => kCura.Utility.Config.IOErrorWaitTimeInSeconds;
	}
}