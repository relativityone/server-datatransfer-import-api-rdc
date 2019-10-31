using System;
using System.Configuration;
using System.Threading;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal static class Wait
	{
		private static readonly TimeSpan DefaultWaitTimeout =
			TimeSpan.FromMilliseconds(
				double.Parse(ConfigurationManager.AppSettings["DefaultWaitTimeoutInMilliseconds"]));

		public static void For(Func<bool> condition)
		{
			For(condition, DefaultWaitTimeout);
		}

		public static void For(Func<bool> condition, TimeSpan timeout)
		{
			SpinWait.SpinUntil(condition, timeout);
		}

		public static void For(Func<bool> condition, TimeSpan checkInterval, TimeSpan timeout)
		{
			var started = DateTime.Now;

			while (!condition() && DateTime.Now - started + checkInterval < timeout) Thread.Sleep(checkInterval);
		}
	}
}