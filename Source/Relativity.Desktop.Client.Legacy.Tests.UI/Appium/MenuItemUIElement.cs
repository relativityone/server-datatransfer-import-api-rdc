using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class MenuItemUIElement : UIElement<MenuItemUIElement>
	{
		public MenuItemUIElement(Func<AppiumWebElement> create) : base(create)
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
			return new MenuItemUIElement(FindChild(ElementType.MenuItem, name));
		}
	}
}