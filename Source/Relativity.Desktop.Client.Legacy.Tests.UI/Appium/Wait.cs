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

		public static bool For(Func<bool> condition)
		{
			return For(condition, DefaultWaitTimeout);
		}

		public static bool For(Func<bool> condition, TimeSpan timeout)
		{
			return SpinWait.SpinUntil(condition, timeout);
		}

		public static bool For(Func<bool> condition, TimeSpan checkInterval, TimeSpan timeout)
		{
			var isConditionSatisfied = condition();
			var started = DateTime.Now;

			while (!isConditionSatisfied && DateTime.Now - started + checkInterval < timeout)
			{
				Thread.Sleep(checkInterval);
				isConditionSatisfied = condition();
			}

			return isConditionSatisfied;
		}
	}
}