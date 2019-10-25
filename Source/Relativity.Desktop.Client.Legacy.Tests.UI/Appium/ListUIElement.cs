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

		public void ClickListItem(int index)
		{
			var listItemText = FindListItemTextByIndex(index);
			listItemText.Click();
		}

		private AppiumWebElement FindListItemTextByIndex(int index)
		{
			var listItem = FindListItemByIndex(index);
			return listItem.FindText();
		}

		//TODO: Refactor this to have ListViewItems collection field
		private AppiumWebElement FindListItemByIndex(int index)
		{
			return Element.FindElementByAccessibilityId($"ListViewItem-{index}");
		}
	}
}