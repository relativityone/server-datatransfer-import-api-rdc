using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class ButtonElementExtensions
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
	}
}