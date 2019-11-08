using System;
using System.Configuration;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public static class DefaultTimeouts
	{
		public static readonly TimeSpan WaitForProperty = TimeSpan.FromSeconds(1);
		public static readonly TimeSpan FindElement = TimeSpan.FromSeconds(1);

		public static readonly TimeSpan WaitForWindow =
			GetTimeoutFromConfig("DefaultWaitForWindowTimeoutInMilliseconds", 30000);

		public static readonly TimeSpan DefaultWaitTimeout =
			GetTimeoutFromConfig("DefaultWaitTimeoutInMilliseconds", 60000);

		private static TimeSpan GetTimeoutFromConfig(string settingsKey, double defaultTimeoutInMilliseconds)
		{
			var settingsValue = ConfigurationManager.AppSettings[settingsKey];
			var timeoutInMilliseconds =
				settingsValue != null ? double.Parse(settingsValue) : defaultTimeoutInMilliseconds;
			return TimeSpan.FromMilliseconds(timeoutInMilliseconds);
		}
	}
}