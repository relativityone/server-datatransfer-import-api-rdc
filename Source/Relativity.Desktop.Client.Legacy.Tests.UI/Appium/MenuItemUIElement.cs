using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class MenuItemUIElement : UIElement<MenuItemUIElement>
	{
		public MenuItemUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
		}

		public MenuItemUIElement ClickMenuItem(string name)
		{
			var menuItem = FindMenuItem(name).WaitFor(TimeSpan.FromMilliseconds(500));
			menuItem.Click();
			return menuItem;
		}

		private MenuItemUIElement FindMenuItem(string name)
		{
			return new MenuItemUIElement(Logger, FindChild(ElementType.MenuItem, name));
		}
	}
}