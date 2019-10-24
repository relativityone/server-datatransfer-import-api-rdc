using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions
{
	public static class MenuElementExtensions
	{
		public static AppiumWebElement FindMenuBar(this AppiumWebElement element, string name)
		{
			return element.FindChild(ElementType.MenuBar, name);
		}

		public static AppiumWebElement WaitForMenuItem(this AppiumWebElement element, string name)
		{
			return element.WaitForChild(x => x.FindChildren(ElementType.MenuItem, name));
		}

		public static AppiumWebElement ClickMenuItem(this AppiumWebElement element, string name)
		{
			var menuItem = element.WaitForMenuItem(name);
			menuItem.Click();
			return menuItem;
		}
	}
}