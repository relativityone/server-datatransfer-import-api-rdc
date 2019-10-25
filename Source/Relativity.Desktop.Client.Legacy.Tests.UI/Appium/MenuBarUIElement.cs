using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class MenuBarUIElement : MenuItemUIElement
	{
		public MenuBarUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}
	}

	public class MenuItemUIElement : UIElement
	{
		protected MenuItemUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public MenuItemUIElement ClickMenuItem(string name)
		{
			var menuItem = WaitForMenuItem(name);
			menuItem.Click();
			return menuItem;
		}

		private MenuItemUIElement WaitForMenuItem(string name)
		{
			var waitForChild = WaitForChild(ElementType.MenuItem, name, TimeSpan.FromMilliseconds(500));
			return new MenuItemUIElement(waitForChild);
		}
	}
}