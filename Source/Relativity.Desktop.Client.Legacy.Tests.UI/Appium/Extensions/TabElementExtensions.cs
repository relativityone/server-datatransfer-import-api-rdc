using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class TabElementExtensions
	{
		public static AppiumWebElement FindTabItem(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.TabItem, name);
		}

		public static AppiumWebElement FindTabWithAutomationId(this AppiumWebElement element, string automationId)
		{
			return element.FindChildWithAutomationId(ElementType.Tab, automationId);
		}
	}
}