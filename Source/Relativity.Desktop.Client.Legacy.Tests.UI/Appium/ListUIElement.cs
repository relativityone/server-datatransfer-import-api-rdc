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
			var item = FindListItemByName(name);
			item.Click();
		}

		private AppiumWebElement FindListItemByName(string name)
		{
			return FindChild(ElementType.ListItem, name)();
		}
	}
}