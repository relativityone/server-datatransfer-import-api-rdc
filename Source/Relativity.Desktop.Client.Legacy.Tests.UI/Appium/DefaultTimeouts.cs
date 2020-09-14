using System;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public static class DefaultTimeouts
	{
		public static readonly TimeSpan WaitForProperty = TimeSpan.FromSeconds(1);
		public static readonly TimeSpan FindElement = TimeSpan.FromSeconds(1);
		public static readonly TimeSpan WaitForWindow = TimeSpan.FromMilliseconds(60000);
		public static readonly TimeSpan DefaultWaitTimeout = TimeSpan.FromMilliseconds(60000);
		public static readonly TimeSpan DefaultWaitCheckInterval = TimeSpan.FromMilliseconds(200);
	}
}