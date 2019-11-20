using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ListUIElement : UIElement<ListUIElement>
	{
		public ListUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public int ItemsCount => FindChildren(ElementType.ListItem).Count;

		public void SelectListItem(string name)
		{
			var item = FindListItem(name);
			item.Click();
		}

		public ListItemUIElement FindListItem(string name)
		{
			return new ListItemUIElement(FindChild(ElementType.ListItem, name));
		}
	}
}