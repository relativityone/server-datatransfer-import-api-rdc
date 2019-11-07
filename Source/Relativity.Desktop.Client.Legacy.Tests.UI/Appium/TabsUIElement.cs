using System;
using OpenQA.Selenium.Appium;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class TabsUIElement : UIElement<TabsUIElement>
	{
		public TabsUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public TabItemUIElement FindTabItem(string name)
		{
			return new TabItemUIElement(FindChild(ElementType.TabItem, name));
		}
	}
}