using System;
using System.Collections.Generic;
using System.Threading;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	internal static class AppiumWebElementExtensions
	{
		public static IReadOnlyList<AppiumWebElement> FindChildren(this AppiumWebElement element)
		{
			return element.FindElementsByXPath("*/*");
		}

		public static AppiumWebElement FindChild(this AppiumWebElement element, string elementType)
		{
			return element.FindElementByXPath($"*//{elementType}");
		}

		public static AppiumWebElement FindChild(this AppiumWebElement element, string elementType, string name)
		{
			return element.FindElementByXPath($"*//{elementType}[@Name=\"{name}\"]");
		}

		public static AppiumWebElement FindChildWithClass(
			this AppiumWebElement element,
			string elementType,
			string name,
			string className)
		{
			return element.FindElementByXPath($"*//{elementType}[@ClassName=\"{className}\"][@Name=\"{name}\"]");
		}

		public static AppiumWebElement FindChildWithAutomationId(
			this AppiumWebElement element,
			string elementType,
			string automationId)
		{
			return element.FindElementByXPath($"*//{elementType}[@AutomationId=\"{automationId}\"]");
		}

		public static IReadOnlyList<AppiumWebElement> FindChildren(this AppiumWebElement element, string elementType)
		{
			return element.FindElementsByXPath($"*//{elementType}");
		}

		public static IReadOnlyList<AppiumWebElement> FindChildren(this AppiumWebElement element, string elementType,
			string name)
		{
			return element.FindElementsByXPath($"*//{elementType}[@Name=\"{name}\"]");
		}

		public static Func<AppiumWebElement> WaitFor(this Func<AppiumWebElement> find)
		{
			return find.WaitFor(TimeSpan.FromMilliseconds(200), DefaultTimeouts.FindElement);
		}

		public static Func<AppiumWebElement> WaitFor(this Func<AppiumWebElement> find, 
			TimeSpan timeout)
		{
			return find.WaitFor(TimeSpan.FromMilliseconds(200), timeout);
		}

		public static Func<AppiumWebElement> WaitFor(this Func<AppiumWebElement> find, TimeSpan checkInterval, TimeSpan timeout)
		{
			return () =>
			{
				var started = DateTime.Now;

				while (true)
				{
					try
					{
						AppiumWebElement child = find();
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