using System;
using System.Threading;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal static class Wait
	{
		public static bool For(Func<bool> condition)
		{
			return For(condition, DefaultTimeouts.DefaultWaitTimeout);
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

		public static Func<AppiumWebElement> WaitFor(this Func<AppiumWebElement> find)
		{
			return find.WaitFor(DefaultTimeouts.FindElement);
		}

		public static Func<AppiumWebElement> WaitFor(this Func<AppiumWebElement> find,
			TimeSpan timeout)
		{
			return find.WaitFor(TimeSpan.FromMilliseconds(200), timeout);
		}

		public static Func<AppiumWebElement> WaitFor(this Func<AppiumWebElement> find, TimeSpan checkInterval,
			TimeSpan timeout)
		{
			return () =>
			{
				var started = DateTime.Now;

				while (true)
				{
					try
					{
						var child = find();
						return child;
					}
					catch (InvalidOperationException)
					{
						if (DateTime.Now - started + checkInterval < timeout)
						{
							Thread.Sleep(checkInterval);
						}
						else
						{
							throw;
						}
					}
				}
			};
		}
	}
}