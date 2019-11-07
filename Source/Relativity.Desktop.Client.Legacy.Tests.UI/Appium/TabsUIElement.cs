using System;
using OpenQA.Selenium.Appium;
using Relativity.Desktop.Client.Legacy.Tests.UI.Appium.Extensions;

namespace Relativity.Desktop.Client.Legacy.Tests.UI.Appium
{
	public class TabsUIElement : UIElement
	{
		public TabsUIElement(Func<AppiumWebElement> create) : base(create)
		{
		}

		public UIElement FindTabItem(string name)
		{
			return Create(() => Element.FindChild(ElementType.TabItem, name));
		}
	}
}