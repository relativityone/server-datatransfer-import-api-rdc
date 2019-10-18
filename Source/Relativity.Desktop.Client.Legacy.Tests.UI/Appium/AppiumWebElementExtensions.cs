namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	using System.Collections.Generic;

	using OpenQA.Selenium.Appium;

	internal static class AppiumWebElementExtensions
	{
		public static AppiumWebElement FindButton(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.Button, name);
		}

		public static AppiumWebElement FindButtonWithClass(this AppiumWebElement element, string name, string className)
		{
			return element.FindChildWithClass(ElementType.Button, name, className);
		}

		public static AppiumWebElement FindButtonWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.Button, automationId);
		}

		public static AppiumWebElement ClickButton(this AppiumWebElement element, string name)
		{
			var button = element.FindButton(name);
			button.Click();
			return button;
		}

		public static AppiumWebElement ClickButtonWithAutomationId(this AppiumWebElement element, string automationId)
		{
			var button = element.FindButtonWithAutomationId(automationId);
			button.Click();
			return button;
		}

		public static AppiumWebElement ClickButtonWithClass(
			this AppiumWebElement element,
			string name,
			string className)
		{
			var button = element.FindChildWithClass(ElementType.Button, name, className);
			button.Click();
			return button;
		}

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

		public static AppiumWebElement FindListWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.List, automationId);
		}

		public static AppiumWebElement FindText(this AppiumWebElement element)
		{
			return element.FindChild(ElementType.Text);
		}

		public static AppiumWebElement FindTreeWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.Tree, automationId);
		}

		public static AppiumWebElement FindMenuBar(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.MenuBar, name);
		}

		public static AppiumWebElement FindMenuItem(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.MenuItem, name);
		}

		public static AppiumWebElement ClickMenuItem(this AppiumWebElement element, string name)
		{
			var menuItem = element.FindMenuItem(name);
			menuItem.Click();
			Wait.Tiny();
			return menuItem;
		}

		public static AppiumWebElement FindTree(this AppiumWebElement element)
		{
			return element.FindChild(ElementType.Tree);
		}

		public static AppiumWebElement FindWindow(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.Window, name);
		}

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
	}
}