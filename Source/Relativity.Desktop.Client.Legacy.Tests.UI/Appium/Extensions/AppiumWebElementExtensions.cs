using System.Collections.Generic;
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
	}
}