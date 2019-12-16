using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class ListUIElement : UIElement<ListUIElement>
	{
		public ListUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
		}

		public int ItemsCount => FindChildren(ElementType.ListItem).Count;

		public void SelectListItem(string name)
		{
			var item = FindListItem(name);
			item.Click();
			item.WaitToBeSelected();
		}

		public ListItemUIElement FindListItem(string name)
		{
			return new ListItemUIElement(Logger, FindChild(ElementType.ListItem, name));
		}
	}
}