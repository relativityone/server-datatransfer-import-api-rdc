using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class ListUIElement : UIElement
	{
		public ListUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public int ItemsCount => Element.FindChildren(ElementType.ListItem).Count;

		public void SelectListItem(string name)
		{
			var item = FindListItemByName(name);
			item.Click();
		}

		private AppiumWebElement FindListItemByName(string name)
		{
			return Element.FindChild(ElementType.ListItem, name);
		}
	}
}