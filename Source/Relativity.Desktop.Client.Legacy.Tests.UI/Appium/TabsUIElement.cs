using System;
using OpenQA.Selenium.Appium;
using Relativity.Logging;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	internal sealed class TabsUIElement : UIElement<TabsUIElement>
	{
		public TabsUIElement(ILog logger, Func<AppiumWebElement> create) : base(logger, create)
		{
		}

		public TabItemUIElement FindTabItem(string name)
		{
			return new TabItemUIElement(Logger, FindChild(ElementType.TabItem, name));
		}
	}
}