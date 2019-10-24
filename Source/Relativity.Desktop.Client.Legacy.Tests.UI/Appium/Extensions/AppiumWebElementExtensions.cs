using System.Collections.Generic;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	internal static class AppiumWebElementExtensions
	{
		public static AppiumWebElement FindEdit(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.Edit, name);
		}

		public static AppiumWebElement FindEditWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.Edit, automationId);
		}

		public static AppiumWebElement FindComboBox(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.ComboBox, name);
		}

		public static AppiumWebElement FindTitleBar(this AppiumWebElement element)
		{
			return element.FindChild(ElementType.TitleBar);
		}

		public static AppiumWebElement FindTreeWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.Tree, automationId);
		}

		public static AppiumWebElement FindTree(this AppiumWebElement element)
		{
			return element.FindChild(ElementType.Tree);
		}

		public static IReadOnlyList<AppiumWebElement> FindChildren(this AppiumWebElement element)
		{
			return element.FindElementsByXPath("*/*");
		}

		public static AppiumWebElement FindChild(this AppiumWebElement element, string elementType)
		{
			return element.FindElementByXPath($"*//{elementType}");
		}

		public static IReadOnlyList<AppiumWebElement> FindChildren(this AppiumWebElement element, string elementType)
		{
			return element.FindElementsByXPath($"*//{elementType}");
		}

		public static AppiumWebElement FindChild(this AppiumWebElement element, string elementType, string name)
		{
			return element.FindElementByXPath($"*//{elementType}[@Name=\"{name}\"]");
		}

		public static IReadOnlyList<AppiumWebElement> FindChildren(this AppiumWebElement element, string elementType,
			string name)
		{
			return element.FindElementsByXPath($"*//{elementType}[@Name=\"{name}\"]");
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

		public static IReadOnlyList<AppiumWebElement> FindChildrenWithAutomationId(
			this AppiumWebElement element,
			string elementType,
			string automationId)
		{
			return element.FindElementsByXPath($"*//{elementType}[@AutomationId=\"{automationId}\"]");
		}
	}
}